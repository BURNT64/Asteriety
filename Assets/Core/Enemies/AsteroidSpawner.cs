using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Refs")]
    public GameObject[] asteroidPrefabs;            // assign variants here
    public Transform centerRef;                     // assign WorldCenter
    public Vector2 fallbackCenter = new Vector2(55f, -48f);

    [Header("Spawn Shape")]
    public float spawnRadius = 12f;                 // ring distance from center
    public float spawnEvery = 1.0f;                 // seconds between spawns
    public int burstCount = 1;                      // how many per tick

    [Header("Determinism (optional)")]
    public bool useSeed = true;
    public int sessionSeed = 12345;

    System.Random rng;

    void Start()
    {
        if (useSeed)
        {
            rng = new System.Random(sessionSeed);
            Random.InitState(sessionSeed);
        }
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

    public void SpawnBurst(int count)
    {
        for (int i = 0; i < count; i++) SpawnOne();
    }

    void SpawnOne()
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        Vector2 center = centerRef ? (Vector2)centerRef.position : fallbackCenter;

        // deterministic or random angle
        float angle = useSeed ? (float)(rng.NextDouble() * Mathf.PI * 2f)
                              : Random.Range(0f, Mathf.PI * 2f);

        Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        // pick a random variant from the array
        int idx = useSeed ? rng.Next(asteroidPrefabs.Length) : Random.Range(0, asteroidPrefabs.Length);
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
