using System.Collections.Generic;
using Data;
using UnityEngine;

namespace ImageLoad
{
    public static class ImageLoader
    {
        public static List<SpriteData> LoadAllImages()
        {
            var sprites = Resources.LoadAll<Sprite>("CardSprites");
            var result = new List<SpriteData>(sprites.Length);

            foreach (var sprite in sprites)
            {
                if (sprite == null) continue;
                result.Add(new SpriteData(sprite.name, sprite));
            }

            return result;
        }
    }
}