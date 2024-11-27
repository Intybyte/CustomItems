using CustomItems.Utils;
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
    }
}
