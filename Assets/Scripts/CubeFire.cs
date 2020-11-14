using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFire : Cube
{
    public GameObject m_fxFire;
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
        if (!m_owner && m_owner == collision.gameObject)
            return;
        if (m_isThrown)
        {
            m_isThrown = false;
            GameObject fire = GameObject.Instantiate(m_fxFire);
            Vector3 pos = collision.contacts[0].point;
            fire.transform.position = pos;
            Destroy(gameObject);
        }
    }
}
