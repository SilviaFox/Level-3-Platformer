using UnityEngine;

public class EnemyHitPhysics : MonoBehaviour
{
    Rigidbody2D rb2d;
    [SerializeField] float hitIntensity = 1;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void ApplyHitForce()
    {
        rb2d.velocity = Vector2.zero;
        Vector2 force = new Vector2(PlayerController.currentAttackForce.x * PlayerController.current.transform.localScale.x, PlayerController.currentAttackForce.y);
        rb2d.AddForce(force * hitIntensity, ForceMode2D.Impulse);
    }
}
