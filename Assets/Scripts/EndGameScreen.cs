using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private LocalizedTextMeshPro localizedEndGameTMP;
    [SerializeField] private GameObject retryButton, backToMenuButton;

    public void EndGame(bool won)
    {
        OverlayManager.Instance.FadeToBlack(0.8f);
        if (won)
        {
            localizedEndGameTMP.LocalizationKey = "Game.ImpostorKill";  
            backToMenuButton.SetActive(true);
        }
        else
        {
            localizedEndGameTMP.LocalizationKey = "Game.InnocentKill";
            retryButton.SetActive(true);
        }
    }

    public void RestartScene()
    {
        OverlayManager.Instance.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
