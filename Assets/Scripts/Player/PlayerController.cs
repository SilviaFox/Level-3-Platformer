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
    [SerializeField] float shotCooldown, gunHideTime;
    [SerializeField] ParticleSystem[] chargeParticles;
    [SerializeField] SpriteAnimator hand;
    float nextCooldownTime;

    [Header("Being Damaged")]
    [SerializeField] float knockbackSpeed = 2f;
    [SerializeField] float knockbackTime = 0.5f;
    [SerializeField] float iFrameTime = 1f;
    [SerializeField] BoxCollider2D hurtBox;

    Rigidbody2D rb2d;
    SpriteAnimator spriteAnimator;
    SpriteRenderer spriteRenderer;
    BoxCollider2D playerCollider;
    Animator animator;
    public static ObjectAudioManager audioManager;

    float groundedSkin = 0.1f, subtractFromBoxSize = 0.05f;
    Vector2 playerSize, colliderOffset, boxSize;
    
    // Attacks
    float attackAnim1Time, attackAnim2Time, attackAnim3Time, attackAirTime;
    float attackResetTime;
    int currentAttack = 1;
    [Header("Melee Attacks")]
    [SerializeField] int groundedAttacks; // The amount of grounded attacks
    [SerializeField] float[] attackDamages;
    [SerializeField] Vector2[] attackForces;
    [SerializeField] float airAttackDamage;
    [SerializeField] Vector2 airAttackForce;
    [SerializeField] float attackResetBuffer;
    
    public static float currentAttackDamage;
    public static Vector2 currentAttackForce;

    [Header("Rebound")]
    [SerializeField] float reboundForce;
    [HideInInspector] public bool allowRebound;

    [Header("Physics")]
    [SerializeField] float groundedForce = 4;
    [SerializeField] PhysicsMaterial2D idlePhysMat;
    [SerializeField] PhysicsMaterial2D movingPhysMat;

    [Header("Game Feel")]
    [SerializeField] ParticleSystem dustParticles;

    // Animation states
    const string PLAYER_RUN = "Player_Run", PLAYER_IDLE = "Player_Idle", PLAYER_SHOOT_IDLE = "Player_Shoot_Idle", PLAYER_DASH = "Player_Dash",
    PLAYER_SHOOT_RUN = "Player_Shoot_Run", PLAYER_JUMP = "Player_Jump", PLAYER_FALL = "Player_Fall", PLAYER_ATTACK = "Player_Attack", PLAYER_ATTACK_AIR = "Player_Attack_Air";

    [HideInInspector] public bool isGrounded, isRebounding, ableToDash = true, isDashing, isCharging, isAttacking, isHurt, isInvincible, isShooting;

    void Awake()
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
        bool groundedThisFrame = isGrounded;

        bool wallCheck = Physics2D.Raycast(transform.position - new Vector3(0,0.5f),Vector2.right * transform.localScale.x, 0.55f, groundedMask);

        if ((nextCooldownTime + gunHideTime < Time.time && !isCharging) || isAttacking)
            isShooting = false;
        else
            hand.ChangeAnimationState("Hand_Shoot");

        // Change facing scale based on direction
        if (moveDirection != 0)
            transform.localScale = new Vector3(moveDirection < 0? -1 : 1, 1, 1);

        rb2d.sharedMaterial = moveDirection == 0 && !wallCheck && isGrounded? idlePhysMat : movingPhysMat;


        // Move and Idle
        if (moveDirection != 0 && !isDashing && !isHurt && !wallCheck)
            Move(moveDirection);
        else if (!isDashing && !isHurt)
        {
            rb2d.velocity = new Vector2(0,rb2d.velocity.y);
            if (isGrounded)
            {
                if (!isAttacking)
                    spriteAnimator.ChangeAnimationState(PLAYER_IDLE, 2, true);

                if (!isShooting)
                    hand.ChangeAnimationState("Hand_Idle");
            }
            
                
        }

        // Dash reset logic
        if (!isDashing && isGrounded && !isHurt)
            ableToDash = true;
        else if (!isGrounded && !isDashing)
            ableToDash = false;
    

        // Grounded Detection
        // Get the location the box should be cast at
        Vector2 boxCenter = (Vector2)transform.position + Vector2.down * (playerSize.y + boxSize.y) * 0.5f;
        // Check to see if player is grounded unless travelling upwards  
        if (rb2d.velocity.y <= 0.1f)
            isGrounded = Physics2D.OverlapBox(boxCenter + colliderOffset, boxSize, 0f, groundedMask) != null;

        // Keep player grounded
        if (isGrounded)
            rb2d.velocity = new Vector2(rb2d.velocity.x, -groundedForce);

        // If grounded this frame, generate dust particles
        if (groundedThisFrame != isGrounded)
        {
            dustParticles.Play();
            if (isGrounded)
                audioManager.Play("Land");
        }

        if (isGrounded || isAttacking)
        {
            isRebounding = false;
            audioManager.Stop("Rebound");
        }
        // Air physics
        if (rb2d.velocity.y < 0 && !isGrounded) // Jump physics
        {
            rb2d.gravityScale = jumpFallMultiplier;
            if (!isAttacking && !isRebounding && !isHurt)
                spriteAnimator.ChangeAnimationState(PLAYER_FALL);
                if (!isShooting)
                    hand.ChangeAnimationState("Hand_Fall");
        }

        else if (!isRebounding && (rb2d.velocity.y > 0 && !InputManager.jumpHeld && !isGrounded))
            // if player is rebounding, this will not be applied, as it will cut off the jump arc
            rb2d.gravityScale = jumpReleaseMultiplier;

        else
        {
            rb2d.gravityScale = defaultGravityScale;
            if (!isGrounded && !isAttacking && !isRebounding && !isHurt)
            {
                spriteAnimator.ChangeAnimationState(PLAYER_JUMP);   
                if (!isShooting)
                    hand.ChangeAnimationState("Hand_Jump");
            }

        }

        if (isGrounded)
            rb2d.velocity = rb2d.velocity + Vector2.down * 5;
    }

    // Jumping
    public void Jump()
    {
        if (isGrounded)
        {   
            rb2d.AddForce(Vector2.up * (jumpForce - rb2d.velocity.y), ForceMode2D.Impulse);
            isGrounded = false;
            isAttacking = false;
            dustParticles.Play();
        }
    }

    // Movement
    public void Move(float direction)
    {
        

        if (isGrounded && !isAttacking && !isDashing)
        {
            AnimatorStateInfo runState = animator.GetCurrentAnimatorStateInfo(0);
            float runTime = runState.normalizedTime % 1;

            spriteAnimator.ChangeAnimationState(PLAYER_RUN, runTime);
            if (!isShooting)
                hand.ChangeAnimationState("Hand_Run", runTime);
        }
        
        rb2d.velocity = new Vector2(direction * moveSpeed, rb2d.velocity.y);
    }

    public IEnumerator Dash()
    {
        bool leftGround = false;

        if (isGrounded && moveDirection != 0)
        {
            float startDirection = moveDirection;
            float dashTime = Time.time + dashLength;
            isDashing = true;
            StartCoroutine(playerGhost.MakeGhosts());
            audioManager.Play("Dash");
            spriteAnimator.ChangeAnimationState(PLAYER_DASH);
            if (!isShooting)
                hand.ChangeAnimationState("Hand_Dash");
            dustParticles.Play();

            while(!isGrounded ||(Time.time < dashTime && ableToDash && startDirection == moveDirection))
            {
                if (leftGround && isGrounded)
                    StopDash();

                // Decrease speed to the original movespeed over time, making dashes smoother
                float currentDashVelocity = (dashSpeed * moveDirection * ((dashTime - Time.time) * 2));
                if (currentDashVelocity <= 0 && moveDirection > 0)
                    currentDashVelocity = 0;
                else if (currentDashVelocity >= 0 && moveDirection < 0)
                    currentDashVelocity = 0;

                if (!isGrounded)
                    leftGround = true;

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
        if (Time.time >= nextCooldownTime && !isAttacking)
        {
            isShooting = true;
            hand.ChangeAnimationState("Hand_Shoot", 0, true);
            audioManager.Play("Shoot" + bulletIndex);
            Instantiate(bullets[bulletIndex], shootPoint.position, shootPoint.rotation);
            nextCooldownTime = Time.time + shotCooldown;
        }
    }

    public IEnumerator Charge()
    {
        bool playedSound = false;
        int chargeLevel = 0;
        float nextChargeTime = Time.time + chargeWaitTime;

        while (isCharging)
        {
            if (Time.time >= nextCooldownTime && !playedSound)
            {
                chargeParticles[chargeLevel].Play();
                audioManager.Play("Charge");
                playedSound = true;
            }

            if(Time.time >= nextChargeTime)
            {
                if (chargeLevel != bullets.Length -1)
                {    
                    chargeParticles[chargeLevel].Stop();
                    chargeLevel ++;
                    chargeParticles[chargeLevel].Play();
                    nextChargeTime = Time.time + chargeWaitTime;
                }
            }

            yield return new WaitForEndOfFrame();
        }
        
        chargeParticles[chargeLevel].Stop();
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
                case "Player_Attack_Air":
                    attackAirTime = clip.length;
                    break;
            }
        }
    }

    public IEnumerator Attack()
    {
        if (!isAttacking && !isHurt)
        {

            audioManager.Play("Attack");
            if (attackResetTime < Time.time || currentAttack > groundedAttacks)
                currentAttack = 1;

            float attackAnimTime = 0;
            isAttacking = true;
            hurtBox.enabled = false;

            if (isGrounded)
                {
                    allowRebound = false;
                    currentAttackDamage = attackDamages[currentAttack - 1];
                    currentAttackForce = attackForces[currentAttack - 1];
                    spriteAnimator.ChangeAnimationState(PLAYER_ATTACK + "_" + currentAttack);

                    switch (currentAttack)
                    {
                        case 1:
                            attackAnimTime = attackAnim1Time;
                        break;

                        case 2:
                            attackAnimTime = attackAnim2Time;
                        break;
                        case 3:
                            attackAnimTime = attackAnim3Time;
                        break;
                    }
                }
            else
            {
                allowRebound = true;
                currentAttackForce = airAttackForce;
                spriteAnimator.ChangeAnimationState(PLAYER_ATTACK_AIR);
                currentAttackDamage = airAttackDamage;
                attackAnimTime = attackAirTime;
            }

            hand.ChangeAnimationState("Hand_Idle");

            float attackAnimEndTime = Time.time + attackAnimTime;

            while (attackAnimEndTime > Time.time && isAttacking)
            {
                yield return new WaitForEndOfFrame();
            }

            attackResetTime = Time.time + attackResetBuffer; 
            isAttacking = false;
            hurtBox.enabled = true;
            currentAttack ++;
        }

    }

    public void Rebound()
    {
        isAttacking = false;
        isRebounding = true;
        StopCoroutine(Attack());
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(Vector2.up * reboundForce, ForceMode2D.Impulse);
        audioManager.Play("Rebound");
        spriteAnimator.ChangeAnimationState("Player_Rebound");
    }

    public IEnumerator Hurt()
    {
        StopDash();
        isRebounding = false;
        isInvincible = true;
        isHurt = true;
        audioManager.Play("Hurt");

        // Knockback
        float currentKnockbackTime = Time.time + knockbackTime;

        while(Time.time < currentKnockbackTime)
        {
            spriteAnimator.ChangeAnimationState("Player_Hurt");
            hand.ChangeAnimationState("Hand_Idle");
            rb2d.velocity = new Vector2 (-transform.localScale.x * knockbackSpeed, 0);
            yield return new WaitForEndOfFrame();
        }

        isHurt = false;

        // I Frames
        float currentIFrameTime = Time.time + iFrameTime;
        while(Time.time < currentIFrameTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            hand.enabled = !hand.enabled;
            yield return new WaitForEndOfFrame();
        }
        spriteRenderer.enabled = true;
        hand.enabled = true;
        isInvincible = false;
    }

    
}
