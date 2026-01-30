using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class MetalShutterAdditiveScene : MonoBehaviour
{
    [SerializeField] private MMF_Player _openShuttersFeedback;
    [SerializeField] private MMF_Player _closeShuttersFeedback;

    public void HoldLoadingScreenBeforeActivation()
    {
        _closeShuttersFeedback.PlayFeedbacks();
        MMAdditiveSceneLoadingManager.SetHold(MMAdditiveSceneLoadingManager.HoldModes.AfterEntryFade, true);
        StartCoroutine(WaitAndClearHold());
    }

    private IEnumerator WaitAndClearHold()
    {
        yield return new WaitUntil(() => !_closeShuttersFeedback.IsPlaying);
        MMAdditiveSceneLoadingManager.SetHold(MMAdditiveSceneLoadingManager.HoldModes.AfterEntryFade, false);
    }

    public void HoldLoadingScreenAfterActivation()
    {
        MMAdditiveSceneLoadingManager.SetHold(MMAdditiveSceneLoadingManager.HoldModes.BeforeExitFade, true);
        StartCoroutine(WaitAndOpenShutters());
    }

    private IEnumerator WaitAndOpenShutters()
    {
        _openShuttersFeedback.PlayFeedbacks();
        yield return new WaitUntil(() => !_openShuttersFeedback.IsPlaying);
        MMAdditiveSceneLoadingManager.SetHold(MMAdditiveSceneLoadingManager.HoldModes.BeforeExitFade, false);
    }
}
