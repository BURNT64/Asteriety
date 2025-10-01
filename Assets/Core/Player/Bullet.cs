using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 2f;
    public int damage = 1;   // set by WeaponMount

    Vector2 dir;

    public void Init(Vector2 direction)
    {
        dir = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var asteroid = other.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            asteroid.Hit(damage);
            Destroy(gameObject);
        }
    }
}
