using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class LobbySceneController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;

    [SerializeField] private TMP_Text player1NameText;
    [SerializeField] private TMP_Text player2NameText;

    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;

    private byte maxPlayers = 2;
    private bool isMaxPlayerSelected = false;
    private bool isRoomNameEntered = false;
    private bool isInLobby = false;

    private readonly List<GameObject> roomListItems = new();

    private void Awake()
    {
        if (createRoomButton != null)
        {
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
            createRoomButton.interactable = false;
        }
        if (roomNameInput != null)
        {
            roomNameInput.onValueChanged.AddListener(OnRoomNameChanged);
        }
    }

    private void Start()
    {
        if (roomListContent != null)
            roomListContent.gameObject.SetActive(false);

        ConnectToPhoton();
    }

    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            PhotonNetwork.JoinLobby();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        isInLobby = true;
        UpdateCreateButtonState();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isInLobby = false;
        SetCreateButtonInteractable(false);
    }

    public void OnMaxPlayerButtonClicked(int selectedMaxPlayers)
    {
        maxPlayers = (byte)selectedMaxPlayers;
        isMaxPlayerSelected = true;
        UpdateCreateButtonState();
    }

    private void OnRoomNameChanged(string input)
    {
        isRoomNameEntered = !string.IsNullOrEmpty(input.Trim());
        UpdateCreateButtonState();
    }

    private void UpdateCreateButtonState()
    {
        SetCreateButtonInteractable(isInLobby && isMaxPlayerSelected && isRoomNameEntered);
    }

    private void SetCreateButtonInteractable(bool interactable)
    {
        if (createRoomButton != null)
            createRoomButton.interactable = interactable;
    }

    private void OnCreateRoomClicked()
    {
        if (!isInLobby || !isMaxPlayerSelected || !isRoomNameEntered)
            return;

        string roomName = roomNameInput.text.Trim();
        var options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnCreatedRoom()
    {
        ShowRoomList(true);
        ClearRoomList();

        AddRoomListItem(roomNameInput.text.Trim(), 1, maxPlayers);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}");
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerNames();
        BroadcastRoomPlayerCount();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        BroadcastRoomPlayerCount();
        UpdatePlayerNames();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        BroadcastRoomPlayerCount();
        UpdatePlayerNames();
    }

    [PunRPC]
    private void UpdateRoomPlayerCount(int playerCount)
    {
        foreach (var item in roomListItems)
        {
            var itemScript = item.GetComponent<RoomListItem>();
            itemScript?.UpdatePlayerCount(playerCount, maxPlayers);
        }
    }

    private void BroadcastRoomPlayerCount()
    {
        photonView.RPC(nameof(UpdateRoomPlayerCount), RpcTarget.AllBuffered, PhotonNetwork.PlayerList.Length);
    }

    private void UpdatePlayerNames()
    {
        player1NameText.text = "";
        player2NameText.text = "";

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == 1)
                player1NameText.text = player.NickName;
            else if (player.ActorNumber == 2)
                player2NameText.text = player.NickName;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();

        if (roomList == null || roomList.Count == 0)
        {
            ShowRoomList(false);
            return;
        }

        ShowRoomList(true);

        foreach (var room in roomList)
        {
            if (!room.RemovedFromList)
                AddRoomListItem(room.Name, room.PlayerCount, room.MaxPlayers);
        }
    }

    private void AddRoomListItem(string roomName, int playerCount, int maxPlayers)
    {
        var item = Instantiate(roomListItemPrefab, roomListContent);
        var itemScript = item.GetComponent<RoomListItem>();

        itemScript?.SetRoomInfo(roomName, playerCount, maxPlayers, OnRoomListItemClicked);
        roomListItems.Add(item);
    }

    private void ClearRoomList()
    {
        foreach (var item in roomListItems)
            Destroy(item);

        roomListItems.Clear();
    }

    private void ShowRoomList(bool show)
    {
        if (roomListContent != null)
            roomListContent.gameObject.SetActive(show);
    }

    private void OnRoomListItemClicked(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
