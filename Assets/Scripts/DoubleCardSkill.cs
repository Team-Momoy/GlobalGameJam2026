using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleCardSkill : Interactable
{
    bool isBurned;

    [SerializeField]
    private LineRenderer lineRenderer, lineRenderer2;

    [SerializeField]
    public bool isSkillReal;

    // Selection state
    private bool selecting = false;
    private readonly List<MaskedNpc> selectedNpcs = new List<MaskedNpc>();
    private readonly Dictionary<MaskedNpc, Vector3> originalScales = new Dictionary<MaskedNpc, Vector3>();

    protected override void Awake()
    {
        base.Awake();

        // Make the onClick start selection mode
        GetComponent<Card>().onClick.AddListener(StartSelection);
    }

    void OnDisable()
    {
        CancelSelection();
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

    // Entry point for selection mode (called on click)
    public void StartSelection()
    {
        if (selecting) return;
        selecting = true;
        selectedNpcs.Clear();
        originalScales.Clear();
        lineRenderer.enabled = true;
        Burn();
        onClick.RemoveAllListeners();
        GetComponent<Card>().ClearOnClickListeners();
    }

    private void CancelSelection()
    {
        selecting = false;
        lineRenderer.enabled = false;
        // revert scales on any selected npcs
        foreach (var kv in originalScales)
        {
            if (kv.Key != null)
                kv.Key.transform.localScale = kv.Value;
        }
        selectedNpcs.Clear();
        originalScales.Clear();
    }

    private void CompleteSelection()
    {
        selecting = false;
        lineRenderer.enabled = false;

        if (selectedNpcs.Count > 0)
        {
            // Call GenerateBatch with the selected NPCs to give them 2 cards each
            print("DoubleCardSkill: completing selection for " + selectedNpcs.Count + " NPCs");
            FindAnyObjectByType<CardManager>().GenerateBatch(selectedNpcs, isSkillReal);
        }

        //Make the lines point 0 be current position and point position 1 be the first and second selected NPCs
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, selectedNpcs[0].CardPlayingPoint.position);
        lineRenderer2.SetPosition(0, transform.position);
        lineRenderer2.SetPosition(1, selectedNpcs[1].CardPlayingPoint.position);

        // revert scales
        foreach (var kv in originalScales)
        {
            if (kv.Key != null)
                kv.Key.transform.localScale = kv.Value;
        }
        selectedNpcs.Clear();
        originalScales.Clear();
    }

    void Update()
    {
        if (!selecting) return;

        // Use the new Input System like Dagger: right click or Escape cancels
        // if ((Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) ||
        //     (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame))
        // {
        //     CancelSelection();
        //     return;
        // }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Raycast into the scene to find a MaskedNpc under the cursor
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 100f))
            {
                var npc = hit.transform.GetComponent<MaskedNpc>();
                if (npc != null && !selectedNpcs.Contains(npc))
                {
                    // record original scale and do a small pop to indicate selection
                    originalScales[npc] = npc.transform.localScale;
                    npc.transform.DOScale(originalScales[npc] * 1.12f, 0.12f).SetEase(Ease.OutBack);

                    selectedNpcs.Add(npc);

                    // if we've got two, complete
                    if (selectedNpcs.Count >= 2)
                    {
                        CompleteSelection();
                    }
                }
            }
        }
    }

    public void MakeDoubleBatch()
    {
        // Backwards compatible shortcut: begin selection mode
        StartSelection();
    }
}
