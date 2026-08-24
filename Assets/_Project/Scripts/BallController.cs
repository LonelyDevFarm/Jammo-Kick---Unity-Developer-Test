using System;
using System.Collections;
using UnityEngine;

public sealed class BallController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float rotationSpeed = 720f;

    private Coroutine flightRoutine;

    public bool IsAvailable { get; private set; } = true;

    public event Action<BallController> ReachedTarget;

    public bool TryKick(
        GoalController target,
        float duration,
        float arcHeight)
    {
        if (!IsAvailable || target == null)
        {
            return false;
        }

        IsAvailable = false;
        flightRoutine = StartCoroutine(
            FlyToTarget(target, duration, arcHeight));

        return true;
    }

    private IEnumerator FlyToTarget(
        GoalController target,
        float duration,
        float arcHeight)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.TargetPosition;

        duration = Mathf.Max(duration, 0.01f);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);

            Vector3 position =
                Vector3.Lerp(startPosition, targetPosition, progress);

            position.y +=
                4f * arcHeight * progress * (1f - progress);

            transform.position = position;

            transform.Rotate(
                Vector3.right,
                rotationSpeed * Time.deltaTime,
                Space.Self);

            yield return null;
        }

        transform.position = targetPosition;
        flightRoutine = null;

        target.Celebrate();
        ReachedTarget?.Invoke(this);
    }

    private void OnDisable()
    {
        if (flightRoutine == null)
        {
            return;
        }

        StopCoroutine(flightRoutine);
        flightRoutine = null;
    }
}