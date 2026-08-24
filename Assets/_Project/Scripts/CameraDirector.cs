using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public sealed class CameraDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private PlayerKickController kickController;
    [SerializeField] private Transform playerTarget;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float returnDelay = 2f;

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

        if (virtualCamera == null ||
            kickController == null ||
            playerTarget == null)
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

        virtualCamera.Follow = ball.transform;
        virtualCamera.LookAt = ball.transform;

        SetFollowingBall(true);
    }

    private void HandleBallReachedTarget(BallController ball)
    {
        if (ball != followedBall)
        {
            return;
        }

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
        virtualCamera.Follow = playerTarget;
        virtualCamera.LookAt = playerTarget;

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