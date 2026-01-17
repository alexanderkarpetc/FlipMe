using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private CardView _cardPrefab;

    public void StartGame(GameFieldSize fieldSize)
    {
        Debug.Log($"Start Game with field size: {fieldSize}");

        var (columns, rows) = GetGridSize(fieldSize);
        ApplyGridLayout(columns, rows);
    }

    public void LoadGame()
    {
        Debug.Log("Load Game");
    }

    private void ApplyGridLayout(int columns, int rows)
    {
        var rect = _gridLayoutGroup.GetComponent<RectTransform>().rect;

        var padding = _gridLayoutGroup.padding;
        var spacing = _gridLayoutGroup.spacing;

        var availableWidth =
            rect.width
            - padding.left - padding.right
            - spacing.x * (columns - 1);

        var availableHeight =
            rect.height
            - padding.top - padding.bottom
            - spacing.y * (rows - 1);

        var cellWidth = availableWidth / columns;
        var cellHeight = availableHeight / rows;

        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = columns;
        _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }

    private static (int columns, int rows) GetGridSize(GameFieldSize size)
    {
        return size switch
        {
            GameFieldSize.Size2x3 => (2, 3),
            GameFieldSize.Size4x4 => (4, 4),
            GameFieldSize.Size4x6 => (4, 6),
            GameFieldSize.Size6x6 => (6, 6),
            _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
        };
    }
}