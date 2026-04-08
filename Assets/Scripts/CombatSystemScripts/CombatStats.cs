using UnityEngine;

public class CombatStats : MonoBehaviour
{
    public string characterName;
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public int defense;
    public int speed;

    public bool isDefending = false;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public int GetEffectiveDefense()
    {
        if (isDefending)
            return defense * 2;
        return defense;
    }

    public void ResetDefend()
    {
        isDefending = false;
    }
}