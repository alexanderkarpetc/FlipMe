using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Helpers;
using ImageLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Views;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private CardView _cardPrefab;
    [SerializeField] private Transform _mainMenu;
    [SerializeField] private Transform _playField;
    [SerializeField] private Transform _topHud;
    [SerializeField] private Transform _loadGameButton;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Transform _winText;

    private readonly List<CardView> _spawned = new();
    private List<CardData> _imageData;
    private CardView _firstCard;
    private SaveManager _saveManager = new();
    private GameSaveData _cachedSave;
    private int _score;
    private bool _wasSuccessfulMatch;

    private void Start()
    {
        _imageData = ImageLoader.LoadAllImages();
        _cachedSave = _saveManager.TryLoad();
        _loadGameButton.gameObject.SetActive(_cachedSave != null);
    }

    public void StartGame(GameFieldSize fieldSize)
    {
        SetActiveField(true);

        var (columns, rows) = GetGridSize(fieldSize);
        ApplyGridLayout(columns, rows);

        var count = columns * rows;
        var pairsNeeded = count / 2;
        if (_imageData == null || _imageData.Count < pairsNeeded)
            throw new InvalidOperationException($"Not enough sprites for pairs. Need={pairsNeeded}, Have={_imageData?.Count ?? 0}");

        var deck = CardDeckBuilder.BuildShuffledDeck(pairsNeeded, _imageData);
        SpawnCards(deck);
        _score = Constants.StartingScore;
        _scoreText.text = $"Score: {_score}";
        _saveManager.SaveInitialState(columns, rows, deck);
    }

    public void ToMainMenu()
    {
        SetActiveField(false);
        CleanupField();
        _cachedSave = _saveManager.TryLoad();
        _loadGameButton.gameObject.SetActive(_cachedSave != null);
    }

    public void LoadGame()
    {
        SetActiveField(true);
        if (_cachedSave == null)
        {
            Debug.LogError("No saved game found.");
            return;
        }
        ApplyGridLayout(_cachedSave.Columns, _cachedSave.Rows);
        var deck = CardDeckBuilder.GetDeckFromLoaded(_imageData, _cachedSave);
        SpawnCards(deck);
        foreach (var cardView in _spawned)
        {
            if (_cachedSave.Cards.Find(c => c.CardId == cardView.Data.Id)?.IsRevealed == true)
            {
                cardView.Complete();
            }
        }
        _score = _cachedSave.Score;
        _wasSuccessfulMatch = _cachedSave.WasSuccessfulMatch;
        _scoreText.text = $"Score: {_score}";
    }

    private void SpawnCards(List<CardData> deck)
    {
        var parent = _gridLayoutGroup.transform;

        for (var i = 0; i < deck.Count; i++)
        {
            var card = Instantiate(_cardPrefab, parent);
            _spawned.Add(card);
            card.OnClicked += OnCardClicked;

            card.Init(deck[i]);
        }
    }

    private void OnCardClicked(CardView view)
    {
        _audioManager.PlayFlipSound();
        if (_firstCard == null)
        {
            _firstCard = view;
            return;
        }
        StartCoroutine(CheckCardsMatchRoutine(view));
    }

    private IEnumerator CheckCardsMatchRoutine(CardView view)
    {
        SetCardLock(true);

        yield return new WaitForSeconds(_cardPrefab.FlipDuration);

        if (_firstCard?.Data.Id == view.Data.Id)
        {
            _firstCard.Complete();
            view.Complete();
            if (_wasSuccessfulMatch)
            {
                _score += Constants.SuccessReveal * Constants.ComboMultiplier;
                _scoreText.text = $" COMBO !!! Score: {_score}";
            }
            else
            {
                _score += Constants.SuccessReveal;
                _scoreText.text = $"Score: {_score}";
            }
            _wasSuccessfulMatch = true;
            _saveManager.UpdateCardAsRevealed(_firstCard.Data.Id);
            _saveManager.UpdateScore(_score, _wasSuccessfulMatch);
            _audioManager.PlayMatchSound();
            CheckGameWin();
        }
        else
        {
            _firstCard.Hide();
            view.Hide();
            _wasSuccessfulMatch = false;
            _score -= Constants.FailCost;
            _scoreText.text = $"Score: {_score}";
            _saveManager.UpdateScore(_score, _wasSuccessfulMatch);
            _audioManager.PlayUnMatchSound();
        }

        SetCardLock(false);
        _firstCard = null;
    }

    private void CheckGameWin()
    {
        foreach (var spawned in _spawned)
        {
            if (!spawned.IsRevealed)
                return;
        }

        _audioManager.PlayWinSound();
        _winText.gameObject.SetActive(true);
    }

    private void SetCardLock(bool value)
    {
        foreach (var spawned in _spawned)
        {
            spawned.SetIsLocked(value);
        }
    }

    private void ApplyGridLayout(int columns, int rows)
    {
        var rect = _gridLayoutGroup.GetComponent<RectTransform>().rect;

        var padding = _gridLayoutGroup.padding;
        var spacing = _gridLayoutGroup.spacing;

        var availableWidth = rect.width - padding.left - padding.right - spacing.x * (columns - 1);

        var availableHeight = rect.height - padding.top - padding.bottom - spacing.y * (rows - 1);

        var cellWidth = availableWidth / columns;
        var cellHeight = availableHeight / rows;

        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = columns;
        _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }

    private static (int columns, int rows) GetGridSize(GameFieldSize size) =>
        size switch
        {
            GameFieldSize.Size2x3 => (3, 2),
            GameFieldSize.Size4x4 => (4, 4),
            GameFieldSize.Size4x6 => (6, 4),
            GameFieldSize.Size6x6 => (6, 6),
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };

    private void CleanupField()
    {
        for (var i = 0; i < _spawned.Count; i++)
        {
            if (_spawned[i] != null)
            {
                _spawned[i].OnClicked -= OnCardClicked;
                Destroy(_spawned[i].gameObject);
            }
        }
        _spawned.Clear();
    }
    
    private void SetActiveField(bool value)
    {
        _mainMenu.gameObject.SetActive(!value);
        _playField.gameObject.SetActive(value);
        _topHud.gameObject.SetActive(value);
        _winText.gameObject.SetActive(false);
    }
}
