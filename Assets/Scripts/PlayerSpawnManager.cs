using Unity.Netcode;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Vector3[] m_SpawnPoints;
    [SerializeField] private NetworkObject m_PlayerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerPosition;
    }
    private void SpawnPlayerPosition(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(m_PlayerPrefab, clientId, false,
            true, false, m_SpawnPoints[clientId]);
    }
}
