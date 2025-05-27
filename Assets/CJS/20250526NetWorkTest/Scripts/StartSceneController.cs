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
            Debug.LogError("Start Button�� ������� �ʾҽ��ϴ�.");
            return;
        }
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    // ���� ��ư Ŭ�� �� �α��� ������ ��ȯ
    private void OnStartButtonClicked()
    {
        Debug.Log("���� ��ư Ŭ����. �α��� ������ �̵��մϴ�.");
        SceneManager.LoadScene(1);
    }
}
