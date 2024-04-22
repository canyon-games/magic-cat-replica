using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class EnemyController : Enemy
{
    public GameObject particles;
    public delegate void EnemyAction();
    public EnemyAction currentAction;
    private void Start()
    {
        currentState = State.Idle;
        ChangeState(State.Run);
    }
    public override void Update()
    {
        if (Vector2.Distance(transform.position, PlayerController.instance.transform.position) < 0.5)
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
                animator.speed = 2;
                currentAction = Agressive;
                break;
            case State.Attack:
                PlayerController.instance.TakeDamage(damage);
                particles.SetActive(true);
                shapeSprite.gameObject.SetActive(false);
                body.SetActive(false);
                //transform.LookAt(PlayerController.instance.transform.position);
                animator.speed = 0;
                currentAction = Attacking;
                break;
            case State.Kill:
                animator.speed = 0;
                Destroy(gameObject);
                currentAction = DestroyBehaviour;
                break;
        }
    }
    public bool TakeDamageAndCheckDeath()
    {
        health--;
        if(health <= 0)
        {
            ChangeState(State.Kill);
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
        transform.position = Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, speed * 2 * Time.deltaTime);
    }
    public void Attacking()
    {

    }
    public void DestroyBehaviour()
    {
        
    }
}
