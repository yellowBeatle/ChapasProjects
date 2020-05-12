using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform m_camera;

    private void LateUpdate()
    {
        transform.LookAt(transform.position + m_camera.forward);
    }
}
