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
            Debug.LogError("Start Button이 연결되지 않았습니다.");
            return;
        }
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    // 시작 버튼 클릭 시 로그인 씬으로 전환
    private void OnStartButtonClicked()
    {
        Debug.Log("시작 버튼 클릭됨. 로그인 씬으로 이동합니다.");
        SceneManager.LoadScene(1);
    }
}
