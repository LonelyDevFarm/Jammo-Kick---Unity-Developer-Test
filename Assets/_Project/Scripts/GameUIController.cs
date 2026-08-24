using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerKickController playerKickController;
    [SerializeField] private CameraDirector cameraDirector;
    [SerializeField] private Button kickButton;
    [SerializeField] private Button autoKickButton;
    [SerializeField] private Button resetButton;

    private bool isRegistered;

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (playerKickController == null ||
            cameraDirector == null ||
            kickButton == null ||
            autoKickButton == null ||
            resetButton == null)
        {
            Debug.LogError(
                "GameUIController is missing references.",
                this);

            enabled = false;
            return;
        }

        playerKickController.KickAvailabilityChanged +=
            SetKickButtonVisible;

        cameraDirector.FollowingBallChanged +=
            SetKickSequenceActive;

        kickButton.onClick.AddListener(
            playerKickController.KickNearbyBall);

        autoKickButton.onClick.AddListener(
            playerKickController.AutoKick);

        resetButton.onClick.AddListener(ReloadScene);

        isRegistered = true;

        SetKickSequenceActive(cameraDirector.IsFollowingBall);
    }

    private void OnDisable()
    {
        if (!isRegistered)
        {
            return;
        }

        if (playerKickController != null)
        {
            playerKickController.KickAvailabilityChanged -=
                SetKickButtonVisible;
        }

        if (cameraDirector != null)
        {
            cameraDirector.FollowingBallChanged -=
                SetKickSequenceActive;
        }

        if (kickButton != null)
        {
            kickButton.onClick.RemoveListener(
                playerKickController.KickNearbyBall);
        }

        if (autoKickButton != null)
        {
            autoKickButton.onClick.RemoveListener(
                playerKickController.AutoKick);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ReloadScene);
        }

        isRegistered = false;
    }

    private void SetKickSequenceActive(bool isFollowingBall)
    {
        kickButton.interactable = !isFollowingBall;
        autoKickButton.interactable = !isFollowingBall;

        SetKickButtonVisible(playerKickController.CanKick);
    }

    private void SetKickButtonVisible(bool isVisible)
    {
        kickButton.gameObject.SetActive(
            isVisible && !cameraDirector.IsFollowingBall);
    }

    private static void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}