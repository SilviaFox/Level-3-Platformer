using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinLogic : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float minStartForce;
    [SerializeField] float maxStartForce;
    [SerializeField] float minMoveWaitTime = 0.5f;
    [SerializeField] float maxMoveWaitTime = 1.5f;

    [SerializeField] int value = 100;

    bool moveReady;

    Rigidbody2D rb2d;
    ObjectAudioManager audioManager;

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioManager = GetComponent<ObjectAudioManager>();
        StartCoroutine(Coin());
    }
    // Update is called once per frame

    IEnumerator Coin()
    {
        rb2d.AddForce(new Vector2(Random.Range(minStartForce,maxStartForce), Random.Range(minStartForce,maxStartForce)), ForceMode2D.Impulse);
        yield return new WaitForSeconds(Random.Range(minMoveWaitTime,maxMoveWaitTime));

        audioManager.Play("Suck");
        moveReady = true;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        
    }

    void FixedUpdate()
    {
        if (moveReady)
            transform.position = Vector3.MoveTowards(transform.position, PlayerController.current.gameObject.transform.position, speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.audioManager.Play("CoinCollect");
            PlayerScore.instance.AddScoreInit(value);
            Destroy(this.gameObject);
        }
    }
}
