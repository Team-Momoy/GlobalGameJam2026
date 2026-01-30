using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Simple singleton (not thread-safe) - kept for convenience
    public static TimeManager Instance { get; private set; }

    [SerializeField]
    private float initialTime = 60f;
    public float InitialTime => initialTime;

    [SerializeField]
    [Tooltip("If true the timer will start paused. Use Resume() to begin.")]
    private bool startPaused = false;

    private float remainingTime;
    public float RemainingTime => remainingTime;

    // Fraction of time remaining in range [0,1]
    public float RemainingFraction => initialTime > 0f ? Mathf.Clamp01(remainingTime / initialTime) : 0f;

    // Events: subscribers can react to time updates and day end
    public event Action<float> OnTimeChanged; // passes remainingTime
    public event Action OnDayEnded;

    private bool paused;

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

    void Start()
    {
        // Ensure initialTime is sensible
        if (initialTime <= 0f) initialTime = 1f;

        remainingTime = initialTime;
        paused = startPaused;
        OnTimeChanged?.Invoke(remainingTime);
    }

    void Update()
    {
        if (paused) return;

        if (remainingTime <= 0f) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            OnTimeChanged?.Invoke(remainingTime);
            OnDayEnded?.Invoke();
        }
        else
        {
            OnTimeChanged?.Invoke(remainingTime);
        }
    }

    // Controls
    public void Pause() => paused = true;
    public void Resume() => paused = false;
    public bool IsPaused() => paused;

    // Mutators
    public void ResetTimer()
    {
        remainingTime = initialTime;
        OnTimeChanged?.Invoke(remainingTime);
    }

    public void SetInitialTime(float seconds, bool resetRemaining = true)
    {
        initialTime = Mathf.Max(1f, seconds);
        if (resetRemaining) {
            remainingTime = initialTime;
            OnTimeChanged?.Invoke(remainingTime);
        }
    }

    public void AddTime(float amount)
    {
        if (amount <= 0f) return;
        remainingTime = Mathf.Min(remainingTime + amount, initialTime);
        OnTimeChanged?.Invoke(remainingTime);
    }

    public void SubtractTime(float amount)
    {
        if (amount <= 0f) return;
        remainingTime = Mathf.Max(remainingTime - amount, 0f);
        OnTimeChanged?.Invoke(remainingTime);
        if (remainingTime <= 0f) OnDayEnded?.Invoke();
    }
}
