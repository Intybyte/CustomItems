using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomItems.Registry
{
    public class SpriteRegistry
    {
        public static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

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
                    sprites[extensionLess] = sprite.Scale(4);
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
