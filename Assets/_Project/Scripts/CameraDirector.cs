using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class CameraDirector : MonoBehaviour
{
    [Header("References")]
    [FormerlySerializedAs("virtualCamera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera ballCamera;
    [SerializeField] private PlayerKickController kickController;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform ballCameraRig;

    [Header("Priority")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float returnDelay = 1.2f;

    private BallController followedBall;
    private Coroutine returnRoutine;
    private bool isRegistered;

    public bool IsFollowingBall { get; private set; }

    public event Action<bool> FollowingBallChanged;

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (playerCamera == null ||
            ballCamera == null ||
            kickController == null ||
            playerTarget == null ||
            ballCameraRig == null)
        {
            Debug.LogError(
                "CameraDirector is missing references.",
                this);

            enabled = false;
            return;
        }

        kickController.BallKicked += FollowBall;
        isRegistered = true;

        FollowPlayer();
    }

    private void LateUpdate()
    {
        if (followedBall == null)
        {
            return;
        }

        ballCameraRig.position = followedBall.transform.position;
    }

    private void OnDisable()
    {
        if (!isRegistered)
        {
            return;
        }

        kickController.BallKicked -= FollowBall;
        UnsubscribeFromBall();

        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }

        isRegistered = false;
    }

    private void FollowBall(BallController ball)
    {
        if (ball == null)
        {
            return;
        }

        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }

        UnsubscribeFromBall();

        followedBall = ball;
        followedBall.ReachedTarget += HandleBallReachedTarget;

        PositionBallCameraRig(ball);

        ballCamera.Follow = ballCameraRig;
        ballCamera.LookAt = ballCameraRig;
        ballCamera.PreviousStateIsValid = false;

        playerCamera.Priority = inactivePriority;
        ballCamera.Priority = activePriority;

        SetFollowingBall(true);
    }

    private void PositionBallCameraRig(BallController ball)
    {
        Vector3 direction =
            ball.TargetPosition - ball.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Vector3.forward;
        }

        ballCameraRig.SetPositionAndRotation(
            ball.transform.position,
            Quaternion.LookRotation(
                direction.normalized,
                Vector3.up));
    }

    private void HandleBallReachedTarget(BallController ball)
    {
        if (ball != followedBall)
        {
            return;
        }

        ballCameraRig.position = ball.transform.position;

        UnsubscribeFromBall();
        returnRoutine = StartCoroutine(ReturnToPlayer());
    }

    private IEnumerator ReturnToPlayer()
    {
        yield return new WaitForSeconds(returnDelay);

        returnRoutine = null;
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        playerCamera.Follow = playerTarget;
        playerCamera.LookAt = playerTarget;

        playerCamera.Priority = activePriority;
        ballCamera.Priority = inactivePriority;

        SetFollowingBall(false);
    }

    private void UnsubscribeFromBall()
    {
        if (followedBall == null)
        {
            return;
        }

        followedBall.ReachedTarget -= HandleBallReachedTarget;
        followedBall = null;
    }

    private void SetFollowingBall(bool isFollowing)
    {
        if (IsFollowingBall == isFollowing)
        {
            return;
        }

        IsFollowingBall = isFollowing;
        FollowingBallChanged?.Invoke(isFollowing);
    }
}