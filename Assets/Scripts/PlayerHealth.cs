using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] int maxHealth;
    [SerializeField] Slider healthBar;
    [HideInInspector] public int visualHealth;
    int health;
    Vector3 healthBarPosition;

    void Start()
    {
        health = maxHealth;
        visualHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;

        healthBarPosition = healthBar.transform.position;
    }

    private void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            StartCoroutine(TakeDamage(50));
        }
        
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            StartCoroutine(Heal(10));
        }

    }

    public IEnumerator TakeDamage(int damage)
    {
        if (!PlayerController.current.isInvincible)
        {
            health -= damage;
            StartCoroutine(PlayerController.current.Hurt());
            while (health != visualHealth)
            {
                healthBar.transform.position += new Vector3(Random.Range(-7,7),Random.Range(-7,7));

                visualHealth --;
                healthBar.value = visualHealth;
                yield return new WaitForEndOfFrame();
            }

            healthBar.transform.position = healthBarPosition;

            if (damage <= 0)
                Debug.Log("die lol");
        }
    }

    public IEnumerator Heal(int healAmount)
    {
        health += healAmount;

        if (health > maxHealth)
            health = maxHealth;

        while (health != visualHealth)
        {
            
            visualHealth ++;

            healthBar.value = visualHealth;
            yield return new WaitForEndOfFrame();
        }
    }

}
