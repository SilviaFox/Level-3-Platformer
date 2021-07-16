using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ObjectReboundLight : MonoBehaviour
{
    Light2D glowLight;
    
    private void OnEnable()
    {
        glowLight = GetComponent<Light2D>();
    }

    public void Hit()
    {
        StopAllCoroutines();
        StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        glowLight.enabled = false;
        yield return new WaitForSeconds(1f);
        glowLight.enabled = true;
    }
}
