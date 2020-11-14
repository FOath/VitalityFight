using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject m_splatter;

    private GameObject m_healthBar;
    // Start is called before the first frame update
    private void Start()
    {
        if (!gameManager) gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (!m_healthBar) m_healthBar = transform.GetChild(2).gameObject;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Cube")
        {
            print("collision");

            GameObject sp = (GameObject)Instantiate(m_splatter);
            sp.transform.position = collision.GetContact(0).point;
            sp.transform.forward = collision.GetContact(0).normal;

            Destroy(collision.gameObject);
        }
    }
    private void Update()
    {
        m_healthBar.transform.LookAt(Camera.main.transform);
        m_healthBar.transform.Rotate(new Vector3(0, 180, 0));
    }
}
