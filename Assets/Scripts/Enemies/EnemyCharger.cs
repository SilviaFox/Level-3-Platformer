using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharger : MonoBehaviour
{
    enum ChargerState
    {
        Idle,
        Jumping,
        Charging,
        Walking,
        Stunned,
        Dying
    }

    [SerializeField] Transform jumpPoint;
    [SerializeField] LayerMask groundMask;

    [Header("Walk")]
    [SerializeField] float walkSpeed = 4;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float jumpSpeed = 8f;

    [Header("Charging")]
    [SerializeField] float chargeBuildupTime = 1f;
    [SerializeField] float chargeLength = 4f;
    [SerializeField] float chargeSpeed = 8f;

    [Header("Stun")]
    [SerializeField] float stunTime = 2f;
    [SerializeField] float rageOnHit = 0.01f;

    [Header("Damage")]
    [SerializeField] int damage = 10;

    float rage;
    bool isCharging = false, isStunned = false;
    bool activated = false;
    bool isDead;
    

    Rigidbody2D rb2d;
    SpriteAnimator animator;
    Ghost ghost;
    ChargerState chargerState = ChargerState.Idle;

    private void OnBecameVisible()
    {
        if (!activated)
        {
            chargerState = ChargerState.Walking;
            activated = true;
        }
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ghost = GetComponent<Ghost>();
        animator = GetComponent<SpriteAnimator>();
    }

    private void Update()
    {

        switch(chargerState)
        {
            case ChargerState.Idle:
            break;

            case ChargerState.Jumping:
                Jump();
            break;

            case ChargerState.Charging:
                if (!isCharging)
                    StartCoroutine(Charge());
            break;

            case ChargerState.Walking:
                Walk();        
            break;

            case ChargerState.Stunned:
                if (!isStunned && !isDead)
                    StartCoroutine(Stun());
                else if (isDead)
                    chargerState = ChargerState.Dying;
            break;
            case ChargerState.Dying:
                rb2d.velocity = new Vector2 (0, rb2d.velocity.y);
            break;
        }
    }

    void Walk()
    {
        animator.ChangeAnimationState("Enemy_Charger_Walk");
        float chargeRage = (float)System.Math.Round(Random.Range(0.1f, 50f), 1);

        transform.localScale = new Vector3(PlayerController.current.transform.position.x > transform.position.x? 1 : -1, 1);

        rb2d.velocity = new Vector2(walkSpeed * transform.localScale.x, rb2d.velocity.y);
        
        if (!Physics2D.Raycast(jumpPoint.position, Vector2.down, 0.5f, groundMask))
        {
            chargerState = ChargerState.Jumping;
            rb2d.AddForce(new Vector2(jumpSpeed * transform.localScale.x, jumpHeight), ForceMode2D.Impulse);
        }

        if (chargeRage < rage && chargerState == ChargerState.Walking)
        {
            Debug.Log("Random Trigger - Rage: " + rage + " Charge: " + chargeRage);
            chargerState = ChargerState.Charging;
        }
        
    }

    void Jump()
    {
        if (Physics2D.Raycast(jumpPoint.position, Vector2.down, 0.5f, groundMask))
        {
            chargerState = ChargerState.Walking;
        }
    }

    IEnumerator Charge()
    {
        animator.ChangeAnimationState("Enemy_Charger_Charge");
        isCharging = true;
        rb2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(chargeBuildupTime);

        StartCoroutine(ghost.MakeGhosts());
        float chargeTime = Time.time + chargeLength;

        while (chargeTime > Time.time)
        {
            rb2d.velocity = new Vector2(chargeSpeed * transform.localScale.x, rb2d.velocity.y);
            yield return new WaitForEndOfFrame();
        }

        chargerState = ChargerState.Walking;
        ghost.makeGhost = false;
        isCharging = false;
    }

    IEnumerator Stun()
    {
        isStunned = true;
        rb2d.velocity = Vector2.zero;
        rage += rageOnHit;

        yield return new WaitForSeconds(stunTime);
        isStunned = false;
        Debug.Log("Stun Trigger");
        chargerState = ChargerState.Charging;
    }

    public void OnHit()
    {
        chargerState = ChargerState.Stunned;
    }

    // Hurt
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayerController.health.TakeDamage(damage)); 
        }

    }

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        chargerState = ChargerState.Dying;
    }


}
