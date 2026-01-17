using UnityEngine;

public sealed class GameModeToggle : MonoBehaviour
{
    public GameFieldSize FieldSize;
}

public enum GameFieldSize
{
    Size2x3,
    Size4x4,
    Size4x6,
    Size6x6,
}
