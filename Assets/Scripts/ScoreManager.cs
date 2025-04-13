using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private TMP_Text m_PlayerOneScoreText;
    [SerializeField] private TMP_Text m_PlayerTwoScoreText;

    private NetworkVariable<int> m_PlayerOneScore = new(0);
    private NetworkVariable<int> m_PlayerTwoScore = new(0);

    private void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_PlayerOneScore.OnValueChanged += (old, val) => m_PlayerOneScoreText.text = val.ToString();
        m_PlayerTwoScore.OnValueChanged += (old, val) => m_PlayerTwoScoreText.text = val.ToString();
    }

    public void AddScore(int playerId)
    {
        if (playerId == 0)
        {
            m_PlayerOneScore.Value++;
        }
        else
        {
            m_PlayerTwoScore.Value++;
        }
    }
}
