using System;
using System.Collections;
using System.Collections.Generic;
using ShapeInputs;
using UnityEngine;
public class EnemyController : Enemy
{
    public GameObject destroyParticles,attackParticles;
    public delegate void EnemyAction();
    public EnemyAction currentAction;
    private void Start()
    {
        currentState = State.Idle;
        ChangeState(State.Run);
    }
    public override void Update()
    {
        if(currentState == State.Kill)return;
        if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) < 1)
        {
            ChangeState(State.Attack);
        }
        else if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) < 3)
        {
            ChangeState(State.Agressive);
        }
        currentAction?.Invoke();
    }
    public void ChangeState(State state)
    {
        if (currentState == state) return;
        currentState = state;
        switch (currentState)
        {
            case State.Run:
                if(animator)animator.speed = 1;
                currentAction = Running;
                break;
            case State.Agressive:
                if(animator)animator.speed = 1.5f;
                currentAction = Agressive;
                break;
            case State.Attack:
                PlayerController.instance.TakeDamage(damage);
                attackParticles.SetActive(true);
                ChangeState(State.Kill);
                //shapeSprite.gameObject.SetActive(false);
                body.SetActive(false);
                //transform.LookAt(PlayerController.instance.transform.position);
                if(animator)animator.speed = 0;
                currentAction = Attacking;
                EnemiesManager.instance.RemoveEnemy(this);
                break;
            case State.Kill:
                if(body)body.SetActive(false);
                shapeDatas.ForEach(data => data.shapeSprite.gameObject.SetActive(false));
                destroyParticles.SetActive(true);
                if(animator)animator.speed = 0;
                currentAction = null;
                //Invoke(nameof(DestroyBehaviour),2);
                Destroy(gameObject,2);
                break;
        }
    }
    public bool TakeDamageAndCheckDeath(Shapes shapes)
    {
        var shapeData=shapeDatas.Find(x => x.shapeType == shapes);
        shapeData.ShapeDrawed();
        if(shapeData.shapeHealth<=0)
        shapeDatas.Remove(shapeData);
        animator.SetTrigger("Hit");
        if(shapeDatas.Count==0)
        {
            return true;
        }
        return false;
        health--;
        if(health <= 0)
        {
            return true;
        }
        return false;
    }
    public void Running()
    {
        //transform.position = Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, speed * Time.deltaTime);
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
    public void Agressive()
    {
        transform.position = Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, speed * 1.5f * Time.deltaTime);
    }
    public void Attacking()
    {

    }
    public void DestroyBehaviour()
    {
        Destroy(gameObject);
    }
}
[Serializable]
public class EnemyBehavior
{
    public ActionType actionType;
    [Header("When Moving")]
    public bool isSpawing;
    public Vector3 posA, posB;
    public bool isMovingUntiDistanceWithPlayer;
    public bool isMovingStraightUntilOtherSide;
    public Vector3 distanceWithPlayer;
    public float travelSpeed;
    [Header("Attack")]
    public float waitTimeBeforeAttack;
    public bool isAttackStraightHorizontal;
    public bool hasAttackAnimation;
    [Header("Shooting")]
    public int shootOccurrance;
    public float timeBeforeShooting;
    public float timeAfterShooting;
    public bool infinityShooting;
    [Header("Throw Other Enemies")]
    public List<Transform> trowTransformStart;
    public List<Transform> trowTransformEnd;
    public float TimeBeforeThrowing;
    public float TimeAfterThrowing;

}