using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class ATBSystem : MonoBehaviour
{
    [Header("Settings")]
    public float baselineFillTime = 5f;

    [Header("Party Size Minimums")]
    public float soloMinimum = 1f;
    public float duoMinimum = 2f;
    public float trioMinimum = 2.5f;
    public float quadMinimum = 3f;

    [Header("Consecutive Action Caps")]
    public int capVsSolo = 5;
    public int capVsDuo = 4;
    public int capVsTrio = 3;
    public int capVsQuad = 2;

    [Header("Events")]
    public UnityEvent<CombatStats> OnATBFilled;

    private List<CombatStats> playerParty = new List<CombatStats>();
    private List<CombatStats> enemyParty = new List<CombatStats>();
    private Dictionary<CombatStats, float> gauges = new Dictionary<CombatStats, float>();
    private Dictionary<CombatStats, float> fillTimes = new Dictionary<CombatStats, float>();

    private int playerConsecutiveActions = 0;
    private int enemyConsecutiveActions = 0;
    private bool isPaused = true;

    public void InitializeBattle(List<CombatStats> players, List<CombatStats> enemies)
    {
        playerParty = players;
        enemyParty = enemies;

        gauges.Clear();
        fillTimes.Clear();
        playerConsecutiveActions = 0;
        enemyConsecutiveActions = 0;

        List<CombatStats> allCombatants = new List<CombatStats>();
        allCombatants.AddRange(players);
        allCombatants.AddRange(enemies);

        int lowestSpeed = allCombatants.Min(c => c.speed);

        foreach (CombatStats combatant in allCombatants)
        {
            gauges[combatant] = 0f;

            float rawFillTime = baselineFillTime / ((float)combatant.speed / lowestSpeed);
            float minimum = GetMinimumFillTime(combatant);
            fillTimes[combatant] = Mathf.Max(rawFillTime, minimum);
        }
    }

    float GetMinimumFillTime(CombatStats combatant)
    {
        int partySize;

        if (playerParty.Contains(combatant))
            partySize = playerParty.Count;
        else
            partySize = enemyParty.Count;

        switch (partySize)
        {
            case 1: return soloMinimum;
            case 2: return duoMinimum;
            case 3: return trioMinimum;
            default: return quadMinimum;
        }
    }

    int GetConsecutiveCap(CombatStats combatant)
    {
        int opposingSize;

        if (playerParty.Contains(combatant))
            opposingSize = enemyParty.Count;
        else
            opposingSize = playerParty.Count;

        switch (opposingSize)
        {
            case 1: return capVsSolo;
            case 2: return capVsDuo;
            case 3: return capVsTrio;
            default: return capVsQuad;
        }
    }

    void Update()
    {
        if (isPaused)
            return;

        CombatStats readyActor = null;
        float highestGauge = 0f;

        foreach (var pair in gauges.ToList())
        {
            if (pair.Key.IsDead())
                continue;

            float newGauge = pair.Value + (Time.deltaTime / fillTimes[pair.Key]);
            gauges[pair.Key] = newGauge;

            if (newGauge >= 1f && newGauge > highestGauge)
            {
                highestGauge = newGauge;
                readyActor = pair.Key;
            }
        }

        if (readyActor != null)
        {
            bool isPlayer = playerParty.Contains(readyActor);

            if (isPlayer)
            {
                int cap = GetConsecutiveCap(readyActor);
                if (enemyConsecutiveActions >= cap)
                {
                    CombatStats forcedPlayer = GetSlowGaugePlayer();
                    if (forcedPlayer != null)
                        readyActor = forcedPlayer;
                }
            }

            isPaused = true;
            gauges[readyActor] = 0f;

            if (playerParty.Contains(readyActor))
            {
                playerConsecutiveActions++;
                enemyConsecutiveActions = 0;
            }
            else
            {
                enemyConsecutiveActions++;
                playerConsecutiveActions = 0;
            }

            OnATBFilled?.Invoke(readyActor);
        }
    }

    CombatStats GetSlowGaugePlayer()
    {
        CombatStats highest = null;
        float highestGauge = 0f;

        foreach (CombatStats player in playerParty)
        {
            if (!player.IsDead() && gauges[player] > highestGauge)
            {
                highestGauge = gauges[player];
                highest = player;
            }
        }

        return highest;
    }

    public void ResumeATB()
    {
        isPaused = false;
    }

    public void PauseATB()
    {
        isPaused = true;
    }
}