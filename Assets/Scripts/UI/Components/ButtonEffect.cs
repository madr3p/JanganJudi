using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Effects")]
    [SerializeField] private bool hoverScale = true;
    [SerializeField] private bool pressScale = true;
    [SerializeField] private bool hoverColor = false;
    [SerializeField] private bool pressColor = false;
    [SerializeField] private bool hoverLift = false;
    [SerializeField] private bool clickPunch = false;
    [SerializeField] private bool clickSound = false;

    [Header("Scale")]
    [SerializeField] private float hoverScaleAmount = 1.05f;
    [SerializeField] private float pressScaleAmount = 0.95f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Color")]
    [SerializeField] private Color hoverColorValue = Color.white;
    [SerializeField] private Color pressColorValue = Color.gray;
    [SerializeField] private float colorSpeed = 10f;

    [Header("Hover Lift")]
    [SerializeField] private float liftAmount = 5f;
    [SerializeField] private float liftSpeed = 10f;

    [Header("Click Punch")]
    [SerializeField] private float punchScale = 1.08f;
    [SerializeField] private float punchDuration = 0.12f;
    [SerializeField] private float punchSpeed = 15f;

    [Header("Audio")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private float audioVolume = 1f;

    private RectTransform rectTransform;
    private Image image;
    private AudioSource audioSource;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Color originalColor;

    private bool isHovering;
    private bool isPressed;
    private bool punchActive;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;

        if (image != null)
            originalColor = image.color;

        if (clickSound)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        UpdateScale();
        UpdateColor();
        UpdateLift();
    }

    private void UpdateScale()
    {
        float targetMultiplier = 1f;

        if (isHovering && hoverScale)
            targetMultiplier = hoverScaleAmount;

        if (isPressed && pressScale)
            targetMultiplier = pressScaleAmount;

        if (punchActive && clickPunch)
            targetMultiplier = punchScale;

        Vector3 targetScale = originalScale * targetMultiplier;

        float speed = punchActive ? punchSpeed : scaleSpeed;

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScale,
            Time.unscaledDeltaTime * speed
        );
    }

    private void UpdateColor()
    {
        if (image == null)
            return;

        Color targetColor = originalColor;

        if (isHovering && hoverColor)
            targetColor = hoverColorValue;

        if (isPressed && pressColor)
            targetColor = pressColorValue;

        image.color = Color.Lerp(
            image.color,
            targetColor,
            Time.unscaledDeltaTime * colorSpeed
        );
    }

    private void UpdateLift()
    {
        Vector2 targetPosition = originalPosition;

        if (isHovering && hoverLift)
            targetPosition += Vector2.up * liftAmount;

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.unscaledDeltaTime * liftSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false;
        punchActive = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;

        if (clickSound && clickClip && audioSource != null)
        {
            audioSource.PlayOneShot(clickClip, audioVolume);
        }

        if (clickPunch)
        {
            punchActive = true;

            CancelInvoke(nameof(StopPunch));
            Invoke(nameof(StopPunch), punchDuration);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private void StopPunch()
    {
        punchActive = false;
    }
}