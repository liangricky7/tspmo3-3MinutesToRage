using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HollowPurpleEffect : MonoBehaviour
{
    public Image hollowPurpleImage;
    public float fadeInTime  = 0.3f;
    public float holdTime    = 1f;
    public float fadeOutTime = 0.5f;

    public void Trigger()
    {
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        // fade in
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInTime);
            SetAlpha(alpha);
            yield return null;
        }

        // hold
        yield return new WaitForSeconds(holdTime);

        // fade out
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        Color c = hollowPurpleImage.color;
        c.a = alpha;
        hollowPurpleImage.color = c;
    }
}