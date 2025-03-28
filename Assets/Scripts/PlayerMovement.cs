using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float m_MoveSpeed;

    private Rigidbody2D m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) this.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_Rigidbody.linearVelocityY = m_MoveSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            m_Rigidbody.linearVelocityY = -m_MoveSpeed;
        }
        else
        {
            m_Rigidbody.linearVelocityY = 0;
        }
    }
}
