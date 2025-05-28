using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    //���۹�ư
    [SerializeField] private Button startButton;
    //�ѱ�¾ִϸ��̼�
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (startButton == null)
        {
            return;
        }

        if (animator == null)
        {
            return;
        }

        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    //Ŭ���� �ִϸ��̼� ���
    private void OnStartButtonClicked()
    {
        animator.SetBool("StartClicked", true);
        startButton.interactable = false; 
    }
    //���Ѿ
    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("Login");
    }
}
