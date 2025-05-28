using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GomokuManager;

public class GamestartGameManager : MonoBehaviourPunCallbacks
{
    // �ڵ��� ����
    [SerializeField] private LineRenderer lineRenderer;
    // ���콺 ��ġ������ ���� ����� ���� ������Ʈ
    [SerializeField] private GameObject hightLight;
    // �� �ٵϾ�
    [SerializeField] private GameObject whiteBlock;
    // ���� �ٵϾ�
    [SerializeField] private GameObject blackBlock;
    // �޴�
    [SerializeField] private Canvas menuCanvas;
    // ���� ���� ��ư
    [SerializeField] private Button exitGameBtn;
    // ���� �¸��� ǥ�� TMP
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    //����Ʈ ������
    [SerializeField] private GameObject[] placeEffectPrefabs;
    
    
    private Vector3 startPos = Vector3.zero;
    Vector3 minVector = Vector3.zero;
    private Camera cam;
    private GameObject cloneHightLight;

    // �ʵ忡 ������ �ִ� �����ǵ�
    private Vector3[] fieldPos = null;
    // �ʵ忡 ���� �� �����ǵ�
    private List<Vector3> fieldInPos = null;
    // �浹 �÷��̾� ����
    private bool isFirst = true;
    // ���� �Ҹ������� ���õ� ��
    private GameObject selectBlock = null;
    // �� ��������
    private bool isMyTurn = false;
    // �̱涧���� ���� �ϴ� �Ҹ�
    private bool isWin = false;

    private GomokuManager gmHDG = new GomokuManager();

    private void Start()
    {
        menuCanvas.gameObject.SetActive(false);
        textMeshProUGUI.gameObject.SetActive(false);

        exitGameBtn.onClick.AddListener(() =>
        {
            photonView.RPC("ExitGameRoom", RpcTarget.All);
        });

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
            Vector3 placePos = cloneHightLight.transform.position;

            bool isNotIn = !fieldInPos.Contains(cloneHightLight.transform.position);
            if (isNotIn)
            {
                GameObject newBlock = PhotonNetwork.Instantiate(selectBlock.name, cloneHightLight.transform.position, whiteBlock.transform.rotation);

                if (placeEffectPrefabs != null && placeEffectPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, placeEffectPrefabs.Length);
                    GameObject effectPrefab = placeEffectPrefabs[randomIndex];

                    PhotonNetwork.Instantiate(effectPrefab.name, placePos, Quaternion.identity);
                }

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

                isWin = gmHDG.CheckWin(stone);
                if (isWin)
                {
                    string text = selectBlock == blackBlock ? "Black Win" : "White Win";
                    photonView.RPC("WinerPlay", RpcTarget.All, text);
                }
            }
            else
            {
                Debug.Log("Do not position");
                // message go
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isWin)
        {
            menuCanvas.gameObject.SetActive(!menuCanvas.gameObject.activeSelf);
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
        SceneManager.LoadScene(2);
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

    [PunRPC]
    public void ExitGameRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void WinerPlay(string text)
    {
        menuCanvas.gameObject.SetActive(true);
        textMeshProUGUI.gameObject.SetActive(true);
        textMeshProUGUI.text = text;
    }
}
