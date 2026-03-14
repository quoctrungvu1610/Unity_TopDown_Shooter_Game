using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    //CanvasGroup canvasGroup;

    //private void Start()
    //{
    //    canvasGroup = GetComponent<CanvasGroup>();
    //}

    //public IEnumerator FadeOut(float time)
    //{
    //    while (canvasGroup.alpha < 1)
    //    {
    //        canvasGroup.alpha += Time.deltaTime / time;
    //        yield return null;
    //    }
    //}

    //public IEnumerator FadeIn(float time)
    //{
    //    while (canvasGroup.alpha > 0)
    //    {
    //        canvasGroup.alpha -= Time.deltaTime / time;
    //        yield return null;
    //    }
    //}

    CanvasGroup canvasGroup;
    Coroutine currentActiveFade = null;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeOutImmediate()
    {
        canvasGroup.alpha = 1;
    }

    public Coroutine FadeOut(float time)
    {
        return Fade(1, time);
    }

    public Coroutine FadeIn(float time)
    {
        return Fade(0, time);
    }

    public Coroutine Fade(float target, float time)
    {
        if (currentActiveFade != null)
        {
            StopCoroutine(currentActiveFade);
        }
        currentActiveFade = StartCoroutine(FadeRoutine(target, time));
        return currentActiveFade;
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.unscaledDeltaTime / time);
            yield return null;
        }
    }
}
