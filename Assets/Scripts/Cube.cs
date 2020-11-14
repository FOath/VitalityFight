using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public GameObject m_owner;
    public bool m_isThrown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (!m_owner && m_owner == collision.gameObject)
            return;
        if (m_isThrown)
            Destroy(gameObject);
    }
}
