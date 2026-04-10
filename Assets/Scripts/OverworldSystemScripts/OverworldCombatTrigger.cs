using UnityEngine;
using UnityEngine.Events;

public enum BattleAdvantage
{
    Neutral,
    PlayerAdvantage,
    EnemyAdvantage
}

public class OverworldCombatTrigger : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;
    public CombatStats playerCombatStats;
    public PlayerMovement playerMovement;
    public PlayerOverworldActions playerActions;

    [Header("Events")]
    public UnityEvent<BattleAdvantage> OnBattleTriggered;

    public void HandleEnemyContact(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        CombatStats enemyStats = enemy.GetComponent<CombatStats>();

        if (enemyStats == null)
            return;

        // Apply hiding shot damage if flagged
        if (enemyHealth != null)
        {
            enemyHealth.ApplyHidingDamageToCombatStats(enemyStats);
        }

        BattleAdvantage advantage = DetermineAdvantage(enemy);
        OnBattleTriggered?.Invoke(advantage);

        // Start battle with appropriate ATB settings
        combatManager.StartBattle(playerCombatStats, enemyStats);

        // Apply advantage after battle starts
        if (advantage == BattleAdvantage.EnemyAdvantage)
        {
            // Enemy gets instant first turn — handled by ATB
            combatManager.atbSystem.PauseATB();
            combatManager.atbSystem.OnATBFilled?.Invoke(enemyStats);
        }
    }

    BattleAdvantage DetermineAdvantage(GameObject enemy)
    {
        Vector2 playerFacing = playerMovement.GetFacingDirection();
        Vector2 toEnemy = ((Vector2)(enemy.transform.position - playerMovement.transform.position)).normalized;
        Vector2 toPlayer = ((Vector2)(playerMovement.transform.position - enemy.transform.position)).normalized;

        EnemyDetection enemyDetection = enemy.GetComponent<EnemyDetection>();
        Vector2 enemyFacing = Vector2.down;
        if (enemyDetection != null)
            enemyFacing = enemyDetection.GetFacingDirection();

        // Check if enemy touched the player's back
        float playerDot = Vector2.Dot(playerFacing, toEnemy);
        if (playerDot < -0.5f)
        {
            return BattleAdvantage.EnemyAdvantage;
        }

        return BattleAdvantage.Neutral;
    }

    public void HandlePlayerShotEnemy(GameObject enemy)
    {
        if (playerActions.IsHiding())
        {
            // Shot from hiding — no battle, just damage flag
            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.ReceiveHidingShot();
            }
        }
        else
        {
            // Shot from behind outside hiding — player advantage battle
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            CombatStats enemyStats = enemy.GetComponent<CombatStats>();

            if (enemyStats == null)
                return;

            if (enemyHealth != null)
                enemyHealth.ApplyHidingDamageToCombatStats(enemyStats);

            OnBattleTriggered?.Invoke(BattleAdvantage.PlayerAdvantage);

            combatManager.StartBattle(playerCombatStats, enemyStats);

            // Player gets first turn
            combatManager.atbSystem.PauseATB();
            combatManager.atbSystem.OnATBFilled?.Invoke(playerCombatStats);
        }
    }
}

