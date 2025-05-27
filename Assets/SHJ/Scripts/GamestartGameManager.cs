using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GomokuManager;

public class GamestartGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject hightLight;
    [SerializeField] private GameObject whiteBlock;
    [SerializeField] private GameObject blackBlock;

    private Vector3 startPos = Vector3.zero;
    Vector3 minVector = Vector3.zero;
    private Camera cam;
    private GameObject cloneHightLight;

    // 필드에 놓을수 있는 포지션들
    private Vector3[] fieldPos = null;
    // 필드에 놓인 돌 포지션들
    private List<Vector3> fieldInPos = null;
    // 흑돌 플레이어 먼저
    private bool isFirst = true;
    // 위의 불린값으로 선택된 돌
    private GameObject selectBlock = null;
    // 내 차례인지
    private bool isMyTurn = false;

    private GomokuManager gmHDG = new GomokuManager();

    private void Start()
    {
        for (int i = 0; i < 15; ++i)
        {
            for (int j = 0; j < 15; ++j)
            {
                GomokuStone stone = new GomokuStone();
                stone.Color = GomokuColor.None;
                stone.XPos = j;
                stone.YPos = i;
                gmHDG.board[j, i] = stone;
            }
        }

        cloneHightLight = Instantiate(hightLight);

        if (lineRenderer != null)
        {
            int size = (int)lineRenderer.transform.localScale.x;
            cam = Camera.main;

            fieldPos = new Vector3[size * size];
            fieldInPos = new List<Vector3>();

            Vector3[] pos = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(pos);
            startPos = pos[0];

            int index = 0;
            for (int i = 0; i < size; ++i)
            {
                float y = startPos.y - i;
                for (int j = 0; j < size; ++j)
                {
                    float x = startPos.x + j;
                    Vector3 newPos = new Vector3(x, y, 0f);
                    fieldPos[index] = newPos;
                    ++index;
                }
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            selectBlock = blackBlock;
            isMyTurn = true;
        }
        else
        {
            selectBlock = whiteBlock;
            isFirst = false;
        }
        // test
        //TestConnect();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isMyTurn)
        {
            foreach (Vector3 pos in fieldInPos)
            {
                Debug.Log(pos);
            }
            bool isNotIn = !fieldInPos.Contains(cloneHightLight.transform.position);
            if (isNotIn)
            {
                GameObject newBlock = PhotonNetwork.Instantiate(selectBlock.name, cloneHightLight.transform.position, whiteBlock.transform.rotation);
                GomokuStone stone = new GomokuStone();
                stone.Color = GomokuColor.None;
                stone.XPos = (int)newBlock.transform.position.x + 7;
                stone.YPos = (int)newBlock.transform.position.y + 7;
                if (selectBlock == blackBlock)
                {
                    stone.Color = GomokuColor.Black;
                }
                else
                {
                    stone.Color = GomokuColor.White;
                }
                gmHDG.board[stone.XPos, stone.YPos] = stone;
                photonView.RPC("AppendFieldPos", RpcTarget.All, cloneHightLight.transform.position);
                photonView.RPC("IsMyTurn", RpcTarget.Others);
                isMyTurn = false;

                bool isWin = gmHDG.CheckWin(stone);
                Debug.Log("이겼나요? " + (isWin ? "그래" : "아니야"));

            }
            else
            {
                Debug.Log("Do not position");
                // message go
            }
        }
    }

    private void LateUpdate()
    {
        Vector3 mousePoint = cam.ScreenToWorldPoint(Input.mousePosition);
        float minDis = 0;
        foreach (Vector3 pos in fieldPos)
        {
            if (minDis == 0)
            {
                minDis = (pos - mousePoint).magnitude;
                continue;
            }
            float dis = (pos - mousePoint).magnitude;
            if (dis < minDis)
            {
                minDis = dis;
                minVector = pos;
            }
        }
        cloneHightLight.transform.position = minVector;
    }


    /// <summary>
    /// test
    /// </summary>

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        SceneManager.LoadScene(0);
    }

    //private string nick = string.Empty;

    //private void TestConnect()
    //{
    //    PhotonNetwork.GameVersion = "0.0.1";
    //    PhotonNetwork.ConnectUsingSettings();
    //}

    //public override void OnConnectedToMaster()
    //{
    //    nick = Random.Range(1, 100).ToString();
    //    Debug.Log("OnConnectedToMaster");
    //    PhotonNetwork.JoinOrCreateRoom("test_room",
    //                             new RoomOptions
    //                             {
    //                                 MaxPlayers = 2,
    //                                 BroadcastPropsChangeToAll = false
    //                             }, null);
    //}

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("OnJoinedRoom");
    //    photonView.RPC("SelectBlock", RpcTarget.All);
    //}

    //public override void OnPlayerEnteredRoom(Player otherPlayer)
    //{
    //    Debug.LogFormat("OnPlayerEnteredRoom" + otherPlayer.NickName);
    //}

    [PunRPC]
    public void IsMyTurn()
    {
        isMyTurn = true;
    }

    [PunRPC]
    public void AppendFieldPos(Vector3 pos)
    {
        fieldInPos.Add(pos);
    }
}
