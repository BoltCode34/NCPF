using UnityEngine;

namespace NCPF.Pipeline.Presentation
{
    /// <summary>Paints a boolean footprint into a point-filtered texture, one pixel per cell, y up.</summary>
    public static class FootprintTexture
    {
        public static Texture2D Build(bool[,] footprint, Color occupied, Color free)
        {
            Texture2D texture = Create(footprint.GetLength(0), footprint.GetLength(1));
            Fill(texture, footprint, occupied, free);
            texture.Apply();
            return texture;
        }

        public static Texture2D Create(int width, int height)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        /// <summary>Writes the pixels without applying them, so the caller can mark cells first.</summary>
        public static void Fill(Texture2D texture, bool[,] footprint, Color occupied, Color free)
        {
            int width = footprint.GetLength(0);
            int height = footprint.GetLength(1);
            Color[] pixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    pixels[x + y * width] = footprint[x, y] ? occupied : free;
                }
            }
            texture.SetPixels(pixels);
        }

        public static void Release(Texture2D texture)
        {
            if (texture == null)
            {
                return;
            }
            if (UnityEngine.Application.isPlaying)
            {
                Object.Destroy(texture);
            }
            else
            {
                Object.DestroyImmediate(texture);
            }
        }
    }
}
