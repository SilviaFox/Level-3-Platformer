using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagePlayerOnContact : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] UnityEvent onDamage;

    // Hurt
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayerController.health.TakeDamage(damage)); 
            onDamage.Invoke();
        }

    }
}
