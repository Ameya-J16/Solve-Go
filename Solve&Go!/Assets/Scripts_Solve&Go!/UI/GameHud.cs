using TMPro;
using UnityEngine;
using UnityEngine.UI;

// In-game HUD: death counter (top left), score (top right) and the current
// level (top centre), each on a dark panel so they read on any background.
// gameSession builds it in code, so it needs no prefab or scene setup.
public class GameHud : MonoBehaviour
{
    static readonly Color PanelColor = new Color(0.03f, 0.03f, 0.08f, 0.62f);
    static readonly Color CaptionColor = new Color(1f, 1f, 1f, 0.6f);
    static readonly Color DeathRed = new Color(1f, 0.22f, 0.22f);

    TextMeshProUGUI deathsText;
    TextMeshProUGUI scoreText;
    TextMeshProUGUI levelText;
    Image skull;
    Image flash;

    float pop = 1f;      // 0 -> 1 progress of the "you died" pop animation
    float flashing = 1f; // 0 -> 1 progress of the red screen flash
    float shownScore;
    int targetScore;

    public static GameHud Create(Transform parent, TMP_FontAsset font, Sprite coinIcon)
    {
        GameObject root = new GameObject("HUD", typeof(RectTransform));
        root.transform.SetParent(parent, false);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameHud hud = root.AddComponent<GameHud>();
        hud.Build(font, coinIcon != null ? coinIcon : PixelArt.Coin());
        return hud;
    }

    void Build(TMP_FontAsset font, Sprite coinIcon)
    {
        // Full-screen red flash when you die. Created first so it sits behind the panels.
        flash = NewImage("Death Flash", transform, null, new Color(DeathRed.r, 0f, 0.05f, 0f));
        RectTransform f = flash.rectTransform;
        f.anchorMin = Vector2.zero;
        f.anchorMax = Vector2.one;
        f.offsetMin = Vector2.zero;
        f.offsetMax = Vector2.zero;

        // Deaths, top left.
        RectTransform deaths = NewPanel("Deaths", new Vector2(0f, 1f), new Vector2(28f, -28f), new Vector2(330f, 118f));
        skull = NewImage("Skull", deaths, PixelArt.Skull(), Color.white);
        Place(skull.rectTransform, new Vector2(20f, 0f), new Vector2(84f, 84f));
        deathsText = NewText("Count", deaths, font, 70f, Color.white);
        Place(deathsText.rectTransform, new Vector2(122f, 12f), new Vector2(200f, 76f));
        TextMeshProUGUI deathsCaption = NewText("Caption", deaths, font, 22f, CaptionColor);
        deathsCaption.text = "DEATHS";
        deathsCaption.characterSpacing = 8f;
        Place(deathsCaption.rectTransform, new Vector2(124f, -36f), new Vector2(200f, 30f));

        // Score, top right.
        RectTransform score = NewPanel("Score", new Vector2(1f, 1f), new Vector2(-28f, -28f), new Vector2(330f, 118f));
        Image coin = NewImage("Coin", score, coinIcon, Color.white);
        coin.preserveAspect = true;
        Place(coin.rectTransform, new Vector2(24f, 0f), new Vector2(76f, 76f));
        scoreText = NewText("Points", score, font, 62f, new Color(1f, 0.86f, 0.3f));
        Place(scoreText.rectTransform, new Vector2(122f, 12f), new Vector2(200f, 76f));
        TextMeshProUGUI scoreCaption = NewText("Caption", score, font, 22f, CaptionColor);
        scoreCaption.text = "SCORE";
        scoreCaption.characterSpacing = 8f;
        Place(scoreCaption.rectTransform, new Vector2(124f, -36f), new Vector2(200f, 30f));

        // Level, top centre.
        RectTransform level = NewPanel("Level", new Vector2(0.5f, 1f), new Vector2(0f, -28f), new Vector2(300f, 70f));
        levelText = NewText("Level", level, font, 38f, Color.white);
        levelText.alignment = TextAlignmentOptions.Center;
        levelText.characterSpacing = 6f;
        RectTransform lt = levelText.rectTransform;
        lt.anchorMin = Vector2.zero;
        lt.anchorMax = Vector2.one;
        lt.offsetMin = Vector2.zero;
        lt.offsetMax = Vector2.zero;
    }

    public void SetDeaths(int deaths, bool animate)
    {
        deathsText.text = deaths.ToString();
        if (animate)
        {
            pop = 0f;
            flashing = 0f;
        }
    }

    public void SetScore(int score, bool animate)
    {
        targetScore = score;
        if (!animate) { shownScore = score; }
        scoreText.text = Mathf.RoundToInt(shownScore).ToString();
    }

    public void SetLevel(int levelNumber, int levelCount)
    {
        levelText.text = levelCount > 0 ? "LEVEL " + levelNumber + " / " + levelCount : "LEVEL " + levelNumber;
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        if (pop < 1f)
        {
            pop = Mathf.Min(1f, pop + dt / 0.5f);
            float k = (1f - pop) * (1f - pop);
            float s = 1f + 0.7f * k;
            deathsText.rectTransform.localScale = new Vector3(s, s, 1f);
            deathsText.color = Color.Lerp(Color.white, DeathRed, k);
            skull.rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(pop * 40f) * 18f * k);
            skull.rectTransform.localScale = Vector3.one * (1f + 0.35f * k);
        }

        if (flashing < 1f)
        {
            flashing = Mathf.Min(1f, flashing + dt / 0.45f);
            Color c = flash.color;
            c.a = 0.32f * (1f - flashing);
            flash.color = c;
        }

        if (!Mathf.Approximately(shownScore, targetScore))
        {
            float speed = Mathf.Max(300f, Mathf.Abs(targetScore - shownScore) * 6f);
            shownScore = Mathf.MoveTowards(shownScore, targetScore, speed * dt);
            scoreText.text = Mathf.RoundToInt(shownScore).ToString();
        }
    }

    // ---- Small UI builders ----

    RectTransform NewPanel(string name, Vector2 corner, Vector2 position, Vector2 size)
    {
        Image panel = NewImage(name, transform, PixelArt.RoundedPanel(), PanelColor);
        panel.type = Image.Type.Sliced;
        panel.pixelsPerUnitMultiplier = 0.6f;
        RectTransform rt = panel.rectTransform;
        rt.anchorMin = corner;
        rt.anchorMax = corner;
        rt.pivot = corner;
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        return rt;
    }

    static Image NewImage(string name, Transform parent, Sprite sprite, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.sprite = sprite;
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, TMP_FontAsset font, float size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        if (font != null) { text.font = font; }
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.alignment = TextAlignmentOptions.Left;
        text.enableWordWrapping = false;
        text.overflowMode = TextOverflowModes.Overflow;
        text.raycastTarget = false;
        // Plain material (no glow) with a dark outline so it reads on anything.
        Material m = text.fontMaterial;
        m.DisableKeyword("GLOW_ON");
        text.outlineWidth = 0.18f;
        text.outlineColor = new Color32(0, 0, 0, 255);
        return text;
    }

    // Anchors to the left middle of the parent, at an offset, with a size.
    static void Place(RectTransform rt, Vector2 offset, Vector2 size)
    {
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = offset;
        rt.sizeDelta = size;
    }
}
