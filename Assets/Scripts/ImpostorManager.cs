using System.Collections.Generic;
using UnityEngine;

public class ImpostorManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] maskSprites, brokenMasksSprites;

    void Awake()
    {
        MaskedNpc[] npcs = FindObjectsByType<MaskedNpc>(FindObjectsSortMode.None);
        int impostorIndex = Random.Range(0, npcs.Length);
        for (int i = 0; i < npcs.Length; i++)
        {
            if (i == impostorIndex)
            {
                npcs[i].IsImpostor = true;
            }
            else
            {
                npcs[i].IsImpostor = false;
            }
        }       
        RandomizeNpcs();
    }

    public void RandomizeNpcs()
    {
        MaskedNpc[] npcs = FindObjectsByType<MaskedNpc>(FindObjectsSortMode.None);

        // Safety check
        if (maskSprites.Length < npcs.Length)
        {
            Debug.LogError("Not enough unique masks for all NPCs!");
            return;
        }

        // Create a list of indices
        List<int> indices = new List<int>();
        for (int i = 0; i < maskSprites.Length; i++)
            indices.Add(i);

        // Shuffle indices
        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = Random.Range(i, indices.Count);
            (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
        }

        // Assign unique masks
        for (int i = 0; i < npcs.Length; i++)
        {
            int index = indices[i];

            npcs[i].maskRenderer.sprite = maskSprites[index];
            npcs[i].maskDestroyedSprite = brokenMasksSprites[index];
        }
    }
}
