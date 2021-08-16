using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraMove : MonoBehaviour
{

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(startPos.x + Mathf.Sin(Time.time / 2), startPos.y + Mathf.Sin(Time.time / 4), startPos.z);
    }
}
