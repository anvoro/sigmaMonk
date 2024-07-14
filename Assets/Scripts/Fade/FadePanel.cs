using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadePanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn(float duration)
    {
        if (ZeroCheck(1f, duration) == false)
        {
            StartCoroutine(Fade(0f, 1f, duration));
        }
    }
    
    public void FadeOut(float duration)
    {
        if (ZeroCheck(0f, duration) == false)
        {
            StartCoroutine(Fade(1f, 0f, duration));
        }
    }

    private bool ZeroCheck(float targetAlpha, float duration)
    {
        if (duration <= Mathf.Epsilon)
        {
            _canvasGroup.alpha = targetAlpha;
            
            return true;
        }

        return false;
    }

    private IEnumerator Fade(float a, float b, float duration)
    {
        var currentTime = 0f;
        while (currentTime < duration)
        {
            var t = currentTime / duration;
            _canvasGroup.alpha = Mathf.SmoothStep(a, b, t);

            currentTime += Time.deltaTime;

            yield return GameManager.WaitEndOfFrame;
        }
    }
}
