using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public int hidingShotKillCount = 5;
    public float hidingShotDamagePercent = 0.2f;

    [Header("Events")]
    public UnityEvent OnHidingShotReceived;
    public UnityEvent OnStealthKilled;

    private int hidingShotsReceived = 0;
    private bool hasHidingShotDamage = false;

    public void ReceiveHidingShot()
    {
        hidingShotsReceived++;

        if (hidingShotsReceived >= hidingShotKillCount)
        {
            OnStealthKilled?.Invoke();
            Destroy(gameObject);
            return;
        }

        if (!hasHidingShotDamage)
        {
            hasHidingShotDamage = true;
        }

        OnHidingShotReceived?.Invoke();
    }

    public void ApplyHidingDamageToCombatStats(CombatStats stats)
    {
        if (hasHidingShotDamage)
        {
            int damage = Mathf.CeilToInt(stats.maxHealth * hidingShotDamagePercent);
            stats.TakeDamage(damage);
        }
    }

    public bool HasHidingShotDamage()
    {
        return hasHidingShotDamage;
    }

    public int GetHidingShotsReceived()
    {
        return hidingShotsReceived;
    }
}

