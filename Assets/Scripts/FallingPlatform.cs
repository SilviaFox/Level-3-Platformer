using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    bool started, shaking;
    [SerializeField] float timeUntilFall = 1;
    BoxCollider2D platformCollider;

    SpriteRenderer spriteRenderer;
    Sprite defaultSprite;
    [SerializeField] Sprite destroyedSprite;

    Vector3 startPosition;
    [SerializeField] float shakeIntensity = 0.1f;

    void Start()
    {
        startPosition = transform.position;
        platformCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.collider.CompareTag("Player") && !started && other.relativeVelocity.y < 0)
        {
            started = true;
            StartCoroutine(FallingPlatformLogic());
        }
    }

    IEnumerator FallingPlatformLogic()
    {
        StartCoroutine(Shake());
        yield return new WaitForSeconds(timeUntilFall);

        spriteRenderer.sprite = destroyedSprite;
        platformCollider.enabled = false;
        shaking = false;

        LeanTween.move(this.gameObject, transform.position + Vector3.down, 0.2f);
        LeanTween.alpha(this.gameObject, 0, 0.2f);
    }

    IEnumerator Shake()
    {
        shaking = true;
        while (shaking)
        {
            yield return new WaitForFixedUpdate();
            transform.position = startPosition;
            transform.position -= Vector3.right * shakeIntensity;
            yield return new WaitForFixedUpdate();
            transform.position = startPosition;
            transform.position += Vector3.right * shakeIntensity;
        }

        transform.position = startPosition;
    }
}
