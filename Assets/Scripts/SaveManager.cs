using System.Collections.Generic;
using Data;
using UnityEngine;

public class SaveManager
{
    private const string GameStateKey = "GameState";
    public void SaveInitialState(int columns, int rows, List<CardData> deck)
    {
        var cardSaveDatas = new List<CardSaveData>();
        foreach (var card in deck)
        {
            var cardSaveData = new CardSaveData
            {
                CardId = card.Id,
                IsRevealed = false
            };
            cardSaveDatas.Add(cardSaveData);
        }

        var saveData = new GameSaveData
        {
            Columns = columns,
            Rows = rows,
            Cards = cardSaveDatas
        };

        PlayerPrefs.SetString(GameStateKey, JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
    }

    public GameSaveData TryLoad()
    {
        if (!PlayerPrefs.HasKey(GameStateKey))
            return null;

        var dataString = PlayerPrefs.GetString(GameStateKey);
        var save = JsonUtility.FromJson<GameSaveData>(dataString);

        return save;
    }

    public void UpdateCardAsRevealed(string cardId)
    {
        var saveData = TryLoad();
        if (saveData == null)
        {
            Debug.LogError("Cannot update card state, no save data found.");
            return;
        }

        foreach (var cardSaveData in saveData.Cards)
        {
            if (cardSaveData.CardId == cardId)
            {
                cardSaveData.IsRevealed = true;
                break;
            }
        }

        PlayerPrefs.SetString(GameStateKey, JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
    }
}