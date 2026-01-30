using Sirenix.OdinInspector;
using UnityEngine;

[InlineEditor]
[CreateAssetMenu(fileName = "CustomerSpawnData", menuName = "Queue/CustomerSpawnData")]
public class CustomerSpawnDataSO : ScriptableObject
{
    [Tooltip("Cada cuanto se revisa el grafico spawneando gente")]
    public float SpawnInterval = 10f;

    [Tooltip("Si vienen muchos juntos, cuando delay entre ellos")]
    public float SpawnDelay = 3f;

    public AnimationCurve CustomerFrequency;
}
