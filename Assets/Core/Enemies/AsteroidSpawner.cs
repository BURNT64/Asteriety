using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Refs")]
    public GameObject[] asteroidPrefabs;            // assign variants here
    public Transform centerRef;                     // optional; else uses fallbackCenter
    public Vector2 fallbackCenter = new Vector2(55f, -48f);

    [Header("Spawn Shape")]
    public float spawnRadius = 12f;                 // ring distance from center
    public float spawnEvery = 1.0f;                 // used only if runContinuousLoop = true
    public int burstCount = 1;                      // used only if runContinuousLoop = true

    [Header("Mode")]
    public bool runContinuousLoop = false;          // FALSE when WaveDirector controls spawns

    [Header("Determinism (optional)")]
    public bool useSeed = true;
    public int sessionSeed = 12345;

    System.Random rng;
    Coroutine activeSequence;

    void Awake()
    {
        if (useSeed)
        {
            rng = new System.Random(sessionSeed);
            Random.InitState(sessionSeed);
        }
    }

    void Start()
    {
        if (runContinuousLoop)
            StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnEvery);
        while (true)
        {
            for (int i = 0; i < burstCount; i++) SpawnOne();
            yield return wait;
        }
    }

    // Instant burst (kept for utility, e.g., debug)
    public void SpawnBurst(int count)
    {
        for (int i = 0; i < count; i++) SpawnOne();
    }

    // NEW: spawn a wave over time instead of all-at-once
    public Coroutine SpawnSequence(int count, float firstDelay, float perSpawnInterval)
    {
        if (activeSequence != null) StopCoroutine(activeSequence);
        activeSequence = StartCoroutine(SpawnSequenceCo(count, firstDelay, perSpawnInterval));
        return activeSequence;
    }

    IEnumerator SpawnSequenceCo(int count, float firstDelay, float perSpawnInterval)
    {
        if (firstDelay > 0f) yield return new WaitForSeconds(firstDelay);

        for (int i = 0; i < count; i++)
        {
            SpawnOne();
            if (perSpawnInterval > 0f)
                yield return new WaitForSeconds(perSpawnInterval);
            else
                yield return null; // at least yield a frame
        }

        activeSequence = null;
    }

    void SpawnOne()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogWarning("AsteroidSpawner: no asteroidPrefabs assigned.");
            return;
        }

        Vector2 center = centerRef ? (Vector2)centerRef.position : fallbackCenter;

        float angle = (useSeed && rng != null)
            ? (float)(rng.NextDouble() * Mathf.PI * 2f)
            : Random.Range(0f, Mathf.PI * 2f);

        Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        int idx = (useSeed && rng != null)
            ? rng.Next(asteroidPrefabs.Length)
            : Random.Range(0, asteroidPrefabs.Length);

        var prefab = asteroidPrefabs[idx];
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var a = go.GetComponent<Asteroid>();
        if (a != null) a.Init(pos, center);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector2 center = centerRef ? (Vector2)centerRef.position : fallbackCenter;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, spawnRadius);
        Gizmos.DrawSphere(center, 0.15f);
    }
#endif
}
