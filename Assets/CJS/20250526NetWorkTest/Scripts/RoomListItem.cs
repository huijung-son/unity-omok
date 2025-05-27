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

    // �� �̸��� �ο� ������ �����ϰ� Ŭ�� �̺�Ʈ ���
    public void SetRoomInfo(string roomName, int playerCount, int maxPlayers, System.Action<string> onClickCallback)
    {
        roomNameText.text = roomName;
        playerCountText.text = $"{playerCount} / {maxPlayers}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickCallback?.Invoke(roomName));
    }
}
