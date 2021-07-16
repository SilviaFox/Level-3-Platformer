using UnityEngine;

public class EnemyHitPhysics : MonoBehaviour
{
    Rigidbody2D rb2d;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void ApplyHitForce()
    {
        Vector2 force = new Vector2(PlayerController.currentAttackForce.x * PlayerController.current.transform.localScale.x, PlayerController.currentAttackForce.y);
        rb2d.AddForce(force, ForceMode2D.Impulse);
    }
}
