﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {

    // Private Enums
    private enum AttackStyle { Horizontal, Vertical };

    [Header("Stats Reference")]
    public LevelManager manager;
    public UnitState pState = new UnitState();

    [Header("GUI References")]
    public Image playerHealthBar;

    [Header("Player Combat Variables")]
    public LayerMask enemyLayer;
    public LayerMask wallLayer;
    [SerializeField] private AttackStyle aStyle;
    public Vector3 attackAreaOffset;
    public Vector3 attackArea;

    [SerializeField] private float length = 2.5f;
    [SerializeField] private float width = 1f;

    [Header("Input Variables")]
    private UnitMovement playerMovement;
    public KeyCode attackKey;

    private Animator anim;

    /// <summary>
    /// Getting References
    /// </summary>
    private void Awake()
    {
        playerMovement = GetComponent<UnitMovement>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Checking for Attack Input
        if (Input.GetKeyDown(attackKey))
        {
            TryMeleeAttack();
        }

        // Updating the GUI
        UpdatePlayerGUIElements();

        // Updating the Player Sprite
        UpdatePlayerSprite();
    }

    private void UpdatePlayerSprite()
    {
        anim.ResetTrigger("UpMove");
        anim.ResetTrigger("DownMove");
        anim.ResetTrigger("LeftMove");
        anim.ResetTrigger("RightMove");

        if (Input.GetKey(KeyCode.W))
        {
            anim.SetTrigger("UpMove");
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            anim.SetTrigger("LeftMove");
            return;
        }

        if (Input.GetKey(KeyCode.S))
        {
            anim.SetTrigger("DownMove");
            return;
        }

        if (Input.GetKey(KeyCode.D))
        {
            anim.SetTrigger("RightMove");
            return;
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
        // Read just the Attack Area based on movement
        AdjustAttackArea();

        // Searching for a target
        Vector3 lastVelocity = GetComponent<UnitMovement>().lastVelocity;
        Vector3 attackVector = new Vector3(((length / 2) * Mathf.Sign(lastVelocity.x)) + ((transform.localScale.x / 2) * Mathf.Sign(lastVelocity.x)),
                                           ((width / 2) * Mathf.Sign(lastVelocity.y)) + ((transform.localScale.y / 2) * Mathf.Sign(lastVelocity.y)),
                                           0f);

        float a = Vector3.Angle(transform.position, transform.position + attackVector);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + attackAreaOffset, attackArea, a, Vector2.left, 0f, enemyLayer);
        if (hit)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();

            if (enemy == null)
            {
                Debug.Log("Hit an Object on enemyLayer but couldn't find Enemy Reference");
                return;
            }

            // Dealing the Damage to the Unit
            enemy.sTracker.TakeDamage(enemy.gameObject, manager.playerStats.unitDamage);
            enemy.StartCoroutine("HurtFlash");
        }
    }

    IEnumerator HurtFlash()
    {
        // Getting the SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color baseColor = sr.color;
        Color flashColor = Color.white;
        int flashReps = 6;
        float delay = 0.05f;

        for (int i = 0; i < flashReps; i++)
        {
            sr.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Updates all GUI References
    /// </summary>
    private void UpdatePlayerGUIElements()
    {
        // Updating the HealthBar
        playerHealthBar.fillAmount = (manager.playerStats.unitHealth / manager.playerStats.unitMaxHealth);
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
