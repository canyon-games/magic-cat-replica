using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int health;
    public int currentHealth;
    public Transform healthBar;
    public static PlayerController instance;
    private void Awake() 
    {
        currentHealth=health;
        if (instance == null) 
        {
            instance = this;
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth-=damage;
        print("Set health "+ (float)(currentHealth/health));
        healthBar.localScale = new Vector3(1,(float)currentHealth/(float)health,1);
        print("current health "+currentHealth);
        if (currentHealth <= 0)
        {
            EventManager.OnLevelFail.Invoke();
        }
    }
}
