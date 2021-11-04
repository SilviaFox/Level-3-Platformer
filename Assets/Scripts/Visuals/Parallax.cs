using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Camera cam; // Camera
    [SerializeField] Transform subject;
    
    Vector2 startPosition; // Position of the object on start
    float startZ;

    Vector2 travel => (Vector2)cam.transform.position - startPosition; // Distance of the camera from start position

    float distanceFromSubject => transform.position.z - subject.position.z; // Distance from player

    float clippingPlane => (cam.transform.position.z + (distanceFromSubject > 0? cam.farClipPlane : cam.nearClipPlane)); // If Z is above Zero, paralax will use far clipping plane

    
    float parallaxFactor => Mathf.Abs(distanceFromSubject) / clippingPlane;
    

    void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Parallax Equation = origin + travel * parallax
        
        Vector2 newPos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}
