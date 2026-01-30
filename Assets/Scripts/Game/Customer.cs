using System;
using System.Linq;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Customer : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private float animationBaseTime = 0.25f;

    [SerializeField]
    private AYellowpaper.SerializedCollections.SerializedDictionary<int, float> patienceByDay;

    [SerializeField]
    private Transform weaponHoldingTransform;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool isOnCounter;

    [SerializeField]
    private MMF_Player entersStoreSFXPlayer;

    private Vector3 initialPosition;

    [SerializeField]
    private WeaponRequest weaponRequest;
    public WeaponRequest WeaponRequest => weaponRequest;

    private float _currentPatience;

    private DeliveryUI _deliveryUI;

    [SerializeField]
    private MMF_Player squashMmfPlayer;

    public void Initialize(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    void Awake()
    {
        weaponRequest.RandomizeRequest();
    }

    void Start()
    {
        initialPosition = transform.position;

        entersStoreSFXPlayer.PlayFeedbacks();

        _currentPatience = patienceByDay[LevelManager.Instance.CurrentDay];

        _deliveryUI = DeliveriesManager.Instance.CreateDeliveryWidget(_currentPatience, weaponRequest.Weapon, weaponRequest.Weight);
    }

    void Update()
    {
        _currentPatience -= Time.deltaTime;
        if (_currentPatience <= 0)
        {
            LevelManager.Instance.AddDelivery(false, false);
            GetComponent<Collider2D>().enabled = false;
            MoveToPosition(Vector3.right * 15, false);

            CustomersQueueManager.Instance.RemoveFromQueue(this);
            
            _deliveryUI.DeleteWidget();
            _currentPatience = 1000;
            Destroy(gameObject, 5f);
        }

    }

    public void MoveToPosition(Vector2 position, bool goingToCounter, float startingDelay = 0)
    {
        DOTween.Kill($"Breathing{GetInstanceID()}");
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(startingDelay);
        sequence.Append(transform.DOLocalMoveX(position.x, animationBaseTime * 8).SetEase(Ease.InOutSine));
        sequence.Join(transform.DOLocalMoveY(transform.localPosition.y + 0.25f, animationBaseTime).SetEase(Ease.InOutSine).SetLoops(8, LoopType.Yoyo));
        sequence.Join(transform.DOScaleX(0.95f, animationBaseTime).SetEase(Ease.InOutSine).SetLoops(8, LoopType.Yoyo));
        sequence.Join(transform.DOScaleY(1.05f, animationBaseTime).SetEase(Ease.InOutSine).SetLoops(8, LoopType.Yoyo));

        sequence.onComplete += () =>
        {
            Sequence breathingSequence = DOTween.Sequence();
            breathingSequence.AppendInterval(UnityEngine.Random.Range(0f, 0.5f));
            breathingSequence.Append(transform.DOMoveY(initialPosition.y + 0.15f, animationBaseTime * 8).SetEase(Ease.InOutSine));
            breathingSequence.Append(transform.DOMoveY(initialPosition.y, animationBaseTime * 8).SetEase(Ease.InOutSine));
            breathingSequence.SetLoops(-1, LoopType.Yoyo);
            breathingSequence.SetId($"Breathing{GetInstanceID()}");
            if (goingToCounter)
            {
                isOnCounter = goingToCounter;
                weaponRequest.ShowRequestBubble();
            }
        };
    }

    public bool ReceiveItem(FinishedWeapon finishedWeapon)
    {
        bool isRightWeapon = finishedWeapon.Weapon == weaponRequest.Weapon;
        bool isRightweight = finishedWeapon.Weight == weaponRequest.Weight;

        float goldMultiplier = 1;

        if (isOnCounter)
        {
            LevelManager.Instance.AddDelivery(isRightweight, isRightWeapon);

            finishedWeapon.transform.SetParent(transform);
            finishedWeapon.transform.position = weaponHoldingTransform.position;

            SortingGroup blueprintGroup = finishedWeapon.GetComponentInChildren<SortingGroup>();
            blueprintGroup.sortingLayerID = spriteRenderer.sortingLayerID;
            blueprintGroup.sortingOrder = spriteRenderer.sortingOrder + 1;

            weaponRequest.DisplayPerformance(isRightWeapon, isRightweight);

            if (!isRightWeapon)
            {
                goldMultiplier -= 0.5f;
            }

            if (!isRightweight)
            {
                goldMultiplier -= 0.5f;
            }

            MoveToPosition(Vector3.right * 15, false, 1);

            CustomersQueueManager.Instance.RemoveFromQueue(this);
            _deliveryUI.DeleteWidget();
            GoldManager.Instance.AddGold((int)(50 * goldMultiplier));

            _currentPatience = 1000;

            DOTween.Kill(gameObject);
            Destroy(gameObject, 5f);
        }

        return isOnCounter;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        squashMmfPlayer.PlayFeedbacks();
    }
}
