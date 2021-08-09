using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeaper : MonoBehaviour
{
    
    Transform target;
    Rigidbody2D rb2d;
    [SerializeField] float launchStrength = 3, upwardsLaunchStrength = 3;
    [SerializeField] float minIntervalTime = 1, maxIntervalTime = 2;
    [SerializeField] LayerMask groundLayer;
    bool active;
    Vector2 boundsSize;
    Ghost ghost;

    void Start()
    {
        target = PlayerController.current.transform;
        rb2d = GetComponent<Rigidbody2D>();
        boundsSize = GetComponent<BoxCollider2D>().size;
        ghost = GetComponent<Ghost>();
    }

    void OnBecameVisible()
    {
        StartCoroutine(Leap());
    }

    void OnBecameInvisible()
    {
        StopAllCoroutines();
    }
    

    IEnumerator Leap() 
    {
        yield return new WaitForSeconds(Random.Range(minIntervalTime, maxIntervalTime));

        while (true)
        {
            ghost.makeGhost = false;
            bool grounded = Physics2D.BoxCast(transform.position, boundsSize, 0, Vector2.down, 0.5f, groundLayer);
            
            Vector3 turnScale = new Vector3(transform.position.x > PlayerController.current.transform.position.x? 1:-1, 1,1);
            transform.localScale = turnScale;

            if (grounded)
            {
                StartCoroutine(ghost.MakeGhosts());
                rb2d.AddForce((target.position - transform.position).normalized * launchStrength, ForceMode2D.Impulse);
                rb2d.AddForce(transform.up * upwardsLaunchStrength, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(Random.Range(minIntervalTime, maxIntervalTime));

        }
    }

}
