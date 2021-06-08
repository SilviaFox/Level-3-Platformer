using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    // Animation States
    string currentState; // Current animation state
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }



    public void ChangeAnimationState(string newState, float animTime = 0)
    {

        //stop the same animation from interrupting itself
        if (currentState == newState) return;
        
        // Play the animation
        animator.Play(newState, 0, animTime);

        // Reassign the current state
        currentState = newState;
    }
}
