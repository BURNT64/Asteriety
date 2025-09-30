using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public int maxHP = 3;
    public float speed = 6f;
    [Range(0f, 1.5f)] public float drift = 0.6f;   // tangential variance
    public float cleanupRadius = 30f;               // relative to center

    int hp;
    Vector2 vel;
    Vector2 center;

    void Awake() { hp = maxHP; }

    // spawnPos: where we spawned; arenaCenter: the target point we head towards
    public void Init(Vector2 spawnPos, Vector2 arenaCenter)
    {
        center = arenaCenter;
        transform.position = spawnPos;

        Vector2 toCenter = (center - spawnPos).normalized;
        Vector2 tangent = new Vector2(-toCenter.y, toCenter.x);
        Vector2 dir = (toCenter + tangent * Random.Range(-drift, drift)).normalized;

        vel = dir * speed;
    }

    public void Hit(int damage)
    {
        hp -= damage;
        if (hp <= 0) Die();
    }

    void Update()
    {
        transform.Translate(vel * Time.deltaTime, Space.World);

        // cleanup relative to arena center
        if (((Vector2)transform.position - center).sqrMagnitude > cleanupRadius * cleanupRadius)
            Destroy(gameObject);
    }

    void Die() { Destroy(gameObject); }
}
