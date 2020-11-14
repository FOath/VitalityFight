using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGas : Cube
{
    public GameObject m_fxGas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (m_isThrown)
        {
            GameObject fire = GameObject.Instantiate(m_fxGas);
            fire.transform.position = collision.contacts[0].point;
            Destroy(gameObject);
        }
    }
}
