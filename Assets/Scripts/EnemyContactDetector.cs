using UnityEngine;

public class EnemyContactDetector : MonoBehaviour
{
    public OverworldCombatTrigger combatTrigger;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            combatTrigger.HandleEnemyContact(gameObject);
        }
    }
}
