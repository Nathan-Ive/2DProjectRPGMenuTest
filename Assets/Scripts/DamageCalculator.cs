using UnityEngine;

public static class DamageCalculator
{
    private const float MAX_OFFENSE_MULTIPLIER = 2f;
    private const float MAX_OFFENSE_RATIO = 11f;
    private const float DEFEND_MINIMUM_REDUCTION = 0.25f;

    public static int CalculateSlashDamage(int attackStat, int defenseStat, bool targetIsDefending)
    {
        int effectiveDefense = targetIsDefending ? defenseStat * 2 : defenseStat;

        float damage;

        if (attackStat <= effectiveDefense)
        {
            damage = (float)(attackStat * attackStat) / effectiveDefense;
        }
        else
        {
            float ratio = (float)attackStat / effectiveDefense;
            float logRatio = Mathf.Log(ratio) / Mathf.Log(MAX_OFFENSE_RATIO);
            float multiplier = 1f + Mathf.Pow(logRatio, 1.5f);

            if (multiplier > MAX_OFFENSE_MULTIPLIER)
                multiplier = MAX_OFFENSE_MULTIPLIER;

            damage = attackStat * multiplier;
        }

        if (targetIsDefending)
        {
            float normalDamage = CalculateSlashDamageRaw(attackStat, defenseStat);
            float reducedDamage = normalDamage * (1f - DEFEND_MINIMUM_REDUCTION);

            if (damage > reducedDamage)
                damage = reducedDamage;
        }

        int finalDamage = Mathf.CeilToInt(damage);
        if (finalDamage < 1)
            finalDamage = 1;

        return finalDamage;
    }

    private static float CalculateSlashDamageRaw(int attackStat, int defenseStat)
    {
        if (attackStat <= defenseStat)
        {
            return (float)(attackStat * attackStat) / defenseStat;
        }
        else
        {
            float ratio = (float)attackStat / defenseStat;
            float logRatio = Mathf.Log(ratio) / Mathf.Log(MAX_OFFENSE_RATIO);
            float multiplier = 1f + Mathf.Pow(logRatio, 1.5f);

            if (multiplier > MAX_OFFENSE_MULTIPLIER)
                multiplier = MAX_OFFENSE_MULTIPLIER;

            return attackStat * multiplier;
        }
    }

    public static int CalculateGunDamage(int damagePerShot, int defenseStat, bool targetIsDefending)
    {
        int effectiveDefense = targetIsDefending ? defenseStat * 2 : defenseStat;

        float damage = damagePerShot;

        if (targetIsDefending)
        {
            damage = damagePerShot * (1f - DEFEND_MINIMUM_REDUCTION);
        }

        int finalDamage = Mathf.CeilToInt(damage);
        if (finalDamage < 1)
            finalDamage = 1;

        return finalDamage;
    }
}