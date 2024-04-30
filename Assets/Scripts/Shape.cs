using System;
using System.Collections;
using System.Collections.Generic;
using ShapeInputs;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Serializable]
    public class ShapeData
    {
        public int shapeHealth=1;
        public void ShapeDrawed()
        {
            shapeHealth--;
            if(shapeHealth <= 0)
            shapeSprite.gameObject.SetActive(false);
        }
        public Shapes shapeType;
        public SpriteRenderer shapeSprite;
    }
    public List<EnemyBehavior> enemyBehaviors;
    public List<ShapeData> shapeDatas;
    public State currentState;
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
}
    public enum State
    {
        Idle,
        Run,
        Agressive,
        Attack,
        Kill
    }
