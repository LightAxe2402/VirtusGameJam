﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit Assets/New Unit", order = 1)]
public class UnitStats : ScriptableObject
{
    [Header("Default Values")]
    public float defHealth;
    public float defStr;

    [Header("Combat Variables")]
    public float unitHealth;
    public float unitMaxHealth;

    public float unitDamage;
    
    public UnitStats(float health = 5f, float damage = 1f)
    {
        unitMaxHealth = unitHealth = health;
        unitDamage = damage;

        Debug.Log("Unit Initialized at " + health.ToString() + " health and " + damage.ToString("F0") + " damage");
    }

    /// <summary>
    /// Takes Damage and checks for death
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(GameObject target, float damage)
    {
        // Reporting the amount of damage unit has taken
        Debug.Log(target.name + " Takes " + damage.ToString("F0") + " damage");

        // Assigning the Damage
        unitHealth -= damage;

        Debug.Log(target.name + " has " + unitHealth.ToString("F0") + " left");

        // Checking for death
        if (unitHealth <= 0f)
        {
            // Checking for a player Object
            if (target.tag == "Player")
            {
                target.GetComponent<Player>().Respawn();
            }

            // Destroying the GameObject
            GameObject.Destroy(target);
        }
    }

    public void Reset()
    {
        unitHealth = unitMaxHealth = defHealth;
        unitDamage = defStr;
    }
}
