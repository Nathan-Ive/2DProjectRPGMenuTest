using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    [Header("References")]
    public ATBSystem atbSystem;
    public AmmoManager ammoManager;
    public MenuManager menuManager;
    public MenuBox combatMenuBox;
    public MenuBox attackSubMenuBox;
    public DialogueBox dialogueBox;

    [Header("Combatants")]
    public CombatStats playerStats;
    public CombatStats enemyStats;

    [Header("Reward Items")]
    [TextArea(1, 3)]
    public List<string> rewardItems;

    [Header("Events")]
    public UnityEvent OnBattleStarted;
    public UnityEvent OnBattleWon;
    public UnityEvent OnBattleLost;
    public UnityEvent OnReturnToOverworld;

    private CombatStats currentActor;
    private bool battleActive = false;

    public void StartBattle(CombatStats player, CombatStats enemy)
    {
        playerStats = player;
        enemyStats = enemy;
        battleActive = true;

        List<CombatStats> players = new List<CombatStats> { playerStats };
        List<CombatStats> enemies = new List<CombatStats> { enemyStats };

        atbSystem.InitializeBattle(players, enemies);
        atbSystem.OnATBFilled.AddListener(HandleTurnReady);

        OnBattleStarted?.Invoke();
        atbSystem.ResumeATB();
    }

    void HandleTurnReady(CombatStats actor)
    {
        currentActor = actor;
        atbSystem.PauseATB();

        if (playerStats == actor)
        {
            combatMenuBox.transform.parent.gameObject.SetActive(true);
            menuManager.OpenMenu(combatMenuBox);
        }
        else
        {
            ExecuteEnemyTurn();
        }
    }


    public void PlayerSlash()
    {
        menuManager.CloseAllMenus();
        combatMenuBox.transform.parent.gameObject.SetActive(false);

        int damage = DamageCalculator.CalculateSlashDamage(
            playerStats.attack,
            enemyStats.defense,
            enemyStats.isDefending
        );

        enemyStats.ResetDefend();
        enemyStats.TakeDamage(damage);

        List<string> lines = new List<string>
        {
            playerStats.characterName + " slashes for " + damage + " damage!"
        };

        dialogueBox.OnDialogueEnded.AddListener(AfterPlayerAction);
        dialogueBox.StartDialogue(lines);
    }

    public void PlayerShoot()
    {
        menuManager.CloseAllMenus();
        combatMenuBox.transform.parent.gameObject.SetActive(false);

        if (!ammoManager.CanShoot())
        {
            List<string> noAmmoLines = new List<string> { "No ammo! You need to reload first." };
            dialogueBox.OnDialogueEnded.AddListener(ReturnToPlayerMenu);
            dialogueBox.StartDialogue(noAmmoLines);
            return;
        }

        int shotDamage = ammoManager.Shoot();
        int damage = DamageCalculator.CalculateGunDamage(
            shotDamage,
            enemyStats.defense,
            enemyStats.isDefending
        );

        enemyStats.ResetDefend();
        enemyStats.TakeDamage(damage);

        List<string> lines = new List<string>
        {
            playerStats.characterName + " fires! " + damage + " damage! (" + ammoManager.GetCurrentAmmo() + "/" + ammoManager.GetMaxAmmo() + " ammo remaining)"
        };

        dialogueBox.OnDialogueEnded.AddListener(AfterPlayerAction);
        dialogueBox.StartDialogue(lines);
    }

    public void PlayerDefend()
    {
        menuManager.CloseAllMenus();
        combatMenuBox.transform.parent.gameObject.SetActive(false);

        playerStats.isDefending = true;

        List<string> lines = new List<string>
        {
            playerStats.characterName + " takes a defensive stance!"
        };

        dialogueBox.OnDialogueEnded.AddListener(AfterPlayerAction);
        dialogueBox.StartDialogue(lines);
    }

    public void PlayerReload()
    {
        menuManager.CloseAllMenus();
        combatMenuBox.transform.parent.gameObject.SetActive(false);

        if (!ammoManager.CanReload())
        {
            List<string> fullLines = new List<string> { "Already fully loaded!" };
            dialogueBox.OnDialogueEnded.AddListener(ReturnToPlayerMenu);
            dialogueBox.StartDialogue(fullLines);
            return;
        }

        ammoManager.ReloadOne();

        List<string> lines = new List<string>
        {
            playerStats.characterName + " reloads. (" + ammoManager.GetCurrentAmmo() + "/" + ammoManager.GetMaxAmmo() + ")"
        };

        dialogueBox.OnDialogueEnded.AddListener(AfterPlayerAction);
        dialogueBox.StartDialogue(lines);
    }

    public void PlayerEscape()
    {
        menuManager.CloseAllMenus();
        combatMenuBox.transform.parent.gameObject.SetActive(false);

        battleActive = false;

        List<string> lines = new List<string> { "Got away safely!" };

        dialogueBox.OnDialogueEnded.AddListener(ReturnToOverworld);
        dialogueBox.StartDialogue(lines);
    }

    void ExecuteEnemyTurn()
    {

        playerStats.ResetDefend();

        int damage = DamageCalculator.CalculateSlashDamage(
            enemyStats.attack,
            playerStats.defense,
            playerStats.isDefending
        );

        playerStats.TakeDamage(damage);

        List<string> lines = new List<string>
        {
            enemyStats.characterName + " attacks for " + damage + " damage!"
        };

        dialogueBox.OnDialogueEnded.AddListener(AfterEnemyAction);
        dialogueBox.StartDialogue(lines);
    }

    void AfterPlayerAction()
    {
        dialogueBox.OnDialogueEnded.RemoveListener(AfterPlayerAction);
        CheckBattleEnd();
    }

    void AfterEnemyAction()
    {
        dialogueBox.OnDialogueEnded.RemoveListener(AfterEnemyAction);
        CheckBattleEnd();
    }

    void ReturnToPlayerMenu()
    {
        dialogueBox.OnDialogueEnded.RemoveListener(ReturnToPlayerMenu);
        menuManager.OpenMenu(combatMenuBox);
    }

    void CheckBattleEnd()
    {
        if (enemyStats.IsDead())
        {
            BattleWon();
        }
        else if (playerStats.IsDead())
        {
            BattleLost();
        }
        else
        {
            atbSystem.ResumeATB();
        }
    }

    void BattleWon()
    {
        battleActive = false;
        atbSystem.OnATBFilled.RemoveListener(HandleTurnReady);

        List<string> lines = new List<string>
        {
            "You defeated " + enemyStats.characterName + "!"
        };

        foreach (string item in rewardItems)
        {
            lines.Add("Obtained: " + item);
        }

        dialogueBox.OnDialogueEnded.AddListener(HandleVictoryEnd);
        dialogueBox.StartDialogue(lines);
    }

    void HandleVictoryEnd()
    {
        dialogueBox.OnDialogueEnded.RemoveListener(HandleVictoryEnd);
        OnBattleWon?.Invoke();
        OnReturnToOverworld?.Invoke();
    }

    void BattleLost()
    {
        battleActive = false;
        atbSystem.OnATBFilled.RemoveListener(HandleTurnReady);

        List<string> lines = new List<string>
        {
            playerStats.characterName + " has fallen..."
        };

        dialogueBox.OnDialogueEnded.AddListener(HandleDefeatEnd);
        dialogueBox.StartDialogue(lines);
    }

    void HandleDefeatEnd()
    {
        dialogueBox.OnDialogueEnded.RemoveListener(HandleDefeatEnd);
        OnBattleLost?.Invoke();
    }

    void ReturnToOverworld()
    {
        dialogueBox.OnDialogueEnded.RemoveListener(ReturnToOverworld);
        OnReturnToOverworld?.Invoke();
    }
}