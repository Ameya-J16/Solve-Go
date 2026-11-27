using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Hover and selection effect for menu buttons: the button grows and
// brightens, and its label gets "> <" markers. While idle it pulses gently.
// Added at runtime by MainMenuStyle.
public class MenuButtonFx : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    TextMeshProUGUI label;
    string original;
    Color baseColor;
    Vector3 baseScale;
    float scale = 1f;
    float phase;
    bool hot;

    void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>(true);
        baseScale = transform.localScale;
        phase = Random.value * 10f;
        if (label != null)
        {
            original = label.text;
            baseColor = label.color;
            label.enableWordWrapping = false;
        }
    }

    void Update()
    {
        float target = hot ? 1.14f : 1f + 0.03f * Mathf.Sin((Time.unscaledTime + phase) * 3f);
        scale = Mathf.Lerp(scale, target, 1f - Mathf.Exp(-Time.unscaledDeltaTime * 14f));
        transform.localScale = baseScale * scale;
    }

    void SetHot(bool value)
    {
        hot = value;
        if (label == null) { return; }
        label.text = value ? "> " + original + " <" : original;
        label.color = value ? Color.Lerp(baseColor, Color.white, 0.5f) : baseColor;
    }

    public void OnPointerEnter(PointerEventData eventData) { SetHot(true); }
    public void OnPointerExit(PointerEventData eventData) { SetHot(false); }
    public void OnSelect(BaseEventData eventData) { SetHot(true); }
    public void OnDeselect(BaseEventData eventData) { SetHot(false); }
}
