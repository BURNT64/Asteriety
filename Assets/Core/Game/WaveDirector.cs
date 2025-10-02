using UnityEngine;
using System.Collections;
using TMPro;

[DisallowMultipleComponent]
public class WaveDirector : MonoBehaviour
{
    [Header("Refs")]
    public AsteroidSpawner spawner;       // assign in Inspector
    public TextMeshProUGUI waveLabel;     // optional "Wave X" banner (can be null)

    [Header("Waves")]
    public int startCount = 8;            // wave 1 size
    public int addPerWave = 4;            // added per wave
    public float intermission = 2f;       // seconds after a wave clears

    [Header("Spawn Timing")]
    public float preSpawnDelay = 0.6f;    // delay before first asteroid of each wave
    public float perSpawnInterval = 0.30f;// time between spawns during a wave

    int wave = 0;
    int alive = 0;
    Coroutine bannerRoutine;

    void Start()
    {
        if (!spawner) spawner = FindObjectOfType<AsteroidSpawner>();
        if (spawner == null)
        {
            Debug.LogError("WaveDirector: No AsteroidSpawner found/assigned.");
            enabled = false;
            return;
        }

        // Make sure spawner is not auto-spawning while we control waves
        spawner.runContinuousLoop = false;

        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (true)
        {
            wave++;
            int count = startCount + addPerWave * (wave - 1);
            alive = count;

            ShowWaveBanner(wave);

            // Trickled spawns instead of burst
            spawner.SpawnSequence(count, preSpawnDelay, perSpawnInterval);

            // Wait until all asteroids are gone (ExplodeAndDie or DespawnSilently)
            while (alive > 0) yield return null;

            // Small break before next wave
            yield return new WaitForSeconds(intermission);
        }
    }

    public void OnAsteroidDied()
    {
        alive = Mathf.Max(0, alive - 1);
    }

    void ShowWaveBanner(int n)
    {
        if (!waveLabel) return;

        waveLabel.text = "Wave " + n;
        waveLabel.gameObject.SetActive(true);

        if (bannerRoutine != null) StopCoroutine(bannerRoutine);
        bannerRoutine = StartCoroutine(HideBannerSoon());
    }

    IEnumerator HideBannerSoon()
    {
        yield return new WaitForSeconds(1.2f);
        if (waveLabel) waveLabel.gameObject.SetActive(false);
    }
}
