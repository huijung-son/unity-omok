using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using System.Collections;


public class LobbySceneController : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private TMP_Text player1NameText;
    [SerializeField] private TMP_Text player2NameText;

    [Header("Room List UI")]
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;

    [SerializeField] private Button[] realStartBtn;

    private byte maxPlayers = 2;
    private bool isMaxPlayerSelected = false;
    private bool isRoomNameEntered = false;
    private bool isInLobby = false;

    private readonly List<GameObject> roomListItems = new List<GameObject>();

    private void Awake()
    {
        if (createRoomButton != null)
        {
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
            createRoomButton.interactable = false; // �ʱ� ��Ȱ��ȭ
        }

        realStartBtn = GetComponentsInChildren<Button>();
        realStartBtn[realStartBtn.Length - 1].onClick.AddListener(StartBtn);
        realStartBtn[realStartBtn.Length - 1].gameObject.SetActive(false);
    }

    private void Start()
    {
        if (roomNameInput != null)
            roomNameInput.onValueChanged.AddListener(OnRoomNameChanged);

        if (roomListContent != null)
            roomListContent.gameObject.SetActive(false); // �� ��� ���� �ʱ�ȭ

        ConnectToPhoton();
    }

    // ���� ���� ���� �õ�
    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetStatus("���� ���� ���� ��...");
        }
        else
        {
            SetStatus("���� ���� �����");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        SetStatus("���� ���� �����. �κ� ���� ��...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SetStatus("�κ� ���� ����!");
        isInLobby = true;
        UpdateCreateButtonState();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetStatus($"���� ���� ���� ����: {cause}");
        isInLobby = false;

        if (createRoomButton != null)
            createRoomButton.interactable = false;
    }

    // �ִ� �ο� ���� ��ư Ŭ�� �� ȣ��
    public void OnMaxPlayerButtonClicked(int selectedMaxPlayers)
    {
        maxPlayers = (byte)selectedMaxPlayers;
        isMaxPlayerSelected = true;
        SetStatus($"�ִ� �ο�: {maxPlayers}�� ���õ�");
        UpdateCreateButtonState();
    }

    // �� �̸� �Է� �ʵ� �� ���� �� ȣ��
    private void OnRoomNameChanged(string input)
    {
        isRoomNameEntered = !string.IsNullOrEmpty(input.Trim());
        UpdateCreateButtonState();
    }

    // �� ���� ��ư Ȱ��ȭ ���� ����
    private void UpdateCreateButtonState()
    {
        if (createRoomButton != null)
            createRoomButton.interactable = isInLobby && isMaxPlayerSelected && isRoomNameEntered;
    }

    // �� ���� ��û ó��
    private void OnCreateRoomClicked()
    {
        if (!isInLobby)
        {
            SetStatus("�κ� �����ؾ� ���� ���� �� �ֽ��ϴ�.");
            return;
        }
        if (!isMaxPlayerSelected)
        {
            SetStatus("�ִ� �ο��� �����ϼ���.");
            return;
        }
        if (!isRoomNameEntered)
        {
            SetStatus("�� �̸��� �Է��ϼ���.");
            return;
        }

        string roomName = roomNameInput.text.Trim();
        var options = new RoomOptions { MaxPlayers = maxPlayers };

        if (PhotonNetwork.CreateRoom(roomName, options))
            SetStatus($"�� ���� ��û ��: {roomName}");
        else
            SetStatus("�� ���� ����!");
    }

    public override void OnCreatedRoom()
    {
        SetStatus("�� ���� ����!");

        if (roomListContent != null)
            roomListContent.gameObject.SetActive(true);

        ClearRoomList();

        var item = Instantiate(roomListItemPrefab, roomListContent);
        var itemScript = item.GetComponent<RoomListItem>();

        string roomName = roomNameInput.text.Trim();
        itemScript.SetRoomInfo(roomName, 1, maxPlayers, OnRoomListItemClicked);
        roomListItems.Add(item);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetStatus($"�� ���� ����: {message}");
    }

    public override void OnJoinedRoom()
    {
        SetStatus("�� ���� ����");
        UpdatePlayerNames();
        Debug.Log("2222 OnJoinedRoom" + PhotonNetwork.PlayerList.Length);
        if (PhotonNetwork.IsMasterClient)
        {
            realStartBtn[realStartBtn.Length - 1].gameObject.SetActive(true);
        }
    }

    public void StartBtn()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            photonView.RPC("RealStart", RpcTarget.All);
        }
        else
        {
            Debug.Log("�÷��̾ �� á���");
        }
    }

    [PunRPC]
    public void RealStart()
    {
        SceneManager.LoadScene(3);
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    if (PhotonNetwork.PlayerList.Length == 2)
    //    {
    //        SceneManager.LoadScene(3);
    //    }
    //}

    // �÷��̾� �̸� UI ������Ʈ
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

        if (roomList.Count == 0)
        {
            if (roomListContent != null)
                roomListContent.gameObject.SetActive(false);
            SetStatus("������ ���� �����");
            return;
        }

        if (roomListContent != null)
            roomListContent.gameObject.SetActive(true);

        foreach (var room in roomList)
        {
            if (!room.RemovedFromList)
            {
                var item = Instantiate(roomListItemPrefab, roomListContent);
                var itemScript = item.GetComponent<RoomListItem>();

                itemScript.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers, OnRoomListItemClicked);
                roomListItems.Add(item);
            }
        }
    }

    // �� ��� �ʱ�ȭ
    private void ClearRoomList()
    {
        foreach (var item in roomListItems)
            Destroy(item);

        roomListItems.Clear();
    }

    // �� ��� ������ Ŭ�� �� ȣ��
    private void OnRoomListItemClicked(string roomName)
    {
        SetStatus($"�� ���� ��û ��: {roomName}");
        PhotonNetwork.JoinRoom(roomName);
    }

    // ���� �޽��� UI �� �α� ���
    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;

        Debug.Log(message);
    }
}
