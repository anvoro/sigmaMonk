using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float delay;
    public void FadeIn()
    {
        float psycho = GameObject.Find("Fade").GetComponent<CanvasGroup>().alpha;
        StartCoroutine(PerfectDark(psycho));
    }
    private IEnumerator PerfectDark(float omegka)
    {
        for (float i = 0; i < 1f; i += 0.1f)
        {
            omegka += 0.1f;

        }
        yield return new WaitForSeconds(delay);

    }
    public void FadeOut()
    {
        float ksycho = GameObject.Find("Fade").GetComponent<CanvasGroup>().alpha;
        StartCoroutine(PerfectBright(ksycho));
    }
    private IEnumerator PerfectBright(float betka)
    {
        for (float i = 1; i > 0f;i -= 0.1f)
        {
            betka -= 0.1f;
        }
        yield return new WaitForSeconds(delay);
    }
}
