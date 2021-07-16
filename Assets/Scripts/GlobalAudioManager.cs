using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectAudioManager))]
public class GlobalAudioManager : MonoBehaviour
{
    public static ObjectAudioManager instance;

    void Start()
    {
        instance = GetComponent<ObjectAudioManager>();
    }
}
