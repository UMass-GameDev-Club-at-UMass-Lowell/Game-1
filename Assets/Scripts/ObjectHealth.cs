using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectHealth : MonoBehaviour
{
    public Animator animator;
    public Transform hurtEffect;

    public bool IsDead { get { return currentHealth <= 0; } }
    public bool enableDamageParticles = true;

    public int maxHealth = 100;
    int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(Transform attacker, int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        Collider2D objectCollider = transform.GetComponent<Collider2D>();

        // generate hurt particles (if enabled)
        if (enableDamageParticles)
        {
            Transform hurtPrefab = Instantiate(hurtEffect,
                    objectCollider.bounds.center,
                    Quaternion.identity);
            hurtPrefab.up = new Vector3(attacker.position.x - objectCollider.transform.position.x, 0f, 0f);
        }

        if (currentHealth <= 0)
            Die();
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    void Die()
    {
        animator.SetBool("IsDead", true);
        if (gameObject.name == "Player")
        {
            AudioManager.Instance.PlaySFX("player_death");
        }
        else if (gameObject.name.Contains("Breakable Pot"))
        {
            AudioManager.Instance.PlaySFX("breakable_pot_destroy");
        }

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        this.enabled = false;
    }

}
