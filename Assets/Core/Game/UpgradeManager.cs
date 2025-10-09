using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI Refs")]
    [SerializeField] GameObject shopPanel;
    [SerializeField] TextMeshProUGUI debrisLabel;
    [SerializeField] Button[] optionButtons;                // size 4 in Inspector
    [SerializeField] TextMeshProUGUI[] optionLabels;        // child labels for the buttons
    [SerializeField] Button startNextWaveButton;

    [Header("Gameplay Refs")]
    [SerializeField] WeaponMount weapon;
    [SerializeField] PlayerHealth player;

    [Header("Costs")]
    public int costFireRate = 5;
    public int costBulletSpeed = 5;
    public int costSpread = 8;
    public int costRepair = 10;

    // Internal
    bool isOpen;
    bool spreadPurchased;
    List<UpgradeOption> _pool;
    UpgradeOption[] _rolled = new UpgradeOption[4];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (shopPanel) shopPanel.SetActive(false);

        // Build the pool once
        _pool = new List<UpgradeOption>()
        {
            new UpgradeOption(UpgradeType.FireRateUp,    "Fire Rate +20%",  "Shoot faster",               costFireRate),
            new UpgradeOption(UpgradeType.BulletSpeedUp, "Bullet Speed +20%","Bullets travel faster",     costBulletSpeed),
            new UpgradeOption(UpgradeType.Spread3,       "Spread x3",       "3-shot spread, +coverage",   costSpread,  oneTime:true),
            new UpgradeOption(UpgradeType.Repair1,       "Repair +1 HP",    "Restore 1 HP (to max)",      costRepair),
        };

        // Wire up button callbacks once
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int idx = i;
            optionButtons[i].onClick.AddListener(() => PickOption(idx));
        }
        if (startNextWaveButton)
            startNextWaveButton.onClick.AddListener(StartNextWave_Click);
    }

    public bool IsOpen => isOpen;

    void UpdateDebrisUI()
    {
        if (debrisLabel) debrisLabel.text = "Debris: " + GameManager.Debris;
    }

    public void OpenShop()
    {
        isOpen = true;
        Time.timeScale = 0f;
        if (shopPanel) shopPanel.SetActive(true);
        UpdateDebrisUI();
        RollNewOptions();
        RefreshOptionLabels();
    }

    public void CloseShop()
    {
        isOpen = false;
        if (shopPanel) shopPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void RollNewOptions()
    {
        // Simple unique roll from available pool; skip one-time items already bought
        var candidates = new List<UpgradeOption>(_pool);

        if (spreadPurchased)
            candidates.RemoveAll(o => o.type == UpgradeType.Spread3);

        // Roll up to 4 unique options
        for (int i = 0; i < _rolled.Length; i++)
        {
            if (candidates.Count == 0) { _rolled[i] = null; continue; }
            int r = Random.Range(0, candidates.Count);
            _rolled[i] = candidates[r];
            candidates.RemoveAt(r);
        }
    }

    void RefreshOptionLabels()
    {
        for (int i = 0; i < optionLabels.Length; i++)
        {
            var lbl = optionLabels[i];
            var btn = optionButtons[i];

            if (_rolled[i] == null)
            {
                lbl.text = "—";
                btn.interactable = false;
            }
            else
            {
                var o = _rolled[i];
                lbl.text = $"{o.title}  (Cost {o.cost})";
                btn.interactable = GameManager.Debris >= o.cost;
            }
        }
    }

    void PickOption(int index)
    {
        var opt = _rolled[index];
        if (opt == null) return;
        if (GameManager.Debris < opt.cost) return;

        GameManager.Debris -= opt.cost;
        ApplyUpgrade(opt);
        UpdateDebrisUI();

        // After purchase, disable that button (single pick per roll)
        optionButtons[index].interactable = false;
    }

    void ApplyUpgrade(UpgradeOption o)
    {
        switch (o.type)
        {
            case UpgradeType.FireRateUp:
                if (weapon) weapon.fireRate *= 1.20f;
                break;

            case UpgradeType.BulletSpeedUp:
                if (weapon) weapon.bulletSpeedMultiplier *= 1.20f;
                break;

            case UpgradeType.Spread3:
                if (weapon)
                {
                    weapon.projectilesPerShot = Mathf.Max(weapon.projectilesPerShot, 3);
                    weapon.spreadDeg = Mathf.Max(weapon.spreadDeg, 10f);
                    spreadPurchased = true;
                }
                break;

            case UpgradeType.Repair1:
                if (player) player.Heal(1);
                break;
        }
    }

    public void StartNextWave_Click()
    {
        CloseShop();
    }
}
