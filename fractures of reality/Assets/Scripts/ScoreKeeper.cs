using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScoreKeeper : ScriptableObject
{
    int skelesKilled;
    int BeholdersKilled;
    int NecromancersKilled;
    int DragonsKilled;
    [SerializeField] public int skeleValue;
    [SerializeField] public int BeholdersValue;
    [SerializeField] public int NecromancersValue;
    [SerializeField] public int DragonsKilledValue;
    [SerializeField] public int playerDeathScoreDeduction;

    public void AddTally(DestructibleHealthCore.EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
                break;
            case DestructibleHealthCore.EnemyType.Skeleton:
                {
                    skelesKilled++;
                    break;
                }
            case DestructibleHealthCore.EnemyType.Beholder:
                {
                    BeholdersKilled++;
                    break;
                }
            case DestructibleHealthCore.EnemyType.Necromancer:
                {
                    NecromancersKilled++;
                    break;
                }
            case DestructibleHealthCore.EnemyType.Dragon:
                {
                    DragonsKilled++;
                    break;
                }
        }
    }

    public void ResetScoreKeeper()
    {
        skelesKilled = 0;
        BeholdersKilled = 0;
        NecromancersKilled = 0;
        DragonsKilled = 0;
    }

    public int getSkeles()
    {
        return skelesKilled;
    }
    public int getBeholders()
    {
        return BeholdersKilled;
    }
    public int getNecromancers()
    {
        return NecromancersKilled;
    }
    public int getDragon()
    {
        return DragonsKilled;
    }
}
