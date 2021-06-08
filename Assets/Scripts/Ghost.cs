using UnityEngine;

public class Ghost : MonoBehaviour
{

    [SerializeField] float ghostDelay; // Delay before next ghost is created
    [SerializeField] float ghostFadeTime = 0.5f;
    private float ghostDelaySeconds;
    [SerializeField] GameObject ghost;
    [HideInInspector] public bool makeGhost = false; // If makeGhost is enabled, set in PlayerMovement
    [SerializeField] SpriteRenderer playerSprite; // Call spriterenderers from this and the player.
    [SerializeField] SpriteRenderer ghostSprite;

    // Start is called before the first frame update
    void Start()
    {
        ghostDelaySeconds = ghostDelay;
    }

    private void Update()
    {
        ghostSprite.sprite = playerSprite.sprite; // Change to player's sprite
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (makeGhost)
        {
        if (ghostDelaySeconds > 0)
        {
            ghostDelaySeconds -= Time.fixedDeltaTime;
        }
        else
        {
            //generate a ghost
            GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation); // Create ghost from prefab on the player's position
            currentGhost.transform.localScale = transform.localScale;
            
            ghostDelaySeconds = ghostDelay; // Reset ghost delay
            LeanTween.alpha(currentGhost, 0, ghostFadeTime).setDestroyOnComplete(true);
            // Destroy(currentGhost, 1f); // Destroy the current ghost

        }
        }
    }
}
