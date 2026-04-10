using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfoUI : MonoBehaviour
{
    [Header("Player Health")]
    public TextMeshProUGUI playerHealthLabel;
    public Image playerHealthBarFill;

    [Header("Enemy Health")]
    public TextMeshProUGUI enemyHealthLabel;
    public Image enemyHealthBarFill;
    public TextMeshProUGUI enemyNameText;

    [Header("ATB")]
    public Image atbBarFill;

    [Header("Ammo")]
    public TextMeshProUGUI ammoText;

    [Header("References")]
    public CombatManager combatManager;
    public ATBSystem atbSystem;
    public AmmoManager ammoManager;

    private CombatStats playerStats;
    private CombatStats enemyStats;

    public void InitializeCombatUI(CombatStats player, CombatStats enemy)
    {
        playerStats = player;
        enemyStats = enemy;

        playerHealthLabel.text = player.characterName;
        enemyHealthLabel.text = enemy.characterName;
        enemyNameText.text = enemy.characterName;

        UpdateHealthBars();
        UpdateAmmoDisplay();
    }

    void Update()
    {
        if (playerStats == null || enemyStats == null)
            return;

        UpdateHealthBars();
        UpdateAmmoDisplay();
        UpdateATBBar();
    }

    void UpdateHealthBars()
    {
        float playerHealthPercent = (float)playerStats.currentHealth / playerStats.maxHealth;
        playerHealthBarFill.fillAmount = playerHealthPercent;

        float enemyHealthPercent = (float)enemyStats.currentHealth / enemyStats.maxHealth;
        enemyHealthBarFill.fillAmount = enemyHealthPercent;
    }

    void UpdateAmmoDisplay()
    {
        ammoText.text = "Ammo: " + ammoManager.GetCurrentAmmo() + "/" + ammoManager.GetMaxAmmo();
    }

    void UpdateATBBar()
    {
        float playerGauge = atbSystem.GetPlayerGauge(playerStats);
        atbBarFill.fillAmount = playerGauge;
    }
}
