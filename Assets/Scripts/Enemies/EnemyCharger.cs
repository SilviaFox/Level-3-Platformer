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
    [SerializeField] float jumpVelocityCap = 10;

    [Header("Charging")]
    [SerializeField] float chargeBuildupTime = 1f;
    [SerializeField] float chargeLength = 4f;
    [SerializeField] float chargeSpeed = 8f;

    [Header("Stun")]
    float stunTime = 0;
    [SerializeField] float rageOnHit = 0.01f;

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

    private void OnBecameInvisible()
    {        
        if (activated)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        ghost = GetComponent<Ghost>();
        animator = GetComponent<SpriteAnimator>();
        UpdateAnimClipTimes();
    }

    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = GetComponent<Animator>().runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "Enemy_Charger_Stun":
                    stunTime = clip.length;
                break;
            }
        }
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

    bool CheckFloor()
    {
        if (!Physics2D.Raycast(jumpPoint.position, Vector2.down, 0.4f, groundMask) || Physics2D.Raycast(jumpPoint.position, Vector2.right * transform.localScale.x, 3f, groundMask) || Physics2D.Raycast(jumpPoint.position, Vector2.right * transform.localScale.x + Vector2.up, 3f, groundMask))
            return true;

        return false;
    }

    void Walk()
    {
        animator.ChangeAnimationState("Enemy_Charger_Walk");
        float chargeRage = (float)System.Math.Round(Random.Range(0.1f, 50f), 1);

        rb2d.velocity = new Vector2(walkSpeed * transform.localScale.x, rb2d.velocity.y);
        
        if (CheckFloor() && rb2d.velocity.y < jumpVelocityCap)
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
        rb2d.velocity = new Vector2(walkSpeed * transform.localScale.x, rb2d.velocity.y);
        if (Physics2D.Raycast(jumpPoint.position, Vector2.down, 0.4f, groundMask))
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
            if (Physics2D.Raycast(jumpPoint.position, Vector2.down, 0.4f, groundMask) && CheckFloor() && rb2d.velocity.y < jumpVelocityCap)
            {
                rb2d.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            }
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
        chargerState = ChargerState.Charging;
    }

    public void OnHit()
    {
        if (chargerState != ChargerState.Charging)
        {
            animator.ChangeAnimationState("Enemy_Charger_Stun", 0, true);
            chargerState = ChargerState.Stunned;
        }
    }

  

    public void Die()
    {
        isDead = true;
        StopAllCoroutines();
        chargerState = ChargerState.Dying;
    }


}
