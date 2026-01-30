using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : SerializedMonoBehaviour
{
    public static PressurePlate Instance;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Animator starBurstVFX;

    [SerializeField]
    private ParticleSystem gameLostVFX;
    
    [SerializeField]
    private UnityEvent OnPlatePressed;
    [SerializeField]
    private UnityEvent OnPlateReleased;

    [SerializeField]
    private GameObject countdownGameobject;
    [SerializeField]
    private TextMeshProUGUI countdownTMP;

    [SerializeField]
    private Dictionary<Weapon, Minigame> minigamesByWeapon;

    [SerializeField]
    private WeightScaleDial weightScaleDial;

    private WeaponBlueprint assignedBlueprint;

    [SerializeField]
    private Transform blueprintSpawnPoint;

    private int idealWeight;

    public bool isAssigned => assignedBlueprint != null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AssignObject(WeaponBlueprint obj)
    {
        if(assignedBlueprint) ReleaseObject();

        assignedBlueprint = obj;
        OnPlatePressed?.Invoke();

        obj.OnWeightChanged += OnWeightChanged;


        assignedBlueprint.transform.DOMove(blueprintSpawnPoint.position, 0.75f).SetEase(Ease.OutBack);
    }

    private void OnWeightChanged(int weight)
    {
        if(idealWeight == weight)
        {
            assignedBlueprint.OnWeightChanged -= OnWeightChanged;
            // assignedBlueprint.FinishWeapon();
            // assignedBlueprint = null;
            OnPlateReleased?.Invoke();
            weightScaleDial.SetWeight(0);

            return;
        }

        weightScaleDial.SetWeight(weight);
    }

    public void ReleaseObject()
    {
        assignedBlueprint.Reset();
        Destroy(assignedBlueprint.gameObject);
        assignedBlueprint = null;
        
        OnPlateReleased?.Invoke();
        weightScaleDial.SetWeight(0);
    }

    public IEnumerator EnableMinigame()
    {
        if(assignedBlueprint == null) yield break;
        CameraZoomIn();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(RunCountdown());
        // yield return new WaitForSeconds(1f);

        minigamesByWeapon[assignedBlueprint.Weapon].gameObject.SetActive(true);

        minigamesByWeapon[assignedBlueprint.Weapon].OnGameFinished += OnMinigameFinished;
    }

    private IEnumerator RunCountdown()
    {
        countdownGameobject.SetActive(true);
        countdownTMP.text = "3";
        yield return new WaitForSeconds(0.6f);
        countdownTMP.text = "2";
        yield return new WaitForSeconds(0.6f);
        countdownTMP.text = "1";
        yield return new WaitForSeconds(0.6f);
        countdownGameobject.SetActive(false);
    }

    private void OnMinigameFinished(bool didPlayerWon)
    {
        if (didPlayerWon)
        {
            starBurstVFX.SetTrigger("Burst");
            int currentWeight = assignedBlueprint.Weight;
            int newWeight = currentWeight > idealWeight ? currentWeight - 1 : currentWeight + 1;

            if(newWeight == idealWeight)
            {
                DisableMinigame();
            }
            else
            {
                minigamesByWeapon[assignedBlueprint.Weapon].ResetAndRestart();
            }
            assignedBlueprint.SetWeight(newWeight);
            weightScaleDial.SetWeight(newWeight);
            
            assignedBlueprint.PriceTag.Upgrade(20, newWeight == idealWeight);
        }
        else
        { 
            gameLostVFX.Play();
            int currentWeight = assignedBlueprint.Weight;
            int newWeight = currentWeight > idealWeight ? currentWeight + 1 : currentWeight - 1;
            newWeight = Mathf.Clamp(newWeight, 1, 10);
            minigamesByWeapon[assignedBlueprint.Weapon].ResetAndRestart();
            assignedBlueprint.SetWeight(newWeight);
            weightScaleDial.SetWeight(newWeight);
        }
    }

    public void DisableMinigame()
    {
        if(assignedBlueprint == null) return;

        minigamesByWeapon[assignedBlueprint.Weapon].OnGameFinished -= OnMinigameFinished;
        minigamesByWeapon[assignedBlueprint.Weapon].gameObject.SetActive(false);
        // CameraZoomOut();
    }

    private void OnEnable()
    {
        CustomersQueueManager.Instance.OnCustomerChange += OnCustomerChange;
    }

    void OnDisable()
    {
        CustomersQueueManager.Instance.OnCustomerChange -= OnCustomerChange;
    }

    private void OnCustomerChange(Customer customer)
    {
        idealWeight = customer.WeaponRequest.Weight;
        weightScaleDial.SetIdealWeight(customer.WeaponRequest.Weight);
    }
    
    private void CameraZoomIn()
    {
        mainCamera.DOOrthoSize(3.3f, 0.5f).SetEase(Ease.OutBack);
        mainCamera.transform.DOLocalMove(new Vector3(3.5f, -2f, mainCamera.transform.localPosition.z), 0.5f).SetEase(Ease.OutQuad);
    }

    public void CameraZoomOut()
    {
        mainCamera.DOOrthoSize(5.2f, 0.5f).SetEase(Ease.OutBack);
        mainCamera.transform.DOLocalMove(new Vector3(0, 0, mainCamera.transform.localPosition.z), 0.5f).SetEase(Ease.OutQuad);
    }
}
