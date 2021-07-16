using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{

    [SerializeField] float ghostDelay; // Delay before next ghost is created
    [SerializeField] float ghostFadeTime = 0.5f;
    [SerializeField] GameObject ghost;
    [HideInInspector] public bool makeGhost = false; // If makeGhost is enabled, set in PlayerMovement
    [SerializeField] SpriteRenderer parentSprite; // Call spriterenderers from this and the player.


    public IEnumerator MakeGhosts() {
        makeGhost = true;
        while (makeGhost)
        {
            //generate a ghost
            GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation); // Create ghost from prefab on the player's position
            currentGhost.GetComponent<SpriteRenderer>().sprite = parentSprite.sprite;
            currentGhost.transform.localScale = transform.localScale;

            LeanTween.alpha(currentGhost, 0, ghostFadeTime).setDestroyOnComplete(true);
            yield return new WaitForSeconds(ghostDelay);
            yield return new WaitForFixedUpdate();
        }
    }
}
