using System.Collections.Generic;
using Data;

namespace Helpers
{
    public static class CardDeckBuilder
    {
        public static List<CardData> BuildShuffledDeck(int pairsNeeded, List<CardData> pool)
        {
            var chosen = new HashSet<int>();
            while (chosen.Count < pairsNeeded)
                chosen.Add(UnityEngine.Random.Range(0, pool.Count));

            var deck = new List<CardData>(pairsNeeded * 2);

            foreach (var idx in chosen)
            {
                var data = pool[idx];
                deck.Add(data);
                deck.Add(data);
            }

            for (var i = deck.Count - 1; i > 0; i--)
            {
                var j = UnityEngine.Random.Range(0, i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }

            return deck;
        }

        public static List<CardData> GetDeckFromLoaded(List<CardData> imageData, GameSaveData cachedSave)
        {
            var deck = new List<CardData>(cachedSave.Cards.Count);
            foreach (var cardSaveData in cachedSave.Cards)
            {
                var cardData = imageData.Find(data => data.Id == cardSaveData.CardId);
                deck.Add(cardData);
            }

            return deck;
        }
    }
}