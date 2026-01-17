using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopHudView : MonoBehaviour
{
    [SerializeField] private Button _toMainMenuButton;

    [SerializeField] private GameManager _manager;

    private void OnEnable()
    {
        _toMainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnDisable()
    {
        _toMainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
    }

    private void OnMainMenuButtonClicked()
    {
        _manager.ToMainMenu();
    }
}