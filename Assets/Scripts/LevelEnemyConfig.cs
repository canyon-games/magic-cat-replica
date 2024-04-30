using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelEnemyConfig", menuName = "Level Configurations/Enemy Config", order = 1)]
public class LevelEnemyConfig : ScriptableObject
{
    [System.Serializable]
    public struct Level
    {
        public bool hasBossLevel;
        public List<EnemySet> enemySets;
    }
    public List<Level> levels;
    //public EnemySet enemyset;
}


[System.Serializable]
public struct EnemySet
{
    public List<EnemyData> enemiesData;
    public bool setCompleted;
}
[System.Serializable]
public class EnemyData
{
    public EnemyType enemyType;
    public GameObject prefab;
    public int health=1;
    public float speed=1;
    public int attackPower=10;
    public float spawnDely=1;
}
public enum EnemyType
{
    air,ground
}