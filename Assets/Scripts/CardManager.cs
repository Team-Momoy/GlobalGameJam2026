using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using DG.Tweening;

public class CardManager : MonoBehaviour
{
    private bool isDealing = false;
    public bool IsDealing {
        get => isDealing;
        set => isDealing = value;
    }

    [SerializeField] private Transform gatherPoint;
    [SerializeField] private float gatherDuration = 0.8f;
    [SerializeField] private float gatherDelay = 0.5f;
    [SerializeField] private float rotateDuration = 0.5f;

    private readonly List<Card> cards = new List<Card>();

    [SerializeField] private float batchZSpacing = 5f;
    private int batchCount = 0;
    private int lastProcessedIndex = -1;
    private int lastBatchStartIndex = 0;

    // New per-batch storage
    public readonly List<List<Card>> batches = new List<List<Card>>();
    private int lastBatchIndex = -1;
    public int LastBatchIndex => lastBatchIndex;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        GenerateBatch();
        CreateFakeSkill();
    }

    private void CreateFakeSkill()
    {
        int randomInt = Random.Range(0, 3);
        switch (randomInt)
        {
            case 0:
                FindFirstObjectByType<RevealSkill>().isSkillReal = false;
                break;
            case 1:
                FindFirstObjectByType<DoubleCardSkill>().isSkillReal = false;
                break;
            case 2:
                FindFirstObjectByType<PointingSkill>().isSkillReal = false;
                break;
            default:
                break;
        }
    }

    private void MoveAllCardsToPoint(Vector3 point)
    {
        // Backwards-compatible: move all cards as a single batch at z=0
        MoveNewCardsToPoint(point, 0, 0f);
    }

    // Main batch API: if `extraPlayers` is provided, those NPCs will play 2 cards each; everyone else plays 1 card.
    // `extraPlayers` is optional (can be null) and this is the single public method to use.
    public void GenerateBatch(List<MaskedNpc> extraPlayers = null, bool isReal = true)
    {
        StartCoroutine(GenerateBatchCoroutine(extraPlayers, isReal));
    }

    private IEnumerator GenerateBatchCoroutine(List<MaskedNpc> extraPlayers, bool isReal = true)
    {
        isDealing = true;
        List<MaskedNpc> extraNpcs = new List<MaskedNpc>(extraPlayers ?? new List<MaskedNpc>());
        // small delay to let animations settle
        yield return new WaitForSeconds(0.2f);

        var allNpcs = FindObjectsByType<MaskedNpc>(FindObjectsSortMode.None);
        if (allNpcs == null || allNpcs.Length == 0)
        {
            Debug.Log("GenerateBatch: no NPCs found");
            yield break;
        }

        Debug.Log($"GenerateBatch called. total NPCs found: {allNpcs.Length}. extraPlayers provided: {(extraNpcs == null ? 0 : extraNpcs.Count)}");

        // Build a lookup for faster checks
        HashSet<MaskedNpc> extras = null;
        if (extraNpcs != null && extraNpcs.Count > 0)
        {
            print("CardManager: GenerateBatch with extra players");
            extras = new HashSet<MaskedNpc>(extraNpcs);
            string names = "";
            foreach (var e in extraNpcs) names += (e != null ? e.name : "null") + ", ";
            Debug.Log($"GenerateBatch extras: {names}");
        }

        int totalPlays = 0;
        foreach (var npc in allNpcs)
        {
            int planned = (extras != null && extras.Contains(npc)) ? 2 : 1;
            totalPlays += planned;
            Debug.Log($"NPC '{npc.name}' scheduled plays: {planned}");
        }

        Debug.Log($"GenerateBatch: totalPlays = {totalPlays}");

        int remaining = totalPlays;
        int startIndex = cards.Count;

        foreach (var npc in allNpcs)
        {
            int plays = (extras != null && extras.Contains(npc)) ? 2 : 1;
            for (int p = 0; p < plays; p++)
            {
                print(plays);
                var (card, sequence) = npc.PlayCard(p <= 0 || isReal);
                if (card != null)
                {
                    cards.Add(card);
                    Debug.Log($"Spawned card for NPC '{npc.name}'. cards.Count = {cards.Count}");
                }

                if (sequence != null)
                {
                    sequence.OnComplete(() =>
                    {
                        remaining--;
                        Debug.Log($"Sequence completed. Remaining plays: {remaining}");
                        if (remaining == 0)
                        {
                            Debug.Log($"All plays complete. Creating batch from startIndex {startIndex}");
                            CreateAndMoveBatchFromRange(startIndex);
                        }
                    });
                }
                else
                {
                    remaining--;
                    Debug.Log($"No sequence for NPC '{npc.name}', decremented remaining to {remaining}");
                }
            }
        }

        // In case everything finished synchronously
        if (remaining == 0)
        {
            Debug.Log($"All plays completed synchronously. Creating batch from startIndex {startIndex}");
            CreateAndMoveBatchFromRange(startIndex);
        }

        yield return null;
    }

    private void MoveNewCardsToPoint(Vector3 point, int startIndex, float zOffset)
    {
        // Backwards compatible: build batch from startIndex and call MoveBatchToPoint
        var batch = new List<Card>();
        for (int i = startIndex; i < cards.Count; i++)
        {
            if (cards[i] != null) batch.Add(cards[i]);
        }

        if (batch.Count > 0)
        {
            batches.Add(batch);
            lastBatchIndex = batches.Count - 1;
            batchCount = batches.Count;
            MoveBatchToPoint(point, batch, zOffset);
        }
    }

    // Helper: create a batch from cards starting at startIndex and move it
    private void CreateAndMoveBatchFromRange(int startIndex)
    {
        var batch = new List<Card>();
        for (int k = startIndex; k < cards.Count; k++)
        {
            if (cards[k] != null) batch.Add(cards[k]);
        }

        Debug.Log($"CreateAndMoveBatchFromRange: startIndex={startIndex}, newCardsFound={batch.Count}");

        if (batch.Count == 0) return;

        Shuffle(batch);
        batches.Add(batch);
        lastBatchIndex = batches.Count - 1;

        float zOffset = lastBatchIndex * batchZSpacing;
        MoveBatchToPoint(gatherPoint ? gatherPoint.position : Vector3.zero, batch, zOffset);
        batchCount = batches.Count;
        lastProcessedIndex = cards.Count - 1;

        Debug.Log($"Batch {lastBatchIndex} created: size={batch.Count}, zOffset={zOffset}");
    }

    private void MoveBatchToPoint(Vector3 point, List<Card> batch, float zOffset)
    {
        Sequence seq = DOTween.Sequence();
        bool hasTweens = false;

        // Track indices via batch index
        lastBatchStartIndex = -1; // no longer using global indices
        lastProcessedIndex = -1;

        for (int idx = 0; idx < batch.Count; idx++)
        {
            var card = batch[idx];
            if (card == null) continue;
            card.transform.SetParent(gatherPoint);

            var move = card.transform.DOMove(point + new Vector3(0, idx * 0.02f, zOffset), gatherDuration).SetDelay(gatherDelay).SetEase(Ease.InOutSine);
            var rot = card.transform.DORotateQuaternion(Quaternion.Euler(-90f, 0f, 180f), gatherDuration).SetDelay(gatherDelay).SetEase(Ease.InOutSine);

            seq.Join(move);
            seq.Join(rot);

            hasTweens = true;
        }

        if (hasTweens)
        {
            seq.OnComplete(() =>
            {
                RotateGatherPoint(rotateDuration);
            });
        }
        else
        {
            // No cards, just flip immediately
            RotateGatherPoint(rotateDuration);
        }
    }

    // Make a function that rotates the gather point around the Z axis
    public void RotateGatherPoint(float duration)
    {
        float spacing = 4f;

        // Precompute the local rotation needed so global ends at (-90,0,180)
        Quaternion desiredGlobal = Quaternion.Euler(90f, 0f, 0f);
        Quaternion inverseParent = Quaternion.Inverse(gatherPoint.rotation);
        Quaternion desiredLocal = inverseParent * desiredGlobal;

        // Center only the most recent batch (if present)
        if (lastBatchIndex >= 0 && lastBatchIndex < batches.Count)
        {
            var batch = batches[lastBatchIndex];
            int validCount = 0;
            foreach (var bc in batch) if (bc != null) validCount++;

            if (validCount > 0)
            {
                float startOffset = -((validCount - 1) / 2f) * spacing; // center the row

                int idx = 0;
                for (int b = 0; b < batch.Count; b++)
                {
                    var card = batch[b];
                    if (card == null) { continue; }

                    float targetX = gatherPoint.position.x + startOffset + (idx * spacing);
                    card.transform.DOMoveX(targetX, 0.5f);

                    // Apply local rotation so after parent's rotation the card's global rotation is correct
                    card.transform.DOLocalRotateQuaternion(desiredLocal, 0.15f).SetEase(Ease.InOutSine);

                    idx++;
                }
            }
        }

        // Reapply desired local rotation to all existing cards so previous batches keep their global rotation
        for (int i = 0; i < cards.Count; i++)
        {
            var c = cards[i];
            if (c == null) continue;
            c.transform.DOLocalRotateQuaternion(desiredLocal, 0.15f).SetEase(Ease.InOutSine);
        }
        isDealing = false;
    }

    // Fisher–Yates shuffle for batches
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
