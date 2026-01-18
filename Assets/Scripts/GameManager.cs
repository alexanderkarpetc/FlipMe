using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Helpers;
using ImageLoad;
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

    private readonly List<CardView> _spawned = new();
    private List<CardData> _imageData;
    private CardView _firstCard;

    private void Start()
    {
        _imageData = ImageLoader.LoadAllImages();
    }

    public void StartGame(GameFieldSize fieldSize)
    {
        Debug.Log($"Start Game with field size: {fieldSize}");
        _mainMenu.gameObject.SetActive(false);
        _playField.gameObject.SetActive(true);
        _topHud.gameObject.SetActive(true);

        var (columns, rows) = GetGridSize(fieldSize);
        ApplyGridLayout(columns, rows);

        var count = columns * rows;
        SpawnCards(count);
    }

    public void ToMainMenu()
    {
        _playField.gameObject.SetActive(false);
        _mainMenu.gameObject.SetActive(true);
        _topHud.gameObject.SetActive(false);
        CleanupField();
    }
    public void LoadGame()
    {
        throw new NotImplementedException();
    }

    private void SpawnCards(int count)
    {
        var pairsNeeded = count / 2;

        if (_imageData == null || _imageData.Count < pairsNeeded)
            throw new InvalidOperationException($"Not enough sprites for pairs. Need={pairsNeeded}, Have={_imageData?.Count ?? 0}");

        var deck = CardDeckBuilder.BuildShuffledDeck(pairsNeeded, _imageData);

        var parent = _gridLayoutGroup.transform;

        for (var i = 0; i < count; i++)
        {
            var card = Instantiate(_cardPrefab, parent);
            _spawned.Add(card);
            card.OnClicked += OnCardClicked;

            card.Init(deck[i]);
        }
    }

    private void OnCardClicked(CardView view)
    {
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
        }
        else
        {
            _firstCard.Hide();
            view.Hide();
        }

        SetCardLock(false);
        _firstCard = null;
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
}
