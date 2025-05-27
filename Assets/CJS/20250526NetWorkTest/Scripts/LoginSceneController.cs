using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    // 확인 버튼 클릭 시 닉네임 저장 후 로비 씬으로 이동
    private void OnConfirmButtonClicked()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("이름을 입력해주세요.");
            return;
        }

        PhotonNetwork.NickName = playerName;
        Debug.Log($"닉네임 설정 완료: {playerName}");

        SceneManager.LoadScene(2);
    }
}
