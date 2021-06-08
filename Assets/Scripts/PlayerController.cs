using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController current;
    public static PlayerHealth health;
    Ghost playerGhost;

    public static float moveDirection;
    [SerializeField] float moveSpeed;

    [Header("Jump Scales")]
    [SerializeField] float jumpForce = 5f; // Force of the player's jump

    [SerializeField] float defaultGravityScale = 1f;
    [SerializeField] float jumpFallMultiplier = 5f; // Lowers the amount of time the jump has to stop
    [SerializeField] float jumpReleaseMultiplier = 2f; // Multiplies fall speed when jump buttons is released

    [SerializeField] LayerMask groundedMask;

    [Header("Dash Scales")]
    [SerializeField] float dashSpeed = 7f; // Dash Movement Speed
    [SerializeField] float dashLength = 1f; // Length of dash in seconds

    [Header("Shooting")]
    [SerializeField] GameObject[] bullets;
    [SerializeField] Transform shootPoint;
    [SerializeField] float chargeWaitTime;
    [SerializeField] float shotCooldown;
    [SerializeField] ParticleSystem[] chargeParticles;

    [Header("Being Damaged")]
    [SerializeField] float knockbackSpeed = 2f, knockbackTime = 0.5f;
    [SerializeField] float iFrameTime = 1f;

    Rigidbody2D rb2d;
    SpriteAnimator spriteAnimator;
    SpriteRenderer spriteRenderer;
    BoxCollider2D playerCollider;
    Animator animator;
    ObjectAudioManager audioManager;

    float groundedSkin = 0.05f, subtractFromBoxSize = 0.05f;
    Vector2 playerSize, colliderOffset, boxSize;

    float attackAnim1Time, attackAnim2Time, attackAnim3Time;
    int currentAttack = 1;
    [SerializeField] int groundedAttacks; // The amount of grounded attacks

    // Animation states
    const string PLAYER_RUN = "Player_Run", PLAYER_IDLE = "Player_Idle", PLAYER_SHOOT_IDLE = "Player_Shoot_Idle",
    PLAYER_SHOOT_RUN = "Player_Shoot_Run", PLAYER_JUMP = "Player_Jump", PLAYER_FALL = "Player_Fall", PLAYER_ATTACK = "Player_Attack";

    [HideInInspector] public bool isGrounded, isRebounding, ableToDash = true, isDashing, isCharging, isAttacking, isHurt, isInvincible;

    void Start()
    {
        current = this;
        health = GetComponent<PlayerHealth>();

        rb2d = GetComponent<Rigidbody2D>();
        spriteAnimator = GetComponent<SpriteAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerCollider = GetComponent<BoxCollider2D>();

        // Ground Detection
        playerSize = playerCollider.size;
        colliderOffset = playerCollider.offset;
        boxSize = new Vector2(playerSize.x - subtractFromBoxSize, groundedSkin);

        playerGhost = GetComponent<Ghost>();
        animator = GetComponent<Animator>();
        audioManager = GetComponent<ObjectAudioManager>();

        UpdateAnimClipTimes();
    }

    private void Update()
    {

        // Change facing scale based on direction
        if (moveDirection != 0)
            transform.localScale = new Vector3(moveDirection < 0? -1 : 1, 1, 1);


        // Move and Idle
        if (moveDirection != 0 && !isDashing && !isHurt)
            Move(moveDirection);
        else if (!isDashing && !isHurt)
        {
            rb2d.velocity = new Vector2(0,rb2d.velocity.y);
            if (isGrounded && !isAttacking)
            {
                spriteAnimator.ChangeAnimationState(isCharging? "Player_Shoot_Idle" : "Player_Idle");
            }
                
        }

        // Dash reset logic
        if (!isDashing && isGrounded && !isHurt)
            ableToDash = true;
        else if (!isGrounded && !isDashing)
            ableToDash = false;


        // Grounded Detection
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;

        if (!isRebounding)
            // Check to see if player is grounded   
            isGrounded = Physics2D.OverlapBox(boxCenter + colliderOffset, boxSize, 0f, groundedMask) != null;


        // Air physics
        if (rb2d.velocity.y < 0 && !isGrounded) // Jump physics
        {
            rb2d.gravityScale = jumpFallMultiplier;
            spriteAnimator.ChangeAnimationState(PLAYER_FALL);
        }

        else if (!isRebounding && (rb2d.velocity.y > 0 && !InputManager.jumpHeld && !isGrounded))
            // if player is rebounding, this will not be applied, as it will cut off the jump arc
            rb2d.gravityScale = jumpReleaseMultiplier;

        else
        {
            rb2d.gravityScale = defaultGravityScale;
            if (!isGrounded)
                spriteAnimator.ChangeAnimationState(PLAYER_JUMP);    
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {   
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Walljump if jump button is pressed
            isGrounded = false;
            isAttacking = false;
        }
    }

    public void Move(float direction)
    {
        float runTime;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(PLAYER_RUN) || !animator.GetCurrentAnimatorStateInfo(0).IsName(PLAYER_SHOOT_RUN))
            runTime = 0;

        if (isGrounded && !isAttacking)
        {
            AnimatorStateInfo runState = animator.GetCurrentAnimatorStateInfo(0);
            runTime = runState.normalizedTime % 1;

            spriteAnimator.ChangeAnimationState(isCharging? PLAYER_SHOOT_RUN : PLAYER_RUN, runTime);
        }
        
        rb2d.velocity = new Vector2(direction * moveSpeed, rb2d.velocity.y);
    }

    public IEnumerator Dash()
    {
        if (isGrounded)
        {
            float startDirection = moveDirection;
            float dashTime = Time.time + dashLength;
            isDashing = true;
            playerGhost.makeGhost = true;
            audioManager.Play("Dash");

            while(!isGrounded ||(Time.time < dashTime && ableToDash && startDirection == moveDirection))
            {
                // Decrease speed to the original movespeed over time, making dashes smoother
                float currentDashVelocity = (dashSpeed * moveDirection * ((dashTime - Time.time) * 2));
                if (currentDashVelocity <= 0 && moveDirection > 0)
                    currentDashVelocity = 0;
                else if (currentDashVelocity >= 0 && moveDirection < 0)
                    currentDashVelocity = 0;

                rb2d.velocity = new Vector2(currentDashVelocity + ((moveSpeed + (dashSpeed /2)) * moveDirection), rb2d.velocity.y);
                    
                yield return new WaitForEndOfFrame();
            }

            isDashing = false;
            ableToDash = true;
            playerGhost.makeGhost = false;
        }
    }

    void StopDash() 
    {
        StopCoroutine(Dash());
        isDashing = false;
        ableToDash = true;
        playerGhost.makeGhost = false;
    }

    public void Shoot(int bulletIndex)
    {
        audioManager.Play("Shoot" + bulletIndex);
        Instantiate(bullets[bulletIndex], shootPoint.position, shootPoint.rotation);
    }

    public IEnumerator Charge()
    {
        bool playedSound = false;
        int chargeLevel = 0;
        float nextCooldownTime = Time.time + shotCooldown;
        float nextChargeTime = Time.time + chargeWaitTime;

        while (isCharging)
        {
            if (Time.time >= nextCooldownTime && !playedSound)
            {
                audioManager.Play("Charge");
                playedSound = true;
            }

            if(Time.time >= nextChargeTime)
            {
                if (chargeLevel != bullets.Length -1)
                {    
                    chargeLevel ++;
                    nextChargeTime = Time.time + chargeWaitTime;


                }
            }

            yield return new WaitForEndOfFrame();
        }

        audioManager.Stop("Charge");
        
        
        if (Time.time >= nextCooldownTime)
            Shoot(chargeLevel);

    }

    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "Player_Attack_1":
                    attackAnim1Time = clip.length;
                    break;
                case "Player_Attack_2":
                    attackAnim2Time = clip.length;
                    break;
                case "Player_Attack_3":
                    attackAnim3Time = clip.length;
                    break;
            }
        }
    }

    public IEnumerator Attack()
    {
        if (currentAttack > groundedAttacks)
            currentAttack = 1;

        float animTime = 0f;
        isAttacking = true;

        // TODO: Replace with current attack animation
        spriteAnimator.ChangeAnimationState(PLAYER_ATTACK + "_" + currentAttack);

        // TODO: Use a switch statement to choose which time is used in this while loop
        while(animTime <= attackAnim1Time)
        {
            animTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        isAttacking = false;
        currentAttack ++;
    }

    public IEnumerator Hurt()
    {
        StopDash();
        // Knockback
        isInvincible = true;
        isHurt = true;
        float currentKnockbackTime = Time.time + knockbackTime;

        while(Time.time < currentKnockbackTime)
        {
            rb2d.velocity = new Vector2 (-transform.localScale.x * knockbackSpeed, 0);
            yield return new WaitForEndOfFrame();
        }

        isHurt = false;

        // I Frames
        float currentIFrameTime = Time.time + iFrameTime;
        while(Time.time < currentIFrameTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}
