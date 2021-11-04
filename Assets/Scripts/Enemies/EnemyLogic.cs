using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyLogic : MonoBehaviour
{

    [SerializeField] float health;
    [SerializeField] BoxCollider2D hitBox;

    [SerializeField] UnityEvent onMeleeDamage;
    [SerializeField] UnityEvent onShootDamage;

    [SerializeField] UnityEvent onKill;
    [SerializeField] GameObject[] coins;
    bool invunerable;
    bool isDead;

    void Start()
    {
        if (health == 0)
            invunerable = true;
    }
    
    public void TakeMeleeDamage(float damage)
    {
        if (!isDead)
        {
            onMeleeDamage.Invoke();
            if (PlayerController.current.allowRebound)
                PlayerController.current.Rebound();

            TakeDamage(damage);
        }
    }

    public void TakeShootDamage(float damage)
    {
        if (!isDead)
        {
            onShootDamage.Invoke();

            TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        for (int i = 0; i < coins.Length / (damage > 30? 2 : 4); i++)
        {
            Instantiate(coins[i], transform.position, new Quaternion());
        }

        if (health <= 0 && !invunerable)
            StartCoroutine(Kill());
    }

    IEnumerator Kill()
    {
        isDead = true;
        DisableHitbox();
        onKill.Invoke();

        foreach (GameObject coin in coins)
        {
            Instantiate(coin, transform.position, new Quaternion());
            yield return null;
        }
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("PlayerHit"))
        {
            TakeMeleeDamage(PlayerController.currentAttackDamage);
            PlayerController.audioManager.Play("AttackHit");
        }
        else if (other.CompareTag("PlayerProjectile"))
        {
            TakeShootDamage(other.GetComponent<BulletLogic>().damage);
            Destroy(other.gameObject);
        }
            
    }

    public void EnableHitbox()
    {
        hitBox.enabled = true;
    }

    public void DisableHitbox()
    {
        hitBox.enabled = false;
    }
    
}
