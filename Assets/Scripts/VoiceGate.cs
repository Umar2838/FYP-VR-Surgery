using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VoiceGate : MonoBehaviour
{
    public AudioClip clip;
    public Button button;

    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            return;
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.loop = false;
    }

    void OnEnable()
    {
        if (button != null)
            button.interactable = false;
        if (source == null || clip == null)
        {
            Unlock();
            return;
        }

        if (clip.loadState != AudioDataLoadState.Loaded)
            clip.LoadAudioData();
        source.Stop();
        source.clip = clip;
        source.Play();
        StopAllCoroutines();
        StartCoroutine(WaitForVoice());
    }

    void OnDisable()
    {
        StopAllCoroutines();
        if (source != null && source.isPlaying)
            source.Stop();
    }

    IEnumerator WaitForVoice()
    {
        yield return null;
        float deadline = Time.unscaledTime + (clip != null ? Mathf.Max(clip.length, 0.1f) : 0.1f);
        while (Time.unscaledTime < deadline)
            yield return null;
        while (source != null && source.isPlaying)
            yield return null;
        Unlock();
    }

    void Unlock()
    {
        if (button != null)
            button.interactable = true;
    }
}
