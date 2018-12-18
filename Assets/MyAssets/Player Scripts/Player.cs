﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    // Private Enums
    private enum AttackStyle { Horizontal, Vertical };

    [Header("Stats Reference")]
    public UnitStats stats;

    [Header("Player Inventory Vars")]
    [HideInInspector] public PlayerInventory pInven;

    [Header("Player Combat Variables")]
    public LayerMask enemyLayer;
    public LayerMask wallLayer;
    [SerializeField] private AttackStyle aStyle;
    public Vector3 attackAreaOffset;
    public Vector3 attackArea;

    [SerializeField] private float length = 2.5f;
    [SerializeField] private float width = 1f;

    [Header("Input Variables")]
    public KeyCode attackKey;

    /// <summary>
    /// Getting References
    /// </summary>
    private void Awake()
    {
        // Getting the PlayerInventory Component
        pInven = GetComponent<PlayerInventory>();

        // Creating a new reference to Stats
        stats = new UnitStats(gameObject);
    }

    private void Update()
    {
        // Checking for Attack Input
        if (Input.GetKeyDown(attackKey))
        {
            TryMeleeAttack();
        }
    }

    private void AdjustAttackArea()
    {
        // Adjusting the AttackAreaOffset based on velocity
        Vector3 lastVelocity = GetComponent<UnitMovement>().lastVelocity;
        if (lastVelocity.x != 0f)
        {
            aStyle = AttackStyle.Horizontal;
            attackArea = new Vector3(length, width, 0f);
            attackAreaOffset = new Vector3(((length / 2) * Mathf.Sign(lastVelocity.x)) + ((transform.localScale.x / 2) * Mathf.Sign(lastVelocity.x)), 0f, 0f);
        }
        else
        {
            aStyle = AttackStyle.Vertical;
            attackArea = new Vector3(width, length, 0f);
            attackAreaOffset = new Vector3(0f, ((length / 2) * Mathf.Sign(lastVelocity.y)) + ((transform.localScale.y / 2) * Mathf.Sign(lastVelocity.y)), 0f);
        }
        
    }

    private void TryMeleeAttack()
    {
        //Readjust the Attack Area based on movement
        AdjustAttackArea();

        // Searching for a target
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + attackAreaOffset, attackArea, 0f, Vector2.left, 0f, enemyLayer);
        if (hit)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();

            if (enemy == null)
            {
                Debug.Log("Hit an Object on enemyLayer but couldn't find Enemy Reference");
                return;
            }

            // Dealing the Damage to the Unit
            enemy.stats.TakeDamage(stats.unitDamage);
        }
    }

    /// <summary>
    /// Drawing the Attack Area
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Drawing the Attack Zone
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position + attackAreaOffset, attackArea);
    }

}
