using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform[] cameraPositions;
    [SerializeField]
    private Transform initialPosition;

    [SerializeField]
    private GameObject leftArrow, rightArrow;

    [SerializeField]
    private MMF_Player swooshSFX;

    private int currentIndex;

    void Awake()
    {
        currentIndex = System.Array.IndexOf(cameraPositions, initialPosition);
    }

    public void MoveLeft()
    {
        swooshSFX.PlayFeedbacks();
        if (currentIndex > 0)
        {
            currentIndex--;
            transform.DOMove(cameraPositions[currentIndex].position, 0.5f).SetEase(Ease.InOutSine);
            leftArrow.SetActive(false);
            rightArrow.SetActive(true);
        }
    }

    public void MoveRight()
    {
        swooshSFX.PlayFeedbacks();
        if (currentIndex < cameraPositions.Length - 1)
        {
            currentIndex++;
            transform.DOMove(cameraPositions[currentIndex].position, 0.5f).SetEase(Ease.InOutSine);
            leftArrow.SetActive(true);
            rightArrow.SetActive(false);
        }
    }
}
