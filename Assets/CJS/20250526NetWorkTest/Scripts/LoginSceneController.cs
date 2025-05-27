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

    // Ȯ�� ��ư Ŭ�� �� �г��� ���� �� �κ� ������ �̵�
    private void OnConfirmButtonClicked()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("�̸��� �Է����ּ���.");
            return;
        }

        PhotonNetwork.NickName = playerName;
        Debug.Log($"�г��� ���� �Ϸ�: {playerName}");

        SceneManager.LoadScene(2);
    }
}
