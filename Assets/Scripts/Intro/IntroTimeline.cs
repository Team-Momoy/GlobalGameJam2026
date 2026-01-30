using System;
using System.Collections;
using Assets.SimpleLocalization.Scripts;
using Febucci.UI;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class IntroTimeline : MonoBehaviour
{
    // This class is use to play the intro cinematic, it should allowed me to change the current image, play sound effects, music, change texts
    // and fade in and out an overlay, also to enable and disable gameobjects. when pressing the skip button it should forward to the next step but if the text
    // is still being written it should finish that instantly and not skip to the next step.

    //Make this an array of arrays of steps, so each step can have multiple actions at the same time
    [SerializeField]
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false, ShowItemCount = true)]
    private Slides[] slides;

    [SerializeField]
    private LocalizedTextMeshPro introText;

    [SerializeField]
    private TypewriterByCharacter typewriter, dialogBoxTypewriter;

    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private AudioClip characterVisibleSfx;

    [SerializeField]
    private MMF_Player goToGameFeedback;

    Coroutine currentSlideCoroutine;

    private int currentSlideIndex = 0;

    private bool isLoadingNextScene = false;


    public void AdvanceSlide()
    {
        if (typewriter.isShowingText)
        {
            typewriter.SkipTypewriter();
            return;
        }
        if (dialogBoxTypewriter.isShowingText)
        {
            dialogBoxTypewriter.SkipTypewriter();
            return;
        }
        currentSlideIndex++;
        if (currentSlideIndex >= slides.Length)
        {
            if (isLoadingNextScene)
            {
                return;
            }
            isLoadingNextScene = true;
            goToGameFeedback.PlayFeedbacks();
            return;
        }
        if (currentSlideCoroutine != null)
        {
            StopCoroutine(currentSlideCoroutine);
        }
        currentSlideCoroutine = StartCoroutine(PlaySlides());
    }
    
    private IEnumerator PlaySlides()
    {
        Slides currentSlides = slides[currentSlideIndex];
        for (int i = 0; i < currentSlides.slideEvents.Length; i++)
        {
            yield return StartCoroutine(PlaySlideEvent(currentSlides.slideEvents[i]));
        }
    }

    private void Start()
    {
        PlayerPrefs.SetInt("IntroWatched", 1);
        StartCoroutine(PlaySlides());
    }

    private IEnumerator PlaySlideEvent(SlideEvent slideEvent)
    {
        switch (slideEvent.stepType)
        {
            case SlideEvent.SlideEventType.NonWaitingClear:
                OverlayManager.Instance.Clear(0.75f);
                break;
            case SlideEvent.SlideEventType.Delay:
                yield return new WaitForSeconds(slideEvent.delayDuration);
                break;
            case SlideEvent.SlideEventType.Text:
                introText.LocalizationKey = slideEvent.text;
                break;
            case SlideEvent.SlideEventType.Image:
                backgroundImage.sprite = slideEvent.image;
                break;
            case SlideEvent.SlideEventType.SoundEffect:
                MMSoundManager.Instance.PlaySound(slideEvent.audioClip, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
                break;
            case SlideEvent.SlideEventType.Music:
                MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.StopTrack, MMSoundManager.MMSoundManagerTracks.Music);
                MMSoundManager.Instance.PlaySound(slideEvent.audioClip, MMSoundManager.MMSoundManagerTracks.Music, Vector3.zero);
                break;
            case SlideEvent.SlideEventType.Fade:
                OverlayManager.Instance.FadeToBlack(1, 0.75f);
                yield return new WaitForSeconds(0.75f);
                break;
            case SlideEvent.SlideEventType.Clear:
                OverlayManager.Instance.Clear(0.75f);
                yield return new WaitForSeconds(0.75f);
                break;
            case SlideEvent.SlideEventType.EnableGameObject:
                slideEvent.gameObject.SetActive(true);
                break;
            case SlideEvent.SlideEventType.DisableGameObject:
                slideEvent.gameObject.SetActive(false);
                break;
        }
    }

    public void OnCharacterVisible()
    {
        MMSoundManager.Instance.PlaySound(characterVisibleSfx, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
    }
}

[Serializable]
internal class Slides
{
    public SlideEvent[] slideEvents;
}

[Serializable]
internal class SlideEvent
{
    public enum SlideEventType
    {
        Text,
        Image,
        SoundEffect,
        Music,
        Fade,
        Clear,
        EnableGameObject,
        DisableGameObject,
        Delay,
        NonWaitingClear,
    }

    public SlideEventType stepType;

    [ShowIf("stepType", SlideEventType.Delay)]
    public float delayDuration;

    [ShowIf("stepType", SlideEventType.Text)]
    public string text;
    [ShowIf("stepType", SlideEventType.Image)]
    public Sprite image;
    [ShowIf("@this.stepType == SlideEventType.SoundEffect || this.stepType == SlideEventType.Music")]
    public AudioClip audioClip;
    [ShowIf("@this.stepType == SlideEventType.EnableGameObject || this.stepType == SlideEventType.DisableGameObject")]
    public GameObject gameObject;


}