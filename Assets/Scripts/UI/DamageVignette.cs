using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    public static DamageVignette Instance { get; private set; }

    [Header("Settings")]
    public Image vignetteImage;
    [Range(0f, 1f)] public float flashAlpha = 0.5f;
    public float fadeDuration = 0.4f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Flash()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        // Instantly set to full red
        SetAlpha(flashAlpha);

        // Fade back to transparent
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(flashAlpha, 0f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = vignetteImage.color;
        c.a = alpha;
        vignetteImage.color = c;
    }
}