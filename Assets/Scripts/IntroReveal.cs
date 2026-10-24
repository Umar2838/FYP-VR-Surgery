using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroReveal : MonoBehaviour
{
    public RectTransform[] steps;
    public float delay = 0.15f;
    public float gap = 0.16f;
    public float duration = 0.42f;
    public float rise = 36f;

    struct Step
    {
        public RectTransform rect;
        public Graphic[] graphics;
        public Color[] colors;
        public Vector2 origin;
    }

    Step[] cache;

    void OnEnable()
    {
        Cache();
        StopAllCoroutines();
        StartCoroutine(Play());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        Restore();
    }

    void Cache()
    {
        if (steps == null)
            return;
        cache = new Step[steps.Length];
        for (int i = 0; i < steps.Length; i++)
        {
            Step step = new Step();
            step.rect = steps[i];
            if (step.rect != null)
            {
                step.origin = step.rect.anchoredPosition;
                step.graphics = step.rect.GetComponentsInChildren<Graphic>(true);
                step.colors = new Color[step.graphics.Length];
                for (int g = 0; g < step.graphics.Length; g++)
                    step.colors[g] = step.graphics[g].color;
                Set(step, 0f);
            }
            cache[i] = step;
        }
    }

    void Set(Step step, float t)
    {
        if (step.rect == null)
            return;
        step.rect.anchoredPosition = step.origin + Vector2.down * rise * (1f - t);
        for (int g = 0; g < step.graphics.Length; g++)
        {
            Color color = step.colors[g];
            color.a *= t;
            step.graphics[g].color = color;
        }
    }

    IEnumerator Play()
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        for (int i = 0; i < cache.Length; i++)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / Mathf.Max(0.01f, duration));
                k = k * k * (3f - 2f * k);
                Set(cache[i], k);
                yield return null;
            }
            Set(cache[i], 1f);
            if (gap > 0f && i < cache.Length - 1)
                yield return new WaitForSeconds(gap);
        }
    }

    void Restore()
    {
        if (cache == null)
            return;
        for (int i = 0; i < cache.Length; i++)
            Set(cache[i], 1f);
    }
}
