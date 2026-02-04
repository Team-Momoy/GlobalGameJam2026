using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;

public class RevealSkill : Interactable
{
    bool isBurned;

    [SerializeField]
    private Transform spotlight;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    public bool isSkillReal;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Card>().onClick.AddListener(TriggerSkill);
    }

    public void Burn()
    {
        if (isBurned) return;
        isBurned = true;

        //Dotween the value from -0.1 to 1
        DOTween.To(() => -0.1f, v =>
        {
            SetBurn(v);
        }, 1f, 0.5f);
    }
    
    private void SetBurn(float value)
    {
        SetFloat("_FadeAmount", value);
    }

    public void TriggerSkill()
    {
        onClick.RemoveAllListeners();
        GetComponent<Card>().ClearOnClickListeners();
        Burn();

        if (isSkillReal)
        {
            RevealImpostor();
        }
        else
        {
            FakeReveal();
        }
    }

    public void RevealImpostor()
    {
        MaskedNpc impostor = null;
        FindObjectsByType<MaskedNpc>(FindObjectsSortMode.None).ForEach(npc => {
            if (npc.IsImpostor)
            {
                impostor = npc;
            }
        });
        SelectTarget(impostor);
    }

    public void FakeReveal()
    {
        MaskedNpc innocent = null;
        FindObjectsByType<MaskedNpc>(FindObjectsSortMode.None).ForEach(npc => {
            if (!npc.IsImpostor)
            {
                innocent = npc;
            }
        });
        SelectTarget(innocent);
    }

    public void SelectTarget(MaskedNpc target)
    {
        spotlight.SetParent(null);
        spotlight.DOScale(3, 0.2f).SetEase(Ease.OutSine);
        spotlight.DOMove(target.CardPlayingPoint.position, 1f).SetEase(Ease.InOutSine);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.CardPlayingPoint.position);
        FindAnyObjectByType<CardManager>().GenerateBatch();
    }
}
