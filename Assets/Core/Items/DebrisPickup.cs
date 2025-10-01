using UnityEngine;

public class DebrisPickup : MonoBehaviour
{
    [Header("General")]
    public int amount = 1;
    public float lifetime = 8f;

    [Header("Homing")]
    public float delayBeforeHoming = 1.0f;   // wait before homing so it's readable
    public float startSpeed = 0f;            // initial speed when homing starts
    public float accel = 30f;                // acceleration while homing
    public float maxSpeed = 20f;             // cap so it doesn’t teleport
    public float snapDistance = 0.2f;        // auto-collect when within this distance

    Transform player;
    float t;
    float curSpeed;

    void Awake()
    {
        // Try to find the player once (requires player tagged "Player")
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj) player = playerObj.transform;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
        curSpeed = startSpeed;
    }

    void Update()
    {
        t += Time.deltaTime;

        // If no player found, just wait out lifetime
        if (!player) return;

        if (t < delayBeforeHoming)
        {
            // optional: tiny bob or spin could go here for readability
            return;
        }

        // Home toward player after delay
        Vector3 toPlayer = (player.position - transform.position);
        float dist = toPlayer.magnitude;

        if (dist <= snapDistance)
        {
            Collect();
            return;
        }

        Vector3 dir = toPlayer / dist; // normalized
        curSpeed = Mathf.Min(maxSpeed, curSpeed + accel * Time.deltaTime);
        transform.position += dir * curSpeed * Time.deltaTime;
    }

    void Collect()
    {
        GameManager.AddDebris(amount);
        Destroy(gameObject);
    }

    // If you still keep a trigger collider on the pickup, this will collect on touch too.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Collect();
    }
}
