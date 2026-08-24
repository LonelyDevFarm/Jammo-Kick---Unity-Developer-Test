using UnityEngine;

public sealed class GoalController : MonoBehaviour
{
    [SerializeField] private ParticleSystem celebrationEffect;

    public Vector3 TargetPosition => transform.position;

    public void Celebrate()
    {
        if (celebrationEffect == null)
        {
            return;
        }

        celebrationEffect.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);

        celebrationEffect.Play(true);
    }
}