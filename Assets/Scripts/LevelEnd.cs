using UnityEngine;
using Cinemachine;

public class LevelEnd : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;
            GameManager.EndStage();
        }
    }
}
