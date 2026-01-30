using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    //I do this to avoid having a copy of the OptionsButton in each scene
    private static OptionsButton Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(transform.parent.gameObject);
        }

        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public void OpenOptions()
    {
        OptionsScreen.Instance.Open();
    }
}
