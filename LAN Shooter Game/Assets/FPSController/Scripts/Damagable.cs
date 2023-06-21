using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField] float health;
    public float maxHealth;

    public void Damage(float amt)
    {
        health -= amt;
        if (health <= 0) { Die(); }
    }

    public void Heal(float amt)
    {
        health += amt;
        if (health > maxHealth) { health = maxHealth; }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
