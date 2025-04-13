using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button m_CreateRoomButton;
    [SerializeField] private Button m_JoinRoomButton;
    [SerializeField] private TMP_InputField m_RoomCodeInputField;

    async void Start()
    {
        m_CreateRoomButton.onClick.AddListener(() => CreateRoom().Forget());
        m_JoinRoomButton.onClick.AddListener(() => JoinRoom().Forget());

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async UniTask CreateRoom()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(2);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log($"Relay Join Code: {joinCode}");

        var lobby = await LobbyService.Instance.CreateLobbyAsync(joinCode, 2, new()
        {
            Data = new()
            {
                { "RelayJoinCode", new(Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Public, joinCode) }
            }
        });

        Debug.Log($"Room Code: {lobby.LobbyCode}");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("dtls"));
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private async UniTask JoinRoom()
    {
        var roomCode = m_RoomCodeInputField.text;

        Debug.Log($"Joining Room: {roomCode}");

        var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(roomCode);
        var joinCode = lobby.Data["RelayJoinCode"].Value;
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        Debug.Log($"Joining Relay Allocation: {joinCode}");

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("dtls"));
        NetworkManager.Singleton.StartClient();
    }
}
