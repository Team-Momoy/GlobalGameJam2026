using UnityEngine;

public class PlayTutorialButton : MonoBehaviour
{
    public void PlayTutorial()
    {
        PlayerPrefs.SetInt("CurrentDay", 0);
        PlayerPrefs.SetInt("TutorialCompleted", 0);
    }
}
