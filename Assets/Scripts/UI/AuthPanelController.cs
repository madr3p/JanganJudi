using UnityEngine;
using System.Collections;

public class AuthPanelController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup loginPanel;
    [SerializeField] private CanvasGroup registerPanel;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.2f;

    private bool isSwitching;

    private void Start()
    {
        loginPanel.gameObject.SetActive(true);
        registerPanel.gameObject.SetActive(false);

        loginPanel.alpha = 1f;
        registerPanel.alpha = 0f;
    }

    public void ShowLogin()
    {
        if (!isSwitching)
            StartCoroutine(SwitchPanel(registerPanel, loginPanel));
    }

    public void ShowRegister()
    {
        if (!isSwitching)
            StartCoroutine(SwitchPanel(loginPanel, registerPanel));
    }

    private IEnumerator SwitchPanel(
        CanvasGroup currentPanel,
        CanvasGroup nextPanel)
    {
        isSwitching = true;

        currentPanel.interactable = false;
        currentPanel.blocksRaycasts = false;

        yield return Fade(currentPanel, 1f, 0f);

        currentPanel.gameObject.SetActive(false);

        nextPanel.gameObject.SetActive(true);
        nextPanel.alpha = 0f;
        nextPanel.interactable = false;
        nextPanel.blocksRaycasts = false;

        yield return Fade(nextPanel, 0f, 1f);

        nextPanel.interactable = true;
        nextPanel.blocksRaycasts = true;

        isSwitching = false;
    }

    private IEnumerator Fade(CanvasGroup group, float start, float end)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / fadeDuration);

            group.alpha = Mathf.Lerp(start, end, progress);

            yield return null;
        }

        group.alpha = end;
    }
}