using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShapeInputs;
using TMPro;
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
    public List<EnemyController> currentEnemies;
    public LevelEnemyConfig config;
    public AbilitiesConfig abilitiesConfig;
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
    public bool hasSlowmo;
    public void SetSlowMoAbility(bool active)
    {
        hasSlowmo=active;
        print("slowmo abilityActivated");
        if (active)
        {
            foreach (var item in currentEnemies)
            {
                item.SlowMo(hasSlowmo,abilitiesConfig.slowMoAbility.factor);
            }
        }
        else
        {
            foreach (var item in currentEnemies)
            {
                item.SlowMo(hasSlowmo,abilitiesConfig.slowMoAbility.factor);
                //item.speed *= abilitiesConfig.slowMoAbility.factor;
            }
        }
    }
    public void SetMirrorAbility(bool active)
    {
        print("mirror abilityActivated");
        var _shapeData = currentEnemies[0].shapeDatas[0];
        //var shapeType = currentEnemies[0].shapeDatas[0].shapeType;
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            foreach (var shapeData in currentEnemies[i].shapeDatas)
            {
                shapeData.shapeSprite.sprite = _shapeData.shapeSprite.sprite;  
                shapeData.shapeType = _shapeData.shapeType;  
            }
            //currentEnemies[i].shapeDatas[0].shapeType = shapeType;
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
    public List<EnemyController> damagedEnemies;
    public void TakeDamageEnemies(Shapes shapes)
    {
        damagedEnemies=new List<EnemyController>();
        var Count=currentEnemies.Count;
        //for (int i = 0; i < Count; i++)
        foreach (var enemy in currentEnemies)
        {
            print("loop count");
            //var enemy = currentEnemies[i];
            for (int j = 0; j < enemy.shapeDatas.Count; j++)
            {
                var shapeData = enemy.shapeDatas[j];
                if (shapeData.shapeType == shapes)
                {
                    if (enemy.TakeDamageAndCheckDeath(shapes))
                    {
                        print("Damage Enemy");
                        //Debug.Break();
                        //RemoveEnemy(currentEnemies[i]);
                        RemoveEnemy(enemy);
                        //damagedEnemies.Add(enemy);
                    }
                }
            }
        }
        // for (int i = 0; i < damagedEnemies.Count; i++)
        // {
        //     RemoveEnemy(damagedEnemies[i]);
        // }
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
        //enemyController.speed = speed/abilitiesConfig.slowMoAbility.factor;
        enemyController.speed = speed;
        enemyController.damage = damage;
        enemyController.health = health;
        if(hasSlowmo)
        enemyController.SlowMo(true,abilitiesConfig.slowMoAbility.factor);
    }
}
