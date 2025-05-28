using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginSceneController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputPlayerName;
    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        if (confirmButton == null || inputPlayerName == null)
        {
            return;
        }

        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnConfirmClicked()
    {
        string playerName = inputPlayerName.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("닉네임 미입력");
            return;
        }

        PhotonNetwork.NickName = playerName;
        SceneManager.LoadScene("Lobby");
    }
}
