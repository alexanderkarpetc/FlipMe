using System.Collections.Generic;
using Data;
using UnityEngine;

namespace ImageLoad
{
    public static class ImageLoader
    {
        public static List<CardData> LoadAllImages()
        {
            var sprites = Resources.LoadAll<Sprite>("CardSprites");
            var result = new List<CardData>(sprites.Length);

            foreach (var sprite in sprites)
            {
                if (sprite == null) continue;
                result.Add(new CardData(sprite.name, sprite));
            }

            return result;
        }
    }
}