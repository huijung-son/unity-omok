using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    //시작버튼
    [SerializeField] private Button startButton;
    //넘기는애니메이션
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

    //클릭시 애니메이션 재생
    private void OnStartButtonClicked()
    {
        animator.SetBool("StartClicked", true);
        startButton.interactable = false; 
    }
    //씬넘어감
    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("Login");
    }
}
