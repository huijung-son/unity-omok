using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LobbySceneController : MonoBehaviourPunCallbacks
{
    //방이름입력
    [SerializeField] private TMP_InputField roomNameInput;
    //방생성
    [SerializeField] private Button createRoomButton;
    //플레이어 이름지정
    [SerializeField] private TMP_Text player1NameText;
    [SerializeField] private TMP_Text player2NameText;
    //방목록스크롤뷰
    [SerializeField] private Transform roomListContent;
    //방목록프리팹
    [SerializeField] private GameObject roomListItemPrefab;
    //게임시작버튼
    [SerializeField] private Button startGameBtn;
    
   
    private byte maxPlayers = 2;
    private bool isMaxPlayerSelected = false;
    private bool isRoomNameEntered = false;
    private bool isInLobby = false;

    private readonly List<GameObject> roomListItems = new();

    
    private void Awake()
    {
        //방생성 이벤트함수
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
        //게임 시작버튼 동기화
        startGameBtn.onClick.AddListener(() =>
        {
            photonView.RPC("OnGameStartButton", RpcTarget.All);
        });

        if (roomListContent != null)
            roomListContent.gameObject.SetActive(false);

        ConnectToPhoton();
    }

    private void Update()
    {
        //방장만 게임시작버튼 노출
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 2)
        {
            startGameBtn.gameObject.SetActive(true);
        }
    }

    //네트워크 연결
    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            PhotonNetwork.JoinLobby();
    }

    
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    //방 입장
    public override void OnJoinedLobby()
    {
        isInLobby = true;
        UpdateCreateButtonState();
    }

    //연결해제
    public override void OnDisconnected(DisconnectCause cause)
    {
        isInLobby = false;
        SetCreateButtonInteractable(false);
    }

    //최대 플레이어 세팅
    public void OnMaxPlayerButtonClicked(int selectedMaxPlayers)
    {
        maxPlayers = (byte)selectedMaxPlayers;
        isMaxPlayerSelected = true;
        UpdateCreateButtonState();
    }

    //방이름 설정
    private void OnRoomNameChanged(string input)
    {
        isRoomNameEntered = !string.IsNullOrEmpty(input.Trim());
        UpdateCreateButtonState();
    }

    //방 생성상태
    private void UpdateCreateButtonState()
    {
        SetCreateButtonInteractable(isInLobby && isMaxPlayerSelected && isRoomNameEntered);
    }

    //방생성 상호작용
    private void SetCreateButtonInteractable(bool interactable)
    {
        if (createRoomButton != null)
            createRoomButton.interactable = interactable;
    }

    //인원,방이름,방만들기 후 방목록 갱신
    private void OnCreateRoomClicked()
    {
        if (!isInLobby || !isMaxPlayerSelected || !isRoomNameEntered)
            return;

        string roomName = roomNameInput.text.Trim();
        var options = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom(roomName, options);
    }

    //방목록
    public override void OnCreatedRoom()
    {
        ShowRoomList(true);
        ClearRoomList();

        AddRoomListItem(roomNameInput.text.Trim(), 1, maxPlayers);
    }

    //방 생성 실패
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}");
    }

    //플레이어 이름, 인원
    public override void OnJoinedRoom()
    {
        UpdatePlayerNames();
        BroadcastRoomPlayerCount();
    }

    //방 입장
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        BroadcastRoomPlayerCount();
        UpdatePlayerNames();
    }

    //방 퇴장
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        BroadcastRoomPlayerCount();
        UpdatePlayerNames();
    }

    
    //플레이어수 동기화
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

    //플레이어 이름 
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

    //방 목록 업데이트 
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
    
    //방목록프리팹
    private void AddRoomListItem(string roomName, int playerCount, int maxPlayers)
    {
        var item = Instantiate(roomListItemPrefab, roomListContent);
        var itemScript = item.GetComponent<RoomListItem>();

        itemScript?.SetRoomInfo(roomName, playerCount, maxPlayers, OnRoomListItemClicked);
        roomListItems.Add(item);
    }

    [PunRPC]
    //게임시작 동기화
    public void OnGameStartButton()
    {
        SceneManager.LoadScene("GameStartScene");
    }

    //방폭파
    private void ClearRoomList()
    {
        foreach (var item in roomListItems)
            Destroy(item);

        roomListItems.Clear();
    }

    //방목록 뷰
    private void ShowRoomList(bool show)
    {
        if (roomListContent != null)
            roomListContent.gameObject.SetActive(show);
    }

    //방목록 프리팹 상호작용
    private void OnRoomListItemClicked(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
