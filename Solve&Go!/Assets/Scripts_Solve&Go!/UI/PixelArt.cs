using System.Collections.Generic;
using UnityEngine;

// Builds small sprites in code so the HUD needs no extra image files.
// Pixel art is written as rows of characters, one character per pixel,
// top row first. Each character maps to a colour in a palette.
public static class PixelArt
{
    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    static readonly Dictionary<char, Color32> skullPalette = new Dictionary<char, Color32>
    {
        { 'K', new Color32(24, 16, 30, 255) },    // outline / eye sockets
        { 'W', new Color32(244, 240, 228, 255) }, // bone
        { 'S', new Color32(190, 182, 172, 255) }, // shading
    };

    static readonly string[] skullRows =
    {
        "....KKKKKKK....",
        "..KKWWWWWWWKK..",
        ".KWWWWWWWWWWSK.",
        ".KWWWWWWWWWWSK.",
        "KWWWWWWWWWWWWSK",
        "KWWKKKWWWKKKWSK",
        "KWKKKKKWKKKKKSK",
        "KWKKKKKWKKKKKSK",
        "KWWKKKWWWKKKWSK",
        ".KWWWWWKWWWWSK.",
        "..KSWWKKKWWSK..",
        "...KWWWWWWSK...",
        "...KWKWKWKWK...",
        "...KWKWKWKSK...",
        "....KKKKKKK....",
    };

    static readonly Dictionary<char, Color32> coinPalette = new Dictionary<char, Color32>
    {
        { 'K', new Color32(70, 40, 10, 255) },
        { 'Y', new Color32(255, 204, 40, 255) },
        { 'O', new Color32(214, 136, 20, 255) },
        { 'W', new Color32(255, 250, 205, 255) },
    };

    static readonly string[] coinRows =
    {
        "...KKKKKK...",
        "..KYYYYYYK..",
        ".KYWWYYYYOK.",
        "KYWYYYYYYYOK",
        "KYWYYYYYYYOK",
        "KYYYYYYYYYOK",
        "KYYYYYYYYYOK",
        "KYYYYYYYYYOK",
        "KYYYYYYYYOOK",
        ".KYYYYYYOOK.",
        "..KOOOOOOK..",
        "...KKKKKK...",
    };

    public static Sprite Skull() { return FromRows("skull", skullRows, skullPalette); }

    public static Sprite Coin() { return FromRows("coin", coinRows, coinPalette); }

    public static Sprite FromRows(string key, string[] rows, Dictionary<char, Color32> palette)
    {
        Sprite cached;
        if (cache.TryGetValue(key, out cached) && cached != null) { return cached; }

        int h = rows.Length;
        int w = rows[0].Length;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color32[] pixels = new Color32[w * h];
        for (int y = 0; y < h; y++)
        {
            string row = rows[h - 1 - y];   // textures start at the bottom
            for (int x = 0; x < w; x++)
            {
                Color32 c;
                pixels[y * w + x] = (x < row.Length && palette.TryGetValue(row[x], out c)) ? c : new Color32(0, 0, 0, 0);
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
        cache[key] = sprite;
        return sprite;
    }

    // A white rounded rectangle for Image.type = Sliced. Tint it with Image.color.
    public static Sprite RoundedPanel()
    {
        Sprite cached;
        if (cache.TryGetValue("panel", out cached) && cached != null) { return cached; }

        const int size = 32;
        const int radius = 12;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                Vector2 nearest = new Vector2(Mathf.Clamp(p.x, radius, size - radius), Mathf.Clamp(p.y, radius, size - radius));
                float alpha = Mathf.Clamp01(radius - Vector2.Distance(p, nearest) + 0.5f);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0,
            SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
        cache["panel"] = sprite;
        return sprite;
    }
}
