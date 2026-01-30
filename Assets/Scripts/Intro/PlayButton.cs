using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    private MMF_Player loadingIntro, loadingGame;
    public void Play()
    {
        if(PlayerPrefs.GetInt("IntroWatched", 0) == 0)
        {
            loadingIntro.PlayFeedbacks();
        }
        else
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.SetInt("CurrentDay", 1);
            loadingGame.PlayFeedbacks();
        }
    }
}
