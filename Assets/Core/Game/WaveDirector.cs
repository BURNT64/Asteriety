using UnityEngine;
using System.Collections;
using TMPro;

[DisallowMultipleComponent]
public class WaveDirector : MonoBehaviour
{
    [Header("Refs")]
    public AsteroidSpawner spawner;           // assign in Inspector
    public TextMeshProUGUI waveLabel;         // optional "Wave X" banner

    [Header("Waves")]
    public int startCount = 8;                // wave 1 size
    public int addPerWave = 4;                // + per wave
    public float intermission = 1.0f;         // tiny buffer between waves

    [Header("Spawn Timing")]
    public float preSpawnDelay = 0.6f;        // delay before first spawn
    public float perSpawnInterval = 0.30f;    // spacing between spawns

    [Header("Post-Wave Flow")]
    [Tooltip("Max seconds to wait for debris to be collected before shop opens")]
    public float debrisWaitTimeout = 2.0f;    // scaled time (debris magnets keep working)

    int wave = 0;
    int alive = 0;
    Coroutine bannerRoutine;

    void Start()
    {
        if (!spawner) spawner = FindObjectOfType<AsteroidSpawner>();
        if (!spawner)
        {
            Debug.LogError("WaveDirector: No AsteroidSpawner found/assigned.");
            enabled = false;
            return;
        }

        // We control spawns wave-by-wave
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

            // Trickled spawns for readability
            spawner.SpawnSequence(count, preSpawnDelay, perSpawnInterval);

            // Wait until all asteroids have died/despawned
            while (alive > 0) yield return null;

            // Immediately lock weapon input so clicks after last kill don't fire/buy
            SetAllWeaponInput(enabled: false);

            // Wait for debris to be collected, but don't hang forever
            yield return WaitForDebrisOrTimeout(debrisWaitTimeout);

            // Open shop if available; it will pause time internally
            if (UpgradeManager.Instance != null)
            {
                UpgradeManager.Instance.OpenShop();
                while (UpgradeManager.Instance.IsOpen) yield return null;
            }
            else
            {
                // Fallback tiny pause if no shop present
                yield return new WaitForSeconds(intermission);
            }

            // Re-enable firing for the next wave
            SetAllWeaponInput(enabled: true);

            // Small breather before next wave spawns
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

    IEnumerator WaitForDebrisOrTimeout(float timeout)
    {
        float t = 0f;
        while (t < timeout)
        {
            // If no debris pickups remain, we're done
            if (FindObjectsOfType<DebrisPickup>().Length == 0)
                yield break;

            t += Time.deltaTime; // scaled time so magnets continue to run
            yield return null;
        }
    }

    void SetAllWeaponInput(bool enabled)
    {
        var mounts = FindObjectsOfType<WeaponMount>(includeInactive: true);
        for (int i = 0; i < mounts.Length; i++)
        {
            mounts[i].SetInputEnabled(enabled);
        }
    }
}
