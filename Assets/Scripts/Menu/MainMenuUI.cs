using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private const string _GAMESCENE = "Game";

    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private Button _settingsButton;

    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private GameObject _settingsPanel;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _settingsButton.onClick.AddListener(() => { SetSettingPanel(true); });
        _backButton.onClick.AddListener(() => { SetSettingPanel(false); });

    }
    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene(_GAMESCENE);
    }
    public void SetSettingPanel(bool setTo)
    {
        _settingsPanel.SetActive(setTo);
    }

}
