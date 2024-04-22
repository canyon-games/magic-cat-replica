using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelEnemyConfig", menuName = "Level Configurations/Enemy Config", order = 1)]
public class LevelEnemyConfig : ScriptableObject
{
    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public EnemyData enemydata;
        public int count;
    }
    [System.Serializable]
    public struct Level
    {
        public List<EnemySpawnInfo> enemies;
    }
    public List<Level> levels;
}


[System.Serializable]
public class EnemyData
{
    public EnemyType enemyType;
    public GameObject prefab;
    public int health;
    public float speed;
    public int attackPower;
    public float spawnDely;
}
public enum EnemyType
{
    air,ground
}