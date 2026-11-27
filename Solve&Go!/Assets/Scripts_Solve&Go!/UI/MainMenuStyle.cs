using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Dresses up the main menu when it opens: pink neon title that flickers like a
// real neon sign, a tagline, the controls, stats from your last run, and hover
// effects on the buttons. MainMenu adds this automatically, so the scene needs
// no changes. Tweak the fields in the Inspector while playing to try colours.
public class MainMenuStyle : MonoBehaviour
{
    [SerializeField] string tagline = "A DELIBERATELY UNFAIR PLATFORMER";
    [SerializeField] string controls = "MOVE   A D / ARROWS        JUMP   SPACE        SHOOT   CLICK";
    [SerializeField] Color titleColor = new Color(1f, 0.35f, 0.88f);
    [SerializeField] Color titleGlow = new Color(1f, 0.1f, 0.75f, 0.85f);
    [SerializeField] Color taglineColor = new Color(0.35f, 0.88f, 1f, 0.95f);
    [SerializeField] Color statsColor = new Color(1f, 0.45f, 0.9f, 0.95f);
    [SerializeField] Color controlsColor = new Color(0.75f, 0.9f, 1f, 0.6f);

    TextMeshProUGUI title;
    Image background;

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) { return; }
        RectTransform root = (RectTransform)canvas.transform;

        foreach (TextMeshProUGUI t in canvas.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (t.text.ToUpperInvariant().Contains("GO")) { title = t; }
        }
        foreach (Image img in canvas.GetComponentsInChildren<Image>(true))
        {
            if (img.name == "Background") { background = img; }
        }

        TMP_FontAsset font = title != null ? title.font : TMP_Settings.defaultFontAsset;

        if (title != null)
        {
            title.color = titleColor;
            Material m = title.fontMaterial;   // this text's own copy
            m.EnableKeyword("GLOW_ON");
            m.SetColor("_GlowColor", titleGlow);
            m.SetFloat("_GlowOuter", 0.6f);
            m.SetFloat("_GlowPower", 0.9f);
            title.UpdateMeshPadding();
        }

        AddText(root, "Tagline", tagline, font, 30f, taglineColor, new Vector2(0f, 352f), 10f);
        AddText(root, "Controls", controls, font, 22f, controlsColor, new Vector2(0f, -428f), 4f);

        if (PlayerPrefs.HasKey(gameSession.LastRunDeathsKey))
        {
            int deaths = PlayerPrefs.GetInt(gameSession.LastRunDeathsKey);
            int points = PlayerPrefs.GetInt(gameSession.LastRunScoreKey);
            string stats = "LAST RUN:   " + deaths + (deaths == 1 ? " DEATH" : " DEATHS") + "   |   " + points + " POINTS";
            AddText(root, "Last Run", stats, font, 28f, statsColor, new Vector2(0f, -360f), 6f);
        }

        foreach (Button b in canvas.GetComponentsInChildren<Button>(true))
        {
            if (b.GetComponent<MenuButtonFx>() == null) { b.gameObject.AddComponent<MenuButtonFx>(); }
        }

        StartCoroutine(NeonFlicker());
    }

    // Every few seconds the neon stutters off and on, like a cheap sign.
    IEnumerator NeonFlicker()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(2.5f, 6f));
            int blinks = Random.Range(1, 4);
            for (int i = 0; i < blinks; i++)
            {
                SetNeon(Random.Range(0.15f, 0.4f));
                yield return new WaitForSecondsRealtime(Random.Range(0.03f, 0.09f));
                SetNeon(1f);
                yield return new WaitForSecondsRealtime(Random.Range(0.04f, 0.14f));
            }
        }
    }

    void SetNeon(float brightness)
    {
        if (title != null) { title.alpha = brightness; }
        if (background != null)
        {
            float b = Mathf.Lerp(0.7f, 1f, brightness);
            background.color = new Color(b, b, b, 1f);
        }
    }

    static void AddText(RectTransform parent, string name, string value, TMP_FontAsset font, float size,
        Color color, Vector2 position, float spacing)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI text = go.AddComponent<TextMeshProUGUI>();
        if (font != null) { text.font = font; }
        text.fontMaterial.DisableKeyword("GLOW_ON");
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyles.Bold;
        text.color = color;
        text.characterSpacing = spacing;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(1600f, 60f);
    }
}
