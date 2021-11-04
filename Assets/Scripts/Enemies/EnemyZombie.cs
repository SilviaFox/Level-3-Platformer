using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyZombie : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] bool startFacingRight;
    Vector3 direction, offset;
    Rigidbody2D rb2d;
    Tilemap worldTiles;
    Vector3 previousPosition;
    [SerializeField] LayerMask worldLayer;
    SpriteRenderer spriteRenderer;

    bool active = false;

    void OnBecameVisible()
    {
        active = true;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

        worldTiles = GameObject.FindGameObjectWithTag("World").GetComponent<Tilemap>();

        direction = startFacingRight? Vector2.right : Vector2.left;
        spriteRenderer.flipX = !startFacingRight;
        offset = GetComponent<BoxCollider2D>().offset;
    }

    void Update()
    {
        if (active)
        {
            rb2d.velocity = direction * speed;


            if (CheckPos())
            {
                direction = -direction;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
    }

    bool CheckPos()
    {
        if (worldTiles.HasTile(PosCalcs.Round(transform.position + direction + offset)))
            return true;
        
        if (Physics2D.Raycast(transform.position + offset, direction, 0.75f, worldLayer))
            return true;

        if (worldTiles.HasTile(PosCalcs.Floor(transform.position + Vector3.down + direction + offset)))
            return false;

        return true;
    }
}
