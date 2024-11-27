using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OmmyShapeML
{
[CreateAssetMenu(fileName = "LevelEnemyConfig", menuName = "Level Configurations/Enemy Config", order = 1)]
public class LevelEnemyConfig : ScriptableObject
{
    [System.Serializable]
    public struct EnemySpawnInfo
    {
        public EnemyType enemyType;
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
public class EnemyType
{
    public string name;
    public GameObject prefab;
    public int health;
    public float speed;
    public int attackPower;
}
}