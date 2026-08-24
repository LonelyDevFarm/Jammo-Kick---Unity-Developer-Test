using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class GameUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerKickController playerKickController;
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

        kickButton.onClick.AddListener(
            playerKickController.KickNearbyBall);

        autoKickButton.onClick.AddListener(
            playerKickController.AutoKick);

        resetButton.onClick.AddListener(ReloadScene);

        isRegistered = true;

        SetKickButtonVisible(playerKickController.CanKick);
    }

    private void OnDisable()
    {
        if (!isRegistered)
        {
            return;
        }

        playerKickController.KickAvailabilityChanged -=
            SetKickButtonVisible;

        kickButton.onClick.RemoveListener(
            playerKickController.KickNearbyBall);

        autoKickButton.onClick.RemoveListener(
            playerKickController.AutoKick);

        resetButton.onClick.RemoveListener(ReloadScene);

        isRegistered = false;
    }

    private void SetKickButtonVisible(bool isVisible)
    {
        kickButton.gameObject.SetActive(isVisible);
    }

    private static void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}