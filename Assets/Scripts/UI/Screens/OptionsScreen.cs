using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    public static OptionsScreen Instance;

    [SerializeField]
    private RectTransform optionsScreen;

    [SerializeField]
    private Slider sfxSlider, musicSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        sfxSlider.value = MMSoundManager.Instance.GetTrackVolume(MMSoundManager.MMSoundManagerTracks.Sfx, false);
        musicSlider.value = MMSoundManager.Instance.GetTrackVolume(MMSoundManager.MMSoundManagerTracks.Music, false);

        // initialize language index from saved preference (or default)
        string saved = PlayerPrefs.GetString("Language", "English");
        int idx = System.Array.IndexOf(languages, saved);
        if (idx < 0) idx = 0;
        languageIndex = idx;
        ChangeLanguage(languages[languageIndex]);
    }

    public void Open()
    {
        OverlayManager.Instance.FadeToBlack(0.7f);

        optionsScreen.gameObject.SetActive(true);
        optionsScreen.DOAnchorPosY(0, 0.3f).SetEase(Ease.OutBack, 3);
    }

    public void Close()
    {
        OverlayManager.Instance.Clear();

        optionsScreen.DOAnchorPosY(1200, 0.3f).SetEase(Ease.InBack, 3).OnComplete(() =>
        {
            optionsScreen.gameObject.SetActive(false);
        });
    }

    public void SetSfxVolume(float volume)
    {
        MMSoundManager.Instance.SetVolumeSfx(volume);
    }

    public void SetMusicVolume(float volume)
    {
        MMSoundManager.Instance.SetVolumeMusic(volume);
    }

    private string[] languages = { "English", "Spanish" };
    private int languageIndex = 0;

    public void ChangeLanguage(string language)
    {
        if (languages == null || languages.Length == 0)
            return;

        LocalizationManager.Language = language;
        PlayerPrefs.SetString("Language", language);

        int idx = System.Array.IndexOf(languages, language);
        languageIndex = (idx >= 0) ? idx : 0;
    }

    // Move to next language (loops to start)
    public void NextLanguage()
    {
        if (languages == null || languages.Length == 0)
            return;

        languageIndex = (languageIndex + 1) % languages.Length;
        ChangeLanguage(languages[languageIndex]);
    }

    // Move to previous language (loops to end)
    public void PreviousLanguage()
    {
        if (languages == null || languages.Length == 0)
            return;

        languageIndex = (languageIndex - 1 + languages.Length) % languages.Length;
        ChangeLanguage(languages[languageIndex]);
    }
}
