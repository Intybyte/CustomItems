using CustomItems.Registry;
using UnityEngine;

namespace CustomItems
{
    public static class SpriteUtil
    {
        public static readonly Color background = new Color(37f / 255f, 19f / 255f, 26f / 255f, 0);

        #region Colors

        public static Sprite SetColor(string sprite, int add_r, int add_g, int add_b)
        {
            return SpriteRegistry.sprites[sprite].SetColor(add_r, add_g, add_b);
        }

        public static Sprite SetColor(this Sprite sprite, int set_r, int set_g, int set_b) 
        {
            var texture = sprite.texture;

            float red = set_r / 255f;
            float green = set_g / 255f;
            float blue = set_b / 255f;

            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {

                    Color pixel = texture.GetPixel(x, y);
                    if (IsBackground(pixel))
                    {
                        newTexture.SetPixel(x, y, background);
                        continue;
                    }

                    newTexture.SetPixel(x, y, new Color(red, green, blue));
                }
            }

            newTexture.Apply();

            return Sprite.Create(
                newTexture,
                sprite.textureRect,
                new Vector2(0.5f, 0.5f),
                sprite.pixelsPerUnit
            );
        }

        public static Sprite AddColor(string sprite, int add_r, int add_g, int add_b)
        {
            return SpriteRegistry.sprites[sprite].AddColor(add_r, add_g, add_b);
        }

        public static Sprite AddColor(this Sprite sprite, int add_r, int add_g, int add_b)
        {
            var texture = sprite.texture;

            float red = add_r / 255f;
            float green = add_g / 255f;
            float blue = add_b / 255f;

            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {

                    Color pixel = texture.GetPixel(x, y);
                    if (IsBackground(pixel))
                    {
                        newTexture.SetPixel(x, y, background);
                        continue;
                    }

                    var replaceRed = Mathf.Clamp(pixel.r + red, 0f, 1f);
                    var replaceGreen = Mathf.Clamp(pixel.g + green, 0f, 1f);
                    var replaceBlue = Mathf.Clamp(pixel.b + blue, 0f, 1f);

                    newTexture.SetPixel(x, y, new Color(replaceRed, replaceGreen, replaceBlue));
                }
            }

            newTexture.Apply();

            return Sprite.Create(
                newTexture,
                sprite.textureRect,
                new Vector2(0.5f, 0.5f),
                sprite.pixelsPerUnit
            );
        }

        #endregion

        #region Scale
        public static Sprite Scale(string sprite, int scale)
        {
            return SpriteRegistry.sprites[sprite].Scale(scale);
        }
        public static Sprite Scale(this Sprite sprite, int scale)
        {
            var texture = sprite.texture;
            int newWidth = texture.width * scale;
            int newHeight = texture.height * scale;

            Texture2D newTexture = new Texture2D(newWidth, newHeight);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color originalPixel = texture.GetPixel(x, y);

                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            newTexture.SetPixel(x * scale + sx, y * scale + sy, originalPixel);
                        }
                    }
                }
            }

            newTexture.Apply();

            Rect newRect = new Rect(0, 0, newWidth, newHeight);

            return Sprite.Create(
                newTexture,
                newRect,
                new Vector2(0.5f, 0.5f),
                sprite.pixelsPerUnit
            );
        }
        #endregion

        public static bool IsBackground(this Color color) =>
            Mathf.Approximately(color.r, background.r) &&
            Mathf.Approximately(color.g, background.g) &&
            Mathf.Approximately(color.b, background.b);
    }
}
