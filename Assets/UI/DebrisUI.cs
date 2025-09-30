using TMPro;
using UnityEngine;

public class DebrisUI : MonoBehaviour
{
    TextMeshProUGUI label;

    void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        label.text = "Debris: " + GameManager.Debris;
    }
}
