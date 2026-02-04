using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointingSkill : Interactable
{
    bool isBurned;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    public bool isSkillReal;

    // Selection state
    private bool selecting = false;
    private MaskedNpc selectedNpc = null;

    protected override void Awake()
    {
        base.Awake();

        // Make the onClick start selection mode
        GetComponent<Card>().onClick.AddListener(StartSelection);
        
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
        lineRenderer.enabled = true;
        Burn();
        onClick.RemoveAllListeners();
        GetComponent<Card>().ClearOnClickListeners();
    }

    private void CompleteSelection()
    {
        selecting = false;
        lineRenderer.enabled = false;

        CardManager cardManager = FindAnyObjectByType<CardManager>();

        if (selectedNpc != null)
        {
            // Call GenerateBatch with the selected NPC to give them 2 cards each
            print("PointingSkill: completing selection for " + selectedNpc.name);
            //Get the current Card manager batch and if the npc is impostor get an impostor card from the batch if not then get a normal card
            bool foundCard = false;
            cardManager.batches[cardManager.LastBatchIndex].ForEach(card => {
                if(foundCard) return;
                if (isSkillReal)
                {                    
                    if (selectedNpc.IsImpostor && card.IsImpostorCard)
                    {
                        card.MarkAsPlayed();
                        foundCard = true;
                    }
                    else if (!selectedNpc.IsImpostor && !card.IsImpostorCard)
                    {
                        card.MarkAsPlayed();
                        foundCard = true;
                    }
                }
                else
                {
                    if(selectedNpc.IsImpostor && !card.IsImpostorCard)
                    {
                        card.MarkAsPlayed();
                        foundCard = true;
                    }
                    else if (!selectedNpc.IsImpostor && card.IsImpostorCard)
                    {
                        card.MarkAsPlayed();
                        foundCard = true;
                    }
                }
            });
        }

        //Make the lines point 0 be current position and point position 1 be the first and second selected NPCs
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, selectedNpc.CardPlayingPoint.position);
        FindAnyObjectByType<CardManager>().GenerateBatch();
    }

    void Update()
    {
        if (!selecting) return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Raycast into the scene to find a MaskedNpc under the cursor
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out var hit, 100f))
            {
                var npc = hit.transform.GetComponent<MaskedNpc>();
                if (npc != null)
                {
                    selectedNpc = npc;
                    CompleteSelection();
                }
            }
        }
    }
}
