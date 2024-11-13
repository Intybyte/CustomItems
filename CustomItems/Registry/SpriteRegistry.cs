using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomItems.Registry
{
    public class SpriteRegistry
    {
        public static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        public static readonly Color background = new Color(37f / 255f, 19f / 255f, 26f / 255f, 0);

        #region Color
        public static Sprite AddColor(string sprite, int add_r, int add_g, int add_b)
        {
            return AddColor(sprites[sprite], add_r, add_g, add_b);
        }

        public static Sprite AddColor(Sprite sprite, int add_r, int add_g, int add_b)
        {
            var texture = sprite.texture;

            float red = add_r / 255f;
            float green = add_g / 255f;
            float blue = add_b / 255f;

            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            newTexture.filterMode = FilterMode.Point;
            newTexture.wrapMode = TextureWrapMode.Clamp;

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

        #region Scaling
        public static Sprite Scale(string sprite, int scale) 
        {
            return Scale(sprites[sprite], scale);
        }
        public static Sprite Scale(Sprite sprite, int scale)
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

        public static bool IsBackground(Color color) =>
            Mathf.Approximately(color.r, background.r) &&
            Mathf.Approximately(color.g, background.g) &&
            Mathf.Approximately(color.b, background.b);

        #region Loading data
        public static void Load() 
        {
            Debug.Log("Init sprites");
            // Get the current executing assembly
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get all resource names from the assembly
            string[] resourceNames = assembly.GetManifestResourceNames();

            // Filter resource names that start with the given path and are image files
            var imageResourceNames = resourceNames
                .Where(name => (name.EndsWith(".png") || name.EndsWith(".jpg") || name.EndsWith(".bmp") ));

            foreach (string resourceName in imageResourceNames)
            {
                var extensionLess = Sanitize(resourceName);

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Debug.Log($"Resource {resourceName} not found.");
                        continue;
                    }

                    // Create Texture2D and load image data into it
                    byte[] imageData = ReadFully(stream);
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    texture.filterMode = FilterMode.Point;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    ImageConversion.LoadImage(texture, imageData);
                    texture.Apply();

                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    Debug.Log($"Loaded Sprite {resourceName} with ID {extensionLess}.");
                    sprites[extensionLess] = sprite;
                }
            }
        }

        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static string Sanitize(string s) {
            string[] stringList = s.Split('.');
            string result = stringList[stringList.Length - 2];
            return result;
        }
        #endregion

        #region yoink images as an example, unused but staying here rent free
        public static void Dump(InventoryItem item) 
        {
            ExportSpriteToBMP(item.sprite, item.effectName);
        }

        private static void ExportSpriteToBMP(Sprite sprite, string spriteName)
        {

            Texture2D texture = sprite.texture;
            Color32[] pixels = texture.GetPixels32();
            int width = texture.width;
            int height = texture.height;

            // Define the save path
            string filePath = Path.Combine(Paths.PluginPath, $"{spriteName}.bmp");

            // Write BMP file
            WriteBMP(filePath, pixels, width, height);
        }

        private static void WriteBMP(string filePath, Color32[] pixels, int width, int height)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                int fileSize = 54 + (width * height * 3); // BMP header + pixel data (3 bytes per pixel)

                // BMP Header
                writer.Write((byte)'B');
                writer.Write((byte)'M');
                writer.Write(fileSize);
                writer.Write(0);
                writer.Write(54); // Pixel data offset

                // DIB Header
                writer.Write(40); // DIB header size
                writer.Write(width);
                writer.Write(height);
                writer.Write((short)1); // Planes
                writer.Write((short)24); // Bits per pixel
                writer.Write(0); // Compression
                writer.Write(width * height * 3); // Image size
                writer.Write(0); // Horizontal resolution
                writer.Write(0); // Vertical resolution
                writer.Write(0); // Colors in palette
                writer.Write(0); // Important colors

                // Pixel Data (bottom-to-top row order)
                for (int y = height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color32 color = pixels[y * width + x];
                        writer.Write(color.b); // BMP format is BGR
                        writer.Write(color.g);
                        writer.Write(color.r);
                    }
                }
            }
        }
        #endregion
    }
}
