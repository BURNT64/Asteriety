using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 3;

    [Header("UI")]
    public TextMeshProUGUI hpLabel;
    public GameObject gameOverPanel;

    [Header("Hit Settings")]
    public float hitCooldown = 0.2f;
    public float flashTime = 0.08f;

    int hp;
    float cd;
    SpriteRenderer sr;
    Color srOrig;

    void Awake()
    {
        hp = Mathf.Max(1, maxHP);
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) srOrig = sr.color;
        UpdateLabel();
    }

    void Update()
    {
        if (cd > 0f) cd -= Time.unscaledDeltaTime;
    }

    public void TakeHit(int dmg)
    {
        if (cd > 0f) return;
        hp = Mathf.Max(0, hp - dmg);
        UpdateLabel();
        HitFlash();

        // Shake on hit
        FindObjectOfType<CameraShake>()?.Shake(0.12f, 0.15f);

        cd = hitCooldown;
        if (hp <= 0) GameOver();
    }

    public void Heal(int amt)
    {
        hp = Mathf.Min(maxHP, hp + amt);
        UpdateLabel();
    }

    void UpdateLabel()
    {
        if (hpLabel != null) hpLabel.text = "HP: " + hp;
    }

    void HitFlash()
    {
        if (sr == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    System.Collections.IEnumerator FlashRoutine()
    {
        sr.color = Color.white;
        yield return new WaitForSeconds(flashTime);
        sr.color = srOrig;
    }

    void GameOver()
    {
        Time.timeScale = 0f;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
}
