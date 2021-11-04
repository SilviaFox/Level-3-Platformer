using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraMove : MonoBehaviour
{

    Vector3 startPos;
    [SerializeField] Vector2 intensity, time;

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(startPos.x + Mathf.Sin(Time.time / time.x) * intensity.x, startPos.y + Mathf.Sin(Time.time / time.y) * intensity.y, startPos.z);
    }
}
