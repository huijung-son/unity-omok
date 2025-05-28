using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private Button startButton;
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

    private void OnStartButtonClicked()
    {
        animator.SetBool("StartClicked", true);
        startButton.interactable = false; 
    }

    public void OnAnimationEnd()
    {
        SceneManager.LoadScene("Login");
    }
}
