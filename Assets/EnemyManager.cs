using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShapeInputs;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Tooltip("Pass sprites according to Shapes enum")]
    public List<Sprite> shapeSprites;
    public int totalEnemies;
    public List<EnemyController> levelEnemies;
    public List<EnemyController> currentEnemies;
    public LevelEnemyConfig config;
    public int currentLevel;
    public float spawnDely;
    public Transform spawnPosition;
    private void Start()
    {
        foreach (var enemyInfo in config.levels[currentLevel].enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                totalEnemies++;
            }
        }


        StartCoroutine(SpawnEnemies());
    }
    public void TakeDamageEnemies(Shapes shapes)
    {
        currentEnemies.Where(enemy => enemy.shapeType == shapes).ToList().ForEach(enemy =>
        {

            if (enemy.TakeDamageAndCheckDeath())
            {
                totalEnemies--;
                currentEnemies.Remove(enemy);
                if (totalEnemies <= 0)
                {
                    EventManager.OnLevelComplete.Invoke();
                }
            }
        });
    }
    IEnumerator SpawnEnemies()
    {
        foreach (var enemyInfo in config.levels[currentLevel].enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                yield return new WaitForSeconds(spawnDely);
                var enemy = Instantiate(enemyInfo.enemyType.prefab, spawnPosition.position, Quaternion.identity);
                var enemyController = enemy.GetComponent<EnemyController>();
                AssignRandomShape(enemyController);
                AssignValues(enemyController, enemyInfo.enemyType.speed,enemyInfo.enemyType.attackPower);
                currentEnemies.Add(enemyController);
            }
        }
    }
    public void AssignRandomShape(EnemyController enemyController)
    {
        int randomValue = Random.Range(0, 17);
        enemyController.shapeType = (Shapes)randomValue;
        enemyController.shapeSprite.sprite = shapeSprites[randomValue];
        //assign speed and other values
    }
    public void AssignValues(EnemyController enemyController, float speed, int damage)
    {
        enemyController.speed = speed;
        enemyController.damage = damage;
    }
}
