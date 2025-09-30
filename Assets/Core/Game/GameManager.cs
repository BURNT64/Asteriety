using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int Debris;

    void Update()
    {
        // Temporary: add debris when pressing Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debris++;
            Debug.Log("Debris: " + Debris);
        }
    }
}
