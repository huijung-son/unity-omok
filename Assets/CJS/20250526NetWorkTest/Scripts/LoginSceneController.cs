using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginSceneController : MonoBehaviour
{
    //닉네임 입력
    [SerializeField] private TMP_InputField inputPlayerName;
    //확인버튼
    [SerializeField] private Button confirmButton;
    //애니메이션
    [SerializeField] private Animator animator;

    private void Awake()
    {
        //닉네임입력
        if (confirmButton == null || inputPlayerName == null)
        {
            Debug.LogError("InputField 또는 Confirm Button이 연결되지 않았습니다.");
            return;
        }
        //애니메이션 연결
        if (animator == null)
        {
            Debug.LogError("Animator가 연결되지 않았습니다.");
            return;
        }
        
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    //닉네임 입력
    private void OnConfirmClicked()
    {
        string playerName = inputPlayerName.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("닉네임 미입력");
            return;
        }
        //닉네임 동기화
        PhotonNetwork.NickName = playerName;
        confirmButton.interactable = false;  

        //클릭시 애니메이션재생
        animator.SetBool("ConfirmClicked", true);
    }

    //씬바뀜
    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("Lobby");
    }
}