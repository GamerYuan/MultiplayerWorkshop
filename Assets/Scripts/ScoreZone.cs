using Unity.Netcode;
using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    public int ScoringPlayerId;
    [SerializeField] private ScoreManager m_ScoreManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!NetworkManager.Singleton.IsServer) enabled = false;
    }

    public void OnBallCollision()
    {
        m_ScoreManager.AddScore(ScoringPlayerId);
    }
}
