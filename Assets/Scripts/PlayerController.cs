using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool healthBarSystem;
    public GameObject[] healths;
    public int health;
    public int currentHealth;
    public Transform healthBar;
    public Animator animator;
    public static PlayerController instance;
    private void Awake() 
    {
        //animator.speed=.4f;
        currentHealth=health;
        if (instance == null) 
        {
            instance = this;
        }
    }
    public void SetAnimatorBool(string trigger,bool value=true)
    {
        //if(trigger=="Scare"&&value)animator.speed=0.4f;
        //else animator.speed=1;
        animator.SetBool(trigger,value);
    }
    public void SetAnimatorTrigger(string trigger)
    {
        //animator.speed=1;
        animator.SetTrigger(trigger);
    }
    public void TakeDamage(int damage)
    {
        AudioManager.Instance.PlaySFX(SFX.Attack);
        SetAnimatorTrigger("Hit");
        foreach (var item in healths)
        {
            if(item.activeInHierarchy)
            {
                item.SetActive(false);
                break;
            }
        }
        currentHealth-=damage;
        print("Set health "+ (float)(currentHealth/health));
        healthBar.localScale = new Vector3(1,(float)currentHealth/(float)health,1);
        if(!healths[healths.Length-1].activeInHierarchy&&!healthBarSystem)
        {
            EventManager.OnLevelFail.Invoke();
        }
        else if (currentHealth <= 0)
        {
            EventManager.OnLevelFail.Invoke();
        }
        print("current health "+currentHealth);
    }
}
