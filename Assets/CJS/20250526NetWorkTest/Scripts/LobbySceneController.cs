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
            createRoomButton.interactable = false; // 초기 비활성화
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
            roomListContent.gameObject.SetActive(false); // 방 목록 숨김 초기화

        ConnectToPhoton();
    }

    // 포톤 서버 연결 시도
    private void ConnectToPhoton()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            SetStatus("포톤 서버 접속 중...");
        }
        else
        {
            SetStatus("포톤 서버 연결됨");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        SetStatus("포톤 서버 연결됨. 로비 입장 중...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SetStatus("로비 입장 성공!");
        isInLobby = true;
        UpdateCreateButtonState();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetStatus($"포톤 서버 연결 끊김: {cause}");
        isInLobby = false;

        if (createRoomButton != null)
            createRoomButton.interactable = false;
    }

    // 최대 인원 선택 버튼 클릭 시 호출
    public void OnMaxPlayerButtonClicked(int selectedMaxPlayers)
    {
        maxPlayers = (byte)selectedMaxPlayers;
        isMaxPlayerSelected = true;
        SetStatus($"최대 인원: {maxPlayers}명 선택됨");
        UpdateCreateButtonState();
    }

    // 방 이름 입력 필드 값 변경 시 호출
    private void OnRoomNameChanged(string input)
    {
        isRoomNameEntered = !string.IsNullOrEmpty(input.Trim());
        UpdateCreateButtonState();
    }

    // 방 생성 버튼 활성화 여부 갱신
    private void UpdateCreateButtonState()
    {
        if (createRoomButton != null)
            createRoomButton.interactable = isInLobby && isMaxPlayerSelected && isRoomNameEntered;
    }

    // 방 생성 요청 처리
    private void OnCreateRoomClicked()
    {
        if (!isInLobby)
        {
            SetStatus("로비에 입장해야 방을 만들 수 있습니다.");
            return;
        }
        if (!isMaxPlayerSelected)
        {
            SetStatus("최대 인원을 선택하세요.");
            return;
        }
        if (!isRoomNameEntered)
        {
            SetStatus("방 이름을 입력하세요.");
            return;
        }

        string roomName = roomNameInput.text.Trim();
        var options = new RoomOptions { MaxPlayers = maxPlayers };

        if (PhotonNetwork.CreateRoom(roomName, options))
            SetStatus($"방 생성 요청 중: {roomName}");
        else
            SetStatus("방 생성 실패!");
    }

    public override void OnCreatedRoom()
    {
        SetStatus("방 생성 성공!");

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
        SetStatus($"방 생성 실패: {message}");
    }

    public override void OnJoinedRoom()
    {
        SetStatus("방 입장 성공");
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
            Debug.Log("플레이어가 덜 찼어요");
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

    // 플레이어 이름 UI 업데이트
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
            SetStatus("생성된 방이 없어용");
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

    // 방 목록 초기화
    private void ClearRoomList()
    {
        foreach (var item in roomListItems)
            Destroy(item);

        roomListItems.Clear();
    }

    // 방 목록 아이템 클릭 시 호출
    private void OnRoomListItemClicked(string roomName)
    {
        SetStatus($"방 입장 요청 중: {roomName}");
        PhotonNetwork.JoinRoom(roomName);
    }

    // 상태 메시지 UI 및 로그 출력
    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;

        Debug.Log(message);
    }
}
