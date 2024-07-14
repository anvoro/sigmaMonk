using System.Collections;
using UnityEngine;

public static class FadeHepler
{
    public static IEnumerator FadeIn(float duration, CanvasGroup canvasGroup, float from = 0f, float to = 1f)
    {
        if (ZeroCheck(1f, duration, canvasGroup) == false)
        {
            return Fade(from, to, duration, canvasGroup);
        }

        return null;
    }
    
    public static IEnumerator FadeOut(float duration, CanvasGroup canvasGroup, float from = 1f, float to = 0f)
    {
        if (ZeroCheck(0f, duration, canvasGroup) == false)
        {
            return Fade(from, to, duration, canvasGroup);
        }
        
        return null;
    }

    private static bool ZeroCheck(float targetAlpha, float duration, CanvasGroup canvasGroup)
    {
        if (duration <= Mathf.Epsilon)
        {
            canvasGroup.alpha = targetAlpha;
            
            return true;
        }

        return false;
    }

    private static IEnumerator Fade(float a, float b, float duration, CanvasGroup canvasGroup)
    {
        var currentTime = 0f;
        while (currentTime < duration)
        {
            var t = currentTime / duration;
            canvasGroup.alpha = Mathf.SmoothStep(a, b, t);

            currentTime += Time.deltaTime;

            yield return GameManager.WaitEndOfFrame;
        }
    }
}
