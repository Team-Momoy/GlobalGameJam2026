using System;
using System.Collections;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public Action<bool> OnGameFinished;

    protected Coroutine playCoroutine;

    public virtual void ResetAndRestart()
    {
        Reset();
        playCoroutine = StartCoroutine(Play());
    }

    public virtual void Reset()
    {
        
    }

    public virtual IEnumerator Play()
    {
        yield return null;
    }
}
