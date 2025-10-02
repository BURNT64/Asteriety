using System.Collections;
using UnityEngine;

public enum FireMode { Auto, Semi }

[DisallowMultipleComponent]
public class WeaponMount : MonoBehaviour
{
    [Header("Refs")]
    public GameObject bulletPrefab;
    public GameObject muzzleFlash; // assign the child MuzzleFlash in inspector

    [Header("Firing")]
    public FireMode fireMode = FireMode.Auto;
    public float fireRate = 6f;

    [Header("Shot Shape")]
    public int projectilesPerShot = 1;
    public float spreadDeg = 0f;

    float cooldown;

    void Update()
    {
        cooldown = Mathf.Max(0f, cooldown - Time.deltaTime);
        float interval = 1f / Mathf.Clamp(fireRate, 0.01f, 50f);

        if (fireMode == FireMode.Semi)
        {
            if (Input.GetMouseButtonDown(0) && cooldown <= 0f)
            {
                FireOneBurst();
                cooldown = interval;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                while (cooldown <= 0f)
                {
                    FireOneBurst();
                    cooldown += interval;
                }
            }
        }
    }

    void FireOneBurst()
    {
        if (!bulletPrefab) return;

        if (projectilesPerShot <= 1 || spreadDeg <= 0.001f)
        {
            SpawnBullet(transform.up);
            return;
        }

        int n = Mathf.Max(1, projectilesPerShot);
        float half = spreadDeg * 0.5f;
        for (int i = 0; i < n; i++)
        {
            float t = (n == 1) ? 0.5f : (float)i / (n - 1);
            float angle = Mathf.Lerp(-half, half, t);
            SpawnBullet(RotateDeg(transform.up, angle));
        }
    }

    void SpawnBullet(Vector2 dir)
    {
        var go = Instantiate(bulletPrefab, transform.position, transform.rotation);
        var b = go.GetComponent<Bullet>();
        if (b != null) b.Init(dir); // damage comes from the prefab now

        if (muzzleFlash != null)
            StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.03f);
        muzzleFlash.SetActive(false);
    }

    static Vector2 RotateDeg(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}
