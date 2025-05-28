using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        if (startButton == null)
        {
            Debug.LogError("Start Button ���� �ʿ�");
            return;
        }

        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Login");
    }
}
