using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShapeInputs;
using Unity.VisualScripting;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager instance;
    private void Awake()
    {
        instance = this;
    }
    [Tooltip("Pass sprites according to Shapes enum")]
    public List<Sprite> shapeSprites;
    public int totalEnemies, killedEnemies;
    public EnemySet currentSet;
    public List<EnemyController> levelEnemies;
    public List<EnemyController> currentEnemies;
    public LevelEnemyConfig config;
    public int currentLevel;
    public Transform spawnPosition, spawnPositionAir;
    private void OnEnable()
    {
        EventManager.onGamePlay.AddListener(StartGame);
    }
    private void StartGame()
    {
        currentLevel = GamePreference.selectedLevel;
        totalEnemies = 0;
        foreach (var enemyInfo in config.levels[currentLevel].enemySets)
        {
            for (int i = 0; i < enemyInfo.enemiesData.Count; i++)
            {
                totalEnemies++;
            }
        }
        StartCoroutine(SpawnEnemies());
        killedEnemies = 0;
        UIManager.instance.UpdateProgress(killedEnemies == 0 ? 0 : (float)killedEnemies / totalEnemies);
    }
    public Ability AbilityButtonClick(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.SlowMO:
                SetSlowMoAbility();
                return slowMoAbility;
            case AbilityType.SameShape:
                return sameShape;
        }
        return null;
    }
    public Ability slowMoAbility, sameShape;
    void SetSlowMoAbility()
    {
        StartCoroutine(SlowMoCorotine());
    }
    IEnumerator SlowMoCorotine()
    {
        foreach (var item in levelEnemies)
        {
            item.speed /= slowMoAbility.factor;
        }
        yield return new WaitForSeconds(slowMoAbility.duration);
        foreach (var item in levelEnemies)
        {
            item.speed *= slowMoAbility.factor;
        }
    }
    public void RemoveEnemy(EnemyController enemyController)
    {
        killedEnemies++;
        AudioManager.Instance.PlaySFX(SFX.Destroy);
        UIManager.instance.UpdateProgress(killedEnemies == 0 ? 0 : (float)killedEnemies / totalEnemies);
        if (currentEnemies.Contains(enemyController))
            currentEnemies.Remove(enemyController);
        enemyController.ChangeState(State.Kill);
        if (currentEnemies.Count == 0)
        {
            currentSet.setCompleted = true;
        }
        if (killedEnemies == totalEnemies)
        {
            Debug.LogWarning("Game Should complete");
            EventManager.OnLevelComplete.Invoke();
        }
    }
    public void TakeDamageEnemies(Shapes shapes)
    {
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            var enemy=currentEnemies[i];
            for (int j = 0; j < enemy.shapeDatas.Count; j++)
            {
                var shapeData=enemy.shapeDatas[j];
                if (shapeData.shapeType == shapes)
                {
                    if (currentEnemies[i].TakeDamageAndCheckDeath(shapes))
                        RemoveEnemy(currentEnemies[i]);
                }
            }
        }
        // foreach (var item in currentEnemies)
        // {
        //     if (item.shapeType == shapes)
        //     {
        //         RemoveEnemy(item);
        //     }
        // }
    }
    public void CompleteSet()
    {
        currentSet.setCompleted = true;
    }
    IEnumerator SpawnEnemies()
    {
        foreach (var enemyInfo in config.levels[currentLevel].enemySets)
        {
            currentSet = enemyInfo;
            for (int i = 0; i < enemyInfo.enemiesData.Count; i++)
            {
                yield return new WaitForSeconds(enemyInfo.enemiesData[i].spawnDely);
                var enemy =
                Instantiate(enemyInfo.enemiesData[i].prefab,
                (enemyInfo.enemiesData[i].enemyType == EnemyType.air) ? spawnPositionAir.position : spawnPosition.position,
                Quaternion.identity);
                var enemyController = enemy.GetComponent<EnemyController>();
                AssignRandomShape(enemyController);
                var enemyData = enemyInfo.enemiesData[i];
                AssignValues(enemyController, enemyData.speed, enemyData.attackPower, enemyData.health);
                currentEnemies.Add(enemyController);
            }
            yield return new WaitUntil(() => currentSet.setCompleted);
        }
    }
    public void AssignRandomShape(EnemyController enemyController)
    {
        foreach (var shapeData in enemyController.shapeDatas)
        {
            int randomValue = UnityEngine.Random.Range(0, 17);
            shapeData.shapeType = (Shapes)randomValue;
            shapeData.shapeSprite.sprite = shapeSprites[randomValue];
        }
        //assign speed and other values
    }
    public void AssignValues(EnemyController enemyController, float speed, int damage, int health)
    {
        enemyController.speed = speed;
        enemyController.damage = damage;
        enemyController.health = health;
    }
}
[Serializable]
public class Ability
{
    public AbilityType type;
    public float duration;
    public int levelRequire;
    public float factor;
}
public enum AbilityType
{
    SlowMO, SameShape
}