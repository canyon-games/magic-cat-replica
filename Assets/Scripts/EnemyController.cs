using System.Collections;
using System.Collections.Generic;
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
                animator.speed = 1;
                currentAction = Running;
                break;
            case State.Agressive:
                animator.speed = 1.5f;
                currentAction = Agressive;
                break;
            case State.Attack:
                PlayerController.instance.TakeDamage(damage);
                attackParticles.SetActive(true);
                ChangeState(State.Kill);
                shapeSprite.gameObject.SetActive(false);
                body.SetActive(false);
                //transform.LookAt(PlayerController.instance.transform.position);
                animator.speed = 0;
                currentAction = Attacking;
                EnemyManager.instance.RemoveEnemy(this);
                break;
            case State.Kill:
                body.SetActive(false);
                shapeSprite.gameObject.SetActive(false);
                destroyParticles.SetActive(true);
                animator.speed = 0;
                currentAction = null;
                //Invoke(nameof(DestroyBehaviour),2);
                Destroy(gameObject,2);
                break;
        }
    }
    public bool TakeDamageAndCheckDeath()
    {
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
