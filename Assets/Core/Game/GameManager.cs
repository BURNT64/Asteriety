using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int Debris;

    void Awake()
    {
        Debris = 0; // reset at start of game
    }

    public static void AddDebris(int amount)
    {
        Debris += amount;
        Debug.Log("Debris: " + Debris);
        // later: update HUD here instead of Debug.Log
    }
}
