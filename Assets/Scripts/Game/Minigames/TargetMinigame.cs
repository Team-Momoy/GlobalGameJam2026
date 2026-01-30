using System.Collections;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetMinigame : Minigame
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private InputAction inputAction;

    private static readonly WaitForSeconds _waitForSeconds0_75 = new(0.75f);

    [SerializeField]
    [MinMaxSlider(-10, 10, true)]
    private Vector2 XRangeMinMax = new Vector2(0, 0);

    [SerializeField]
    [MinMaxSlider(-10, 10, true)]
    private Vector2 YRangeMinMax = new Vector2(0, 0);

    [SerializeField]
    private GameObject targetPrefab;

    private Vector2 randomPosition = new();

    [SerializeField]
    private AudioClip comboSFX;

    [SerializeField]
    private int totalTargets = 4;

    [SerializeField]
    private float targetSpawnDelay = 0.75f;

    private int combo;

    private int targetsHit;
    
    // Track total targets spawned to limit spawning
    private int targetsSpawned = 0;
    
    private bool hasLost = false;
    
    private Transform targetsContainer;

    void OnEnable()
    {
        inputAction.Enable();
        inputAction.performed += CheckHit;
        TargetPrefab.OnTargetMissed += HandleTargetMissed;
        Reset();
        
        // Create container for all spawned targets
        GameObject container = new GameObject("TargetsContainer");
        container.transform.SetParent(transform);
        targetsContainer = container.transform;
        
        playCoroutine = StartCoroutine(Play());
    }

    public override void Reset()
    {
        if(playCoroutine != null) StopCoroutine(playCoroutine);
        combo = 0;
        targetsHit = 0;
        targetsSpawned = 0;
        hasLost = false;
    }

    private void OnDisable()
    {
        inputAction.performed -= CheckHit;
        TargetPrefab.OnTargetMissed -= HandleTargetMissed;
        inputAction.Disable();
        
        if(playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
        }
        
        if(targetsContainer != null)
        {
            Destroy(targetsContainer.gameObject);
        }
    }

    public override IEnumerator Play()
    {
        yield return new WaitForSeconds(2f);
        // Only spawn totalTargets amount of targets
        while (targetsSpawned < totalTargets && !hasLost)
        {
            Vector3 newRandomPosition = GetRandomPosition();
            while(Vector2.Distance(randomPosition, newRandomPosition) < 1.5f)
            {
                newRandomPosition = GetRandomPosition();
            }

            randomPosition = newRandomPosition;
            
            GameObject targetInstance = Instantiate(targetPrefab, transform.position + newRandomPosition, Quaternion.identity);
            targetInstance.transform.SetParent(targetsContainer);
            targetsSpawned++;
            
            yield return new WaitForSeconds(targetSpawnDelay);
        }
        
        // Win condition: all targets spawned and all targets hit correctly
        if(!hasLost && targetsHit == totalTargets)
        {
            OnGameFinished?.Invoke(true);
        }
    }

    private void CheckHit(InputAction.CallbackContext context)
    {
        // Read mouse position from the new Input System
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();

        // Convert screen position to world position
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Raycast at that position (point raycast)
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            mouseWorldPos,
            Vector2.zero
        );

        TargetPrefab target = null;
        float distanceToTarget = 10;
        foreach (RaycastHit2D hit in hits)
        {
            if (!hit.collider.TryGetComponent(out TargetPrefab currentTarget)) continue;
            float distance = Vector2.Distance(hit.collider.transform.position, mouseWorldPos);
            if(distance < distanceToTarget)
            {
                target = currentTarget;
                distanceToTarget = distance;
            }
        }
        if(target != null)
        {
            if (target.Hit())
            {
                targetsHit++;
                combo++;
                MMSoundManager.Instance.PlaySound(comboSFX, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero, false, 1, 0, false, 0, 1, null, false, null, null, 1.2f + (0.1f * combo));
                
                // Check if player won
                if(targetsHit == totalTargets)
                {
                    OnGameFinished?.Invoke(true);
                }
            }
            else
            {
                // Failed to hit target in time - lose
                LoseGame();
            }
        }
    }

    private void HandleTargetMissed()
    {
        // Target was not clicked and disappeared - lose
        if (!hasLost)
        {
            LoseGame();
        }
    }

    private void LoseGame()
    {
        hasLost = true;
        combo = 0;
        
        // Destroy all remaining targets
        foreach(Transform child in targetsContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Invoke lose
        OnGameFinished?.Invoke(false);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(XRangeMinMax.x, XRangeMinMax.y), Random.Range(YRangeMinMax.x, YRangeMinMax.y));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        // Draw spawn area bounds
        Vector3 min = transform.position + new Vector3(XRangeMinMax.x, YRangeMinMax.x);
        Vector3 max = transform.position + new Vector3(XRangeMinMax.y, YRangeMinMax.y);

        Vector3 center = (min + max) / 2f;
        Vector3 size = max - min;
        size.z = 0.01f;

        Gizmos.DrawWireCube(center, size);
    }

}
