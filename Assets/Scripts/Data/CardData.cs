using UnityEngine;

namespace Data
{
    public readonly struct CardData
    {
        public readonly string Id;
        public readonly Sprite Sprite;

        public CardData(string id, Sprite sprite)
        {
            Id = id;
            Sprite = sprite;
        }
    }
}