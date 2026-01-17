using UnityEngine;

namespace Data
{
    public readonly struct SpriteData
    {
        public readonly string Id;
        public readonly Sprite Sprite;

        public SpriteData(string id, Sprite sprite)
        {
            Id = id;
            Sprite = sprite;
        }
    }
}