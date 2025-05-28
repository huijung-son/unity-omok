using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginSceneController : MonoBehaviour
{
    //�г��� �Է�
    [SerializeField] private TMP_InputField inputPlayerName;
    //Ȯ�ι�ư
    [SerializeField] private Button confirmButton;
    //�ִϸ��̼�
    [SerializeField] private Animator animator;

    private void Awake()
    {
        //�г����Է�
        if (confirmButton == null || inputPlayerName == null)
        {
            Debug.LogError("InputField �Ǵ� Confirm Button�� ������� �ʾҽ��ϴ�.");
            return;
        }
        //�ִϸ��̼� ����
        if (animator == null)
        {
            Debug.LogError("Animator�� ������� �ʾҽ��ϴ�.");
            return;
        }
        
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    //�г��� �Է�
    private void OnConfirmClicked()
    {
        string playerName = inputPlayerName.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("�г��� ���Է�");
            return;
        }
        //�г��� ����ȭ
        PhotonNetwork.NickName = playerName;
        confirmButton.interactable = false;  

        //Ŭ���� �ִϸ��̼����
        animator.SetBool("ConfirmClicked", true);
    }

    //���ٲ�
    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("Lobby");
    }
}