using UnityEngine;

public class CombatTest : MonoBehaviour
{
    public CombatManager combatManager;
    public CombatStats playerStats;
    public CombatStats enemyStats;
    public GameObject combatMenuPanel;
    public InputManager inputManager;

    private bool battleStarted = false;

    void OnEnable()
    {
        inputManager.OnConfirmPressed.AddListener(HandleConfirm);
    }

    void OnDisable()
    {
        inputManager.OnConfirmPressed.RemoveListener(HandleConfirm);
    }

    void HandleConfirm()
    {
        if (!battleStarted)
        {
            battleStarted = true;
            combatMenuPanel.SetActive(true);
            combatManager.StartBattle(playerStats, enemyStats);
            this.enabled = false;
        }
    }
}
