using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 720f; // degrees per second

    void Update()
    {
        // get mouse world position
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // direction from ship to mouse
        Vector2 direction = mouseWorld - transform.position;

        // calculate target angle (pointing UP = forward)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // rotate smoothly toward target
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, 0, targetAngle),
            rotateSpeed * Time.deltaTime
        );
    }
}
