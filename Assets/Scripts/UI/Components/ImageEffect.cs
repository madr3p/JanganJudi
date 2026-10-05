using UnityEngine;

public class ImageEffect : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private bool pulse = true;
    [SerializeField] private bool floatEffect = true;
    [SerializeField] private bool sway = false;
    [SerializeField] private bool fadePulse = false;

    [Header("Pulse")]
    [SerializeField] private float pulseAmount = 0.03f;
    [SerializeField] private float pulseSpeed = 2f;

    [Header("Float")]
    [SerializeField] private float floatAmount = 5f;
    [SerializeField] private float floatSpeed = 1.5f;

    [Header("Sway")]
    [SerializeField] private float swayAmount = 2f;
    [SerializeField] private float swaySpeed = 1f;

    [Header("Fade Pulse")]
    [SerializeField] private float fadeAmount = 0.05f;
    [SerializeField] private float fadeSpeed = 2f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    private float pulseOffset;
    private float floatOffset;
    private float swayOffset;
    private float fadeOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;

        // Random starting points so multiple images don't animate identically.
        pulseOffset = Random.Range(0f, Mathf.PI * 2f);
        floatOffset = Random.Range(0f, Mathf.PI * 2f);
        swayOffset = Random.Range(0f, Mathf.PI * 2f);
        fadeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float time = Time.unscaledTime;

        UpdatePulse(time);
        UpdateFloat(time);
        UpdateSway(time);
        UpdateFade(time);
    }

    private void UpdatePulse(float time)
    {
        if (!pulse)
            return;

        float wave = Mathf.Sin(
            time * pulseSpeed + pulseOffset
        );

        float scale = 1f + (wave * pulseAmount);

        rectTransform.localScale = originalScale * scale;
    }

    private void UpdateFloat(float time)
    {
        if (!floatEffect)
            return;

        float wave = Mathf.Sin(
            time * floatSpeed + floatOffset
        );

        float offset = wave * floatAmount;

        rectTransform.anchoredPosition =
            originalPosition + Vector2.up * offset;
    }

    private void UpdateSway(float time)
    {
        if (!sway)
            return;

        float wave = Mathf.Sin(
            time * swaySpeed + swayOffset
        );

        float rotation = wave * swayAmount;

        rectTransform.localRotation =
            originalRotation * Quaternion.Euler(0f, 0f, rotation);
    }

    private void UpdateFade(float time)
    {
        if (!fadePulse || canvasGroup == null)
            return;

        float wave = Mathf.Sin(
            time * fadeSpeed + fadeOffset
        );

        float alpha = 1f - ((wave + 1f) * 0.5f * fadeAmount);

        canvasGroup.alpha = alpha;
    }
}