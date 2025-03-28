using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_LaunchCountdown;

    private Rigidbody2D m_Rigidbody;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!NetworkManager.IsServer)
        {
            this.enabled = false;
        }
        else
        {
            StartCoroutine(LaunchCountdownCoroutine());
        }
    }

    private void LaunchBall()
    {
        float angle = Random.Range(135f, 225f);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);

        m_Rigidbody.linearVelocity = new Vector3(x, y, 0) * m_MoveSpeed;
    }

    private IEnumerator LaunchCountdownCoroutine()
    {
        yield return new WaitForSeconds(m_LaunchCountdown);
        LaunchBall();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var contact = collision.contacts[0];
        var newVel = Vector3.Reflect(m_Rigidbody.linearVelocity, contact.normal);
        m_Rigidbody.linearVelocity = newVel;
    }
}
