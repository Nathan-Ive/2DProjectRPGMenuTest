using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public PlayerOverworldActions playerActions;
    public CombatManager combatManager;

    [Header("Overworld Objects")]
    public List<GameObject> overworldObjects;

    [Header("Combat Objects")]
    public GameObject combatVisuals;

    [Header("Events")]
    public UnityEvent OnEnteredCombat;
    public UnityEvent OnReturnedToOverworld;

    public void EnterCombat()
    {
        playerMovement.SetEnabled(false);
        playerActions.enabled = false;

        foreach (GameObject obj in overworldObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (combatVisuals != null)
            combatVisuals.SetActive(true);

        OnEnteredCombat?.Invoke();
    }

    public void ReturnToOverworld()
    {
        playerMovement.SetEnabled(true);
        playerActions.enabled = true;

        foreach (GameObject obj in overworldObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        if (combatVisuals != null)
            combatVisuals.SetActive(false);

        OnReturnedToOverworld?.Invoke();
    }
}