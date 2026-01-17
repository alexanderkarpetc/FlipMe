using System.Collections.Generic;
using Data;

namespace Helpers
{
    public static class CardDeckBuilder
    {
        public static List<SpriteData> BuildShuffledDeck(int pairsNeeded, List<SpriteData> pool)
        {
            var chosen = new HashSet<int>();
            while (chosen.Count < pairsNeeded)
                chosen.Add(UnityEngine.Random.Range(0, pool.Count));

            var deck = new List<SpriteData>(pairsNeeded * 2);

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
    }
}