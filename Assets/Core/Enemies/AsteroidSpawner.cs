using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Refs")]
    public GameObject asteroidPrefab;
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

    void SpawnOne()
    {
        Vector2 center = centerRef ? (Vector2)centerRef.position : fallbackCenter;

        // consistent angle generation (deterministic if useSeed == true)
        float angle = useSeed ? (float)(rng.NextDouble() * Mathf.PI * 2f)
                              : Random.Range(0f, Mathf.PI * 2f);

        Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        // instantiate at the position and tell the asteroid what to aim at
        var go = Instantiate(asteroidPrefab, pos, Quaternion.identity);
        go.GetComponent<Asteroid>().Init(pos, center);
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
