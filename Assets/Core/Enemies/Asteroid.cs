using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public int maxHP = 3;
    public float speed = 6f;
    [Range(0f, 1.5f)] public float drift = 0.6f;
    public float cleanupRadius = 30f;

    [Header("Combat")]
    public int contactDamage = 1;

    [Header("Drops & VFX")]
    public GameObject debrisPickupPrefab;
    public GameObject explosionPrefab;

    int hp;
    Vector2 vel;
    Vector2 center;
    bool dying;

    void Awake() { hp = maxHP; }

    public void Init(Vector2 spawnPos, Vector2 arenaCenter)
    {
        center = arenaCenter;
        transform.position = spawnPos;
        Vector2 toCenter = (center - spawnPos).normalized;
        Vector2 tangent = new Vector2(-toCenter.y, toCenter.x);
        float side = Mathf.Clamp(Random.Range(-drift, drift), -0.3f, 0.3f);
        Vector2 dir = (toCenter + tangent * side).normalized;
        vel = dir * speed;
    }

    public void Hit(int damage)
    {
        if (dying) return;
        hp -= damage;
        if (hp <= 0) ExplodeAndDie();
    }

    void Update()
    {
        if (dying) return;
        transform.Translate(vel * Time.deltaTime, Space.World);

        // IMPORTANT: if it drifts past cleanup, notify WaveDirector and despawn
        if (((Vector2)transform.position - center).sqrMagnitude > cleanupRadius * cleanupRadius)
            DespawnSilently();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (dying) return;

        if (other.CompareTag("Player"))
        {
            var hpComp = other.GetComponent<PlayerHealth>();
            if (hpComp != null) hpComp.TakeHit(contactDamage);
            ExplodeAndDie();
        }
    }

    void ExplodeAndDie()
    {
        if (dying) return;
        dying = true;

        // camera shake
        FindObjectOfType<CameraShake>()?.Shake(0.08f, 0.10f);

        // VFX
        if (explosionPrefab)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Drops
        if (debrisPickupPrefab != null)
        {
            int count = Random.Range(1, 3); // 1–2
            for (int i = 0; i < count; i++)
            {
                Vector2 off = Random.insideUnitCircle * 0.3f;
                Instantiate(debrisPickupPrefab, transform.position + (Vector3)off, Quaternion.identity);
            }
        }

        // notify wave director on death
        var dir = FindObjectOfType<WaveDirector>();
        if (dir) dir.OnAsteroidDied();

        Destroy(gameObject);
    }

    // NEW: cleanup path that still notifies WaveDirector but skips VFX/drops
    void DespawnSilently()
    {
        if (dying) return;
        dying = true;

        var dir = FindObjectOfType<WaveDirector>();
        if (dir) dir.OnAsteroidDied();

        Destroy(gameObject);
    }
}
