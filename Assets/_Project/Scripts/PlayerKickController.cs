using System;
using UnityEngine;

public sealed class PlayerKickController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallController[] balls;
    [SerializeField] private Transform[] goalTargets;

    [Header("Kick")]
    [SerializeField, Min(0f)] private float kickRange = 1.5f;
    [SerializeField, Min(0.01f)] private float flightDuration = 1.2f;
    [SerializeField, Min(0f)] private float arcHeight = 2f;

    private BallController nearbyBall;

    public bool CanKick => nearbyBall != null;

    public event Action<bool> KickAvailabilityChanged;
    public event Action<BallController> BallKicked;

    private void Update()
    {
        SetNearbyBall(FindNearestBallInRange());
    }

    public void KickNearbyBall()
    {
        if (nearbyBall == null)
        {
            return;
        }

        Kick(nearestBall: nearbyBall);
    }

    public void AutoKick()
    {
        BallController farthestBall = FindFarthestAvailableBall();

        if (farthestBall == null)
        {
            return;
        }

        Kick(nearestBall: farthestBall);
    }

    private void Kick(BallController nearestBall)
    {
        Transform goalTarget =
            FindNearestGoal(nearestBall.transform.position);

        if (goalTarget == null)
        {
            return;
        }

        if (!nearestBall.TryKick(
                goalTarget,
                flightDuration,
                arcHeight))
        {
            return;
        }

        BallKicked?.Invoke(nearestBall);

        if (nearestBall == nearbyBall)
        {
            SetNearbyBall(null);
        }
    }

    private BallController FindNearestBallInRange()
    {
        BallController result = null;
        float nearestSqrDistance = kickRange * kickRange;

        if (balls == null)
        {
            return null;
        }

        for (int i = 0; i < balls.Length; i++)
        {
            BallController ball = balls[i];

            if (ball == null || !ball.IsAvailable)
            {
                continue;
            }

            Vector3 offset =
                ball.transform.position - transform.position;

            offset.y = 0f;
            float sqrDistance = offset.sqrMagnitude;

            if (sqrDistance > nearestSqrDistance)
            {
                continue;
            }

            nearestSqrDistance = sqrDistance;
            result = ball;
        }

        return result;
    }

    private BallController FindFarthestAvailableBall()
    {
        BallController result = null;
        float farthestSqrDistance = -1f;

        if (balls == null)
        {
            return null;
        }

        for (int i = 0; i < balls.Length; i++)
        {
            BallController ball = balls[i];

            if (ball == null || !ball.IsAvailable)
            {
                continue;
            }

            Vector3 offset =
                ball.transform.position - transform.position;

            offset.y = 0f;
            float sqrDistance = offset.sqrMagnitude;

            if (sqrDistance <= farthestSqrDistance)
            {
                continue;
            }

            farthestSqrDistance = sqrDistance;
            result = ball;
        }

        return result;
    }

    private Transform FindNearestGoal(Vector3 ballPosition)
    {
        Transform result = null;
        float nearestSqrDistance = float.MaxValue;

        if (goalTargets == null)
        {
            return null;
        }

        for (int i = 0; i < goalTargets.Length; i++)
        {
            Transform goal = goalTargets[i];

            if (goal == null)
            {
                continue;
            }

            float sqrDistance =
                (goal.position - ballPosition).sqrMagnitude;

            if (sqrDistance >= nearestSqrDistance)
            {
                continue;
            }

            nearestSqrDistance = sqrDistance;
            result = goal;
        }

        return result;
    }

    private void SetNearbyBall(BallController newBall)
    {
        if (nearbyBall == newBall)
        {
            return;
        }

        nearbyBall = newBall;
        KickAvailabilityChanged?.Invoke(CanKick);
    }
}