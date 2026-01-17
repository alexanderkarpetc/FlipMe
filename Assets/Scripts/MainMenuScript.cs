using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameManager _manager;
    
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _loadButton;
    
    [SerializeField] private ToggleGroup _toggleGroup;

    private void OnEnable()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
        _loadButton.onClick.RemoveListener(OnLoadButtonClicked);
    }

    private void OnLoadButtonClicked()
    {
        _manager.LoadGame();
    }

    private void OnStartButtonClicked()
    {
        var fieldSize = GameFieldSize.Size2x3;
        foreach (var toggle in _toggleGroup.ActiveToggles())
        {
            fieldSize = toggle.GetComponent<GameModeToggle>().FieldSize;
            break;
        }
        _manager.StartGame(fieldSize);
    }
}
