using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginSceneController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputPlayerName;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (confirmButton == null || inputPlayerName == null)
        {
            Debug.LogError("InputField �Ǵ� Confirm Button�� ������� �ʾҽ��ϴ�.");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator�� ������� �ʾҽ��ϴ�.");
            return;
        }

        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    private void OnConfirmClicked()
    {
        string playerName = inputPlayerName.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("�г��� ���Է�");
            return;
        }

        PhotonNetwork.NickName = playerName;
        confirmButton.interactable = false;  
        animator.SetBool("ConfirmClicked", true);
    }

    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("Lobby");
    }
}