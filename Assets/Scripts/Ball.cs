using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_LaunchCountdown;

    private Rigidbody2D m_Rigidbody;
    private NetworkTransform m_NetworkTransform;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_NetworkTransform = GetComponent<NetworkTransform>();
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
            NetworkManager.Singleton.OnClientConnectedCallback += Ball_OnClientConnectedCallback;
        }
    }

    private void Ball_OnClientConnectedCallback(ulong clientId)
    {
        if (clientId == 1) LaunchCountdownTask(Random.Range(0, 2)).Forget();
    }

    private void LaunchBall(int launchToPlayerId)
    {
        float angle = launchToPlayerId == 0 ? Random.Range(135f, 225f) : Random.Range(-45f, 45f);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);

        m_Rigidbody.linearVelocity = new Vector3(x, y, 0) * m_MoveSpeed;
    }

    private async UniTask LaunchCountdownTask(int launchToPlayerId)
    {
        await UniTask.WaitForSeconds(m_LaunchCountdown);
        LaunchBall(launchToPlayerId);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ScoreZone"))
        {
            var scoreZone = collision.gameObject.GetComponent<ScoreZone>();
            scoreZone.OnBallCollision();
            m_Rigidbody.linearVelocity = Vector3.zero;
            m_NetworkTransform.Teleport(Vector3.zero, Quaternion.identity, Vector3.one);
            int sendToPlayer = scoreZone.ScoringPlayerId == 0 ? 1 : 0;
            LaunchCountdownTask(sendToPlayer).Forget();
            return;
        }

        var contact = collision.contacts[0];
        var newVel = Vector3.Reflect(m_Rigidbody.linearVelocity, contact.normal);
        m_Rigidbody.linearVelocity = newVel;
    }
}
