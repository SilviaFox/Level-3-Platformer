using UnityEngine;
using Cinemachine;

public class CameraFocusArea : MonoBehaviour
{
    public static float defaultCamLensSize;
    public static CinemachineVirtualCamera cam;

    [SerializeField] float size = 8f;
    [SerializeField] Transform focusPoint;

    void Awake()
    {
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        defaultCamLensSize = cam.m_Lens.OrthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cam.m_Lens.OrthographicSize = size;
            cam.m_Follow = focusPoint == null? transform : focusPoint;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cam.m_Lens.OrthographicSize = defaultCamLensSize;
            cam.m_Follow = PlayerController.current.transform;
        }
    }
}
