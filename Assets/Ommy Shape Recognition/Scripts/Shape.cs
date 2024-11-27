using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OmmyShapeML
{
public class Enemy : MonoBehaviour
{
    public Shapes shapeType;
    public State currentState;
    public SpriteRenderer shapeSprite;
    public GameObject body;

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
}