using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyZombie : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] bool startFacingRight;
    Vector2 direction;
    Rigidbody2D rb2d;
    Tilemap worldTiles;
    Vector3 previousPosition;
    [SerializeField] LayerMask worldLayer;

    void Start()
    {
        direction = startFacingRight? Vector2.right : Vector2.left;
        rb2d = GetComponent<Rigidbody2D>();
        worldTiles = GameObject.FindGameObjectWithTag("World").GetComponent<Tilemap>();
    }

    void Update()
    {
        rb2d.velocity = direction * speed;

        if (CheckPos())
            direction = -direction;
    }

    bool CheckPos()
    {
        if (worldTiles.HasTile(PosCalcs.Round(transform.position + (Vector3)direction)))
            return true;
        
        if (Physics2D.Raycast(transform.position, direction, 0.75f, worldLayer))
            return true;

        if (worldTiles.HasTile(PosCalcs.Floor(transform.position + Vector3.down + (Vector3)direction)))
            return false;

        return true;
    }
}
