using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private const string _GAMESCENE = "Game";

    [SerializeField]
    private Button _startButton;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }
    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(_GAMESCENE);
    }

}
