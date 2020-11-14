using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] private Rigidbody m_rigidbody = null;

    private GameObject m_camera = null;

    // 虚拟摇杆控制器
    public VariableJoystick m_moveJoystick;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.5f;
    private bool m_jumpInput = false;

    private bool m_isGrounded;

    // 方块相关属性
    private GameObject m_handedCube;
    public bool m_hasCube = false;
    public float m_throwStrength = 2.0f;

    // 冲刺
    private bool m_isDash = false;
    private bool m_dashInput = false;
    private float m_dashTime = 0.0f;
    public float m_dashThreshold = 0.1f;
    public float m_dashSpeed = 500.0f;
    // 游戏内属性
    private int m_maxHealth = 100;
    public int m_currentHealth = 100;

    private List<Collider> m_colisions = new List<Collider>();
    // Start is called before the first frame update
    void Awake()
    {
        if (!m_animator) { m_animator = gameObject.GetComponent<Animator>(); }
        if(!m_rigidbody) { m_rigidbody = gameObject.GetComponent<Rigidbody>(); }
    }
    private void Start()
    {
        if (!m_camera) { m_camera = Camera.main.gameObject; }
    }
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; ++i)
        {
            if(Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_colisions.Contains(collision.collider))
                {
                    m_colisions.Add(collision.collider);
                }    
                m_isGrounded = true;
                this.GetComponent<CapsuleCollider>().material.dynamicFriction = 0.5f;
                this.GetComponent<CapsuleCollider>().material.frictionCombine = PhysicMaterialCombine.Average;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for(int i = 0; i < contactPoints.Length; ++i)
        {
            if(Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true;
                break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if(!m_colisions.Contains(collision.collider))
            {
                m_colisions.Add(collision.collider);
            }
        }
        else
        {
            if(m_colisions.Contains(collision.collider))
            {
                m_colisions.Remove(collision.collider);
            }
            if(m_colisions.Count == 0)
            {
                m_isGrounded = false;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(m_colisions.Contains(collision.collider))
        {
            m_colisions.Remove(collision.collider);
        }
        if(m_colisions.Count == 0)
        {
            m_isGrounded = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!m_jumpInput && Input.GetKeyDown(KeyCode.Space))
        {
            m_jumpInput = true;
        }
        if(m_isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && !m_isDash)
        {
            m_dashInput = true;
            m_isDash = true;
        }
        
    }
    private void FixedUpdate()
    {
        m_animator.SetBool("Grounded", m_isGrounded);
        DirectUpdate();
        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }
    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (m_moveJoystick)
        {
            v = m_moveJoystick.Direction.y;
            h = m_moveJoystick.Direction.x;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = m_camera.transform.forward * m_currentV + m_camera.transform.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        // 冲刺优先级最高
        if (m_isDash)
        {
            if(m_dashTime <= 0)
            {
                m_isDash = false;
                m_dashTime = m_dashThreshold;
            }
            else
            {
                m_dashTime -= Time.deltaTime;
                GetComponent<Rigidbody>().velocity = direction * m_dashTime * m_dashSpeed;
            }
            return;
        }



        if (direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", directionLength);
        }


        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if(jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            this.GetComponent<CapsuleCollider>().material.dynamicFriction = 0;
            this.GetComponent<CapsuleCollider>().material.frictionCombine = PhysicMaterialCombine.Minimum;
            m_jumpTimeStamp = Time.time;
            m_rigidbody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if(!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }
        if(!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    public void pickupCube()
    {
        if (!m_isGrounded)
            return;

        Vector3 start = transform.position;
        start.y += 0.25f;

        Ray ray1 = new Ray(start, transform.forward);
        Ray ray2 = new Ray(start, transform.right);
        Ray ray3 = new Ray(start, -transform.right);
        RaycastHit raycastHit;

        if(Physics.Raycast(ray1, out raycastHit, 0.5f) 
                || Physics.Raycast(ray2, out raycastHit, 0.5f) 
                || Physics.Raycast(ray3, out raycastHit, 0.5f))
        {
            if(raycastHit.collider.gameObject.tag == "Cube")
            {
                m_animator.SetBool("Pickup", true);
                m_handedCube = raycastHit.collider.gameObject;
                m_handedCube.GetComponent<Cube>().m_owner = gameObject;
                m_handedCube.GetComponent<BoxCollider>().enabled = false;
                m_handedCube.GetComponent<Rigidbody>().useGravity = false;
                m_handedCube.transform.parent = transform;
                m_handedCube.transform.localPosition = new Vector3(0, 1.0f, 0.5f);
                m_hasCube = true;
            }
        }
    }
    public void throwCube(Vector2 dir)
    {
        if (!m_hasCube || !m_handedCube)
            return;
        // 人物转向
        Vector3 direction = m_camera.transform.forward * dir.y + m_camera.transform.right * dir.x;
        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;
        transform.rotation = Quaternion.LookRotation(direction);

        m_handedCube.transform.parent = null;
        if (m_handedCube.GetComponent<Cube>())
        {
            m_handedCube.GetComponent<Cube>().m_isThrown = true;
        }
        m_handedCube.GetComponent<BoxCollider>().enabled = true;
        m_handedCube.GetComponent<Rigidbody>().isKinematic = false;
        m_handedCube.GetComponent<Rigidbody>().useGravity = true;
        m_handedCube.GetComponent<Rigidbody>().AddForce((direction + new Vector3(0, 0.2f, 0)) * m_throwStrength, ForceMode.Impulse);
        m_handedCube = null;
        m_hasCube = false;
        

    }
}
