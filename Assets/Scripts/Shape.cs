using System.Collections;
using System.Collections.Generic;
using ShapeInputs;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Shapes shapeType;
    public State currentState;
    public SpriteRenderer shapeSprite;
    public GameObject body;
    public Animator animator;

    public float speed=1;
    public int damage=1;
    public float health=1;
    public virtual void Attack()
    {

    } 
    public virtual void Update()
    {

    }
    public enum State
    {
        Idle,
        Run,
        Agressive,
        Attack,
        Kill
    }
}
