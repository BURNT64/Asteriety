using UnityEngine;

public class WeaponMount : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 6f;
    float nextShot;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextShot)
        {
            nextShot = Time.time + 1f / fireRate;
            var go = Instantiate(bulletPrefab, transform.position, transform.rotation);
            go.GetComponent<Bullet>().Init(transform.up);
        }
    }
}
