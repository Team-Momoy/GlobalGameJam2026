using System.Collections;
using Assets.SimpleLocalization.Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndOfDayScreen : MonoBehaviour
{
    [SerializeField]
    private LocalizedTextMeshPro titleTMP, goodTMP, neutralTMP, badTMP;

    [SerializeField]
    private RectTransform container;

    [SerializeField]
    private Image deliveryPrefab;

    [SerializeField]
    private RectTransform deliveriesContainer;

    [SerializeField]
    private Sprite goodDeliverySprite;
    [SerializeField]
    private Sprite neutralDeliverySprite;
    [SerializeField]
    private Sprite badDeliverySprite;

    [SerializeField]
    private RectTransform seal;

    void Start()
    {
        CustomersQueueManager.Instance.OnStoreClosedAndEmpty += StoreClosedAndEmpty;
    }

    private void StoreClosedAndEmpty()
    {
        OverlayManager.Instance.FadeToBlack(0.7f);

        var deliveries = LevelManager.Instance.Deliveries;

        titleTMP.LocalizationVariables = new object[] { LevelManager.Instance.CurrentDay };
        goodTMP.LocalizationVariables = new object[] { deliveries[DeliveryRating.Good], deliveries[DeliveryRating.Good] * 50 };
        neutralTMP.LocalizationVariables = new object[] { deliveries[DeliveryRating.Neutral], deliveries[DeliveryRating.Neutral] * 25 };
        badTMP.LocalizationVariables = new object[] { deliveries[DeliveryRating.Bad], 0 };

        ClearPreviousDeliveries();

        // Start coroutine to instantiate deliveries with animation
        StartCoroutine(InstantiateDeliveriesWithDelay(
            new (int count, Sprite sprite)[] {
                (deliveries[DeliveryRating.Good], goodDeliverySprite),
                (deliveries[DeliveryRating.Neutral], neutralDeliverySprite),
                (deliveries[DeliveryRating.Bad], badDeliverySprite)
            }
        ));

        AnimateContainer();
    }

    private void ClearPreviousDeliveries()
    {
        if (deliveriesContainer == null) return;
        for (int i = deliveriesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(deliveriesContainer.GetChild(i).gameObject);
        }
    }

    private IEnumerator InstantiateDeliveriesWithDelay((int count, Sprite sprite)[] deliveryData)
    {
        float delay = 0.2f;

        yield return new WaitForSeconds(1f);

        foreach (var (count, sprite) in deliveryData)
        {
            for (int i = 0; i < count; i++)
            {
                if (deliveryPrefab == null || deliveriesContainer == null) continue;

                Image img = Instantiate(deliveryPrefab, deliveriesContainer);
                img.sprite = sprite;
                img.transform.localScale = Vector3.zero;
                img.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack, 3);

                yield return new WaitForSeconds(delay);
            }
        }
    }

    public void GoToNextDay()
    {
        LevelManager.Instance.AdvanceToNextDay();
        OverlayManager.Instance.Clear(0.7f);
    }

    private void AnimateContainer()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(container.DOAnchorPos(Vector2.zero, 2f).SetDelay(2f).SetEase(Ease.OutSine));
        sequence.Append(seal.DOLocalRotate(Vector3.back * 360, 1f, RotateMode.LocalAxisAdd)).SetEase(Ease.OutBack, 3);
    }

    void OnDisable()
    {
        CustomersQueueManager.Instance.OnStoreClosedAndEmpty -= StoreClosedAndEmpty;
    }
}
