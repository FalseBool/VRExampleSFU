using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSaber : MonoBehaviour
{
    [ContextMenuItem("ToggleLightSaber", "ToggleLightSaber")]//debug

    [SerializeField]
    private GameObject lightRootRef;

    [SerializeField]
    private GameObject lightEndRef;

    public event System.Action<bool> LightSaberStateChanged;

    private bool switchedOn = false;

    public float switchedDuration = 0.1f;

    public float length = 1f;

    private float time = 0f;

    private Coroutine coroutine;

    public void ToggleLightSaber() 
    {
        switchedOn = !switchedOn;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(FadeRoutine(switchedOn));
    }

    private void OnEnable()
    {
        LightSaberStateChanged += OnLightSaberStateChanged;
    }

    private void OnDisable()
    {
        LightSaberStateChanged -= OnLightSaberStateChanged;
    }

    private void OnLightSaberStateChanged(bool enabledState)
    {
        if (!enabledState)
        {
            lightRootRef.SetActive(false);
        }
    }

    private IEnumerator FadeRoutine(bool enable)
    {
        if (enable)
        {
            lightRootRef.SetActive(true);
        }

        while (true)
        {
            if (enable)
            {
                time += Time.deltaTime * 1 / switchedDuration;
            }
            else
            {
                time -= Time.deltaTime * 1 / switchedDuration;
            }

            lightRootRef.transform.localScale = new Vector3(lightRootRef.transform.localScale.x,
                Mathf.Lerp(0, length, time),
                lightRootRef.transform.localScale.z);

            if (time >= 1 || time < 0)
            {
                time = Mathf.Clamp01(time);
                lightRootRef.transform.localScale = new Vector3(lightRootRef.transform.localScale.x,
                Mathf.Lerp(0, length, time),
                lightRootRef.transform.localScale.z);
                LightSaberStateChanged?.Invoke(switchedOn);
                break;
            }

            yield return null;
        }
    }
}
