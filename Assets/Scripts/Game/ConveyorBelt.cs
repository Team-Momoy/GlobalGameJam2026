using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField]
    private Transform leverTransform, mainContainer;

    [SerializeField]
    private SpriteRenderer beltSpriteRenderer;

    [SerializeField]
    private MMF_Player leverSFXPlayer, loopSFXPlayer;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform itemsContainer;

    private MaterialPropertyBlock _materialPropertyBlock;

    private float currentSpeed = 0f;

    private bool isRunning = false;

    public bool IsRunning => isRunning;


    public void ToggleConveyor()
    {
        DOTween.Kill("ConveyorBeltSpeedTween", true);
        isRunning = !isRunning;

        leverSFXPlayer.PlayFeedbacks();

        leverTransform.DORotate(new Vector3(0, 0, 42 * (isRunning ? -1 : 1)), 0.5f).SetEase(Ease.OutBack);

        //Tween the speed to 1 if isRunning is true, otherwise to 0
        float targetSpeed = isRunning ? 1f : 0f;
        DOTween.To(() => currentSpeed, x => currentSpeed = x, targetSpeed, 0.5f).SetEase(Ease.OutQuad).SetId("ConveyorBeltSpeedTween");

        if (!isRunning)
        {
            //Wiggle the conveyor belt on the y axis
            mainContainer.DOLocalMoveY(0.2f, 0.1f).SetLoops(4, LoopType.Yoyo).SetEase(Ease.OutQuad).SetId("ConveyorBeltSpeedTween");
            loopSFXPlayer.StopFeedbacks();
        }
        else
        {
            loopSFXPlayer.PlayFeedbacks();
        }
    }

    void Awake()
    {
        _materialPropertyBlock = new();
    }

    private float currentScroll = 0f;

    void Update()
    {
        if (isRunning)
        {
            currentScroll += currentSpeed * speed * Time.deltaTime;
            beltSpriteRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat("_Offset", currentScroll);
            beltSpriteRenderer.SetPropertyBlock(_materialPropertyBlock);

            //Move each item transform to the right based on delta time
            foreach (Transform item in itemsContainer)
            {
                //If the item is out of bounds, move it to the left side
                if (item.localPosition.x > 4f)
                {
                    ToggleConveyor();
                    continue;
                }

                item.Translate(currentSpeed * speed * 1.95f * Time.deltaTime * Vector2.right, Space.World);

            }
        }
    }

    public void PlaceItem(Transform item)
    {
        item.SetParent(itemsContainer);
    }


}
