using UnityEngine;

public class BulletLogic : MonoBehaviour
{

    float direction;
    [SerializeField] string bulletAnim;
    [SerializeField] float speed = 30;
    [SerializeField] LayerMask worldMask;
    public float damage;
    BoxCollider2D bulletCollider;

    void OnEnable()
    {
        direction = PlayerController.current.transform.localScale.x;
        transform.localScale = new Vector3(direction,1,1);
        GetComponent<Animator>().Play(bulletAnim);
        bulletCollider = GetComponent<BoxCollider2D>();
       
    }

    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction * speed, 0) * Time.deltaTime;

        if (Physics2D.BoxCast(transform.position, bulletCollider.size, 0, bulletCollider.offset, 0, worldMask))
        {
            Destroy(this.gameObject);            
        }
    }

}
