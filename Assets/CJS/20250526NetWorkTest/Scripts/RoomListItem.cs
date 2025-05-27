using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // 방 이름과 인원 정보를 설정하고 클릭 이벤트 등록
    public void SetRoomInfo(string roomName, int playerCount, int maxPlayers, System.Action<string> onClickCallback)
    {
        roomNameText.text = roomName;
        playerCountText.text = $"{playerCount} / {maxPlayers}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(roomName));
    }
}
