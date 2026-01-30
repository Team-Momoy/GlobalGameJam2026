using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HammerMinigame : Minigame, IPointerDownHandler
{
    [SerializeField] private Transform indicatorTransform;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float indicatorSpeed = 500f;
    [SerializeField] private float winRange = 50f;
    [SerializeField] private float minX = -200f;
    [SerializeField] private float maxX = 200f;
    private float targetX;
    private float indicatorX;
    private float indicatorDirection = 1f;
    private bool gameActive = false;
    private bool allowMovement = false;

    private Coroutine indicatorCoroutine;

    public override void Reset()
    {
        // Stop any running coroutines
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        // Set random target position between min and max
        targetX = Random.Range(minX, maxX);
        
        // Position the target
        Vector3 targetPos = targetTransform.localPosition;
        targetPos.x = targetX;
        targetTransform.localPosition = targetPos;
        
        // Start indicator at the middle position
        indicatorX = (minX + maxX) / 2f;
        Vector3 indicatorPos = indicatorTransform.localPosition;
        indicatorPos.x = indicatorX;
        indicatorTransform.localPosition = indicatorPos;
        indicatorDirection = 1f;
        
        gameActive = false;
    }

    void Awake()
    {
        // Don't start automatically
    }

    private void OnEnable()
    {
        ResetAndRestart();
    }

    public override IEnumerator Play()
    {
        gameActive = true;
        allowMovement = true;
        
        while (gameActive && allowMovement)
        {
            // Move the indicator back and forth
            indicatorX += indicatorDirection * indicatorSpeed * Time.deltaTime;
            
            // Bounce at the edges
            if (indicatorX >= maxX)
            {
                indicatorX = maxX;
                indicatorDirection = -1f;
            }
            else if (indicatorX <= minX)
            {
                indicatorX = minX;
                indicatorDirection = 1f;
            }
            
            // Update indicator position
            Vector3 indicatorPos = indicatorTransform.localPosition;
            indicatorPos.x = indicatorX;
            indicatorTransform.localPosition = indicatorPos;
            
            yield return null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!gameActive)
            return;

        // Immediately stop the coroutine completely
        allowMovement = false;
        gameActive = false;
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        
        // Check if indicator is close enough to target
        float distance = Mathf.Abs(indicatorX - targetX);
        bool won = distance <= winRange;
        
        
        // Wait 0.5 seconds before allowing restart
        StartCoroutine(RestartDelay(won));
    }

    private IEnumerator RestartDelay(bool won)
    {
        yield return new WaitForSeconds(0.5f);

        OnGameFinished?.Invoke(won);
    }

    private void OnDrawGizmosSelected()
    {
        if (targetTransform == null || indicatorTransform == null)
            return;

        // Get the parent transform to properly position gizmos
        Transform parent = indicatorTransform.parent ?? indicatorTransform;
        Vector3 parentPos = parent.position;

        Vector3 targetPos = targetTransform.position;
        Vector3 indicatorPos = indicatorTransform.position;

        // Draw target position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPos, 0.2f);

        // Draw win range around target
        Gizmos.color = new Color(0, 1, 0, 1f);
        Gizmos.DrawWireCube(targetPos, new Vector3(winRange * 2, 0.5f, 0.5f));

        // Draw indicator position
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(indicatorPos, 0.15f);

        // Draw min and max bounds
        Gizmos.color = Color.yellow;
        float centerX = (minX + maxX) / 2f;
        Vector3 minPos = parentPos;
        minPos.x += minX;
        minPos.y = indicatorPos.y;
        minPos.z = indicatorPos.z;
        Vector3 maxPos = parentPos;
        maxPos.x += maxX;
        maxPos.y = indicatorPos.y;
        maxPos.z = indicatorPos.z;
        Gizmos.DrawLine(minPos, maxPos);
        
        // Draw center point
        Gizmos.color = Color.cyan;
        Vector3 centerPos = parentPos;
        centerPos.x += centerX;
        centerPos.y = indicatorPos.y;
        centerPos.z = indicatorPos.z;
        Gizmos.DrawSphere(centerPos, 0.1f);
    }
}
