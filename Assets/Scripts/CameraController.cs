using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float m_mouseX;
    private Vector3 dirToPlayer;

    public GameObject player;
    private void Start()
    {
        dirToPlayer = player.transform.position - transform.position;
    }
    private void FixedUpdate()
    {
        // 控制摄像机旋转
        if (Input.GetMouseButton(1))
        {
            m_mouseX = Input.GetAxis("Mouse X");

            Quaternion rot = Quaternion.AngleAxis(m_mouseX * 5.0f, Vector3.up);
            dirToPlayer = rot * dirToPlayer;

            transform.rotation = rot * transform.rotation;
        }
        else
        {
            m_mouseX = Mathf.SmoothStep(m_mouseX, 0, 0.2f);
            
            Quaternion rot = Quaternion.AngleAxis(m_mouseX * 5.0f, Vector3.up);
            dirToPlayer = rot * dirToPlayer;

            transform.rotation = rot * transform.rotation;
        }
        transform.position = player.transform.position - dirToPlayer;
    }
}
