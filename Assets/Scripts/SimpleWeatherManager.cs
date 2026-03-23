using UnityEngine;
using System.Collections;

public enum WeatherState { Clear, Rain, Wind }

public class SimpleWeatherManager : MonoBehaviour
{
    public ParticleSystem rainParticles;
    public WindZone windZone;
    public float timeBetweenChanges = 30f;
    public float transitionDuration = 5f;

    [Header("Fog Density")]
    public float clearFogDensity = 0f;
    public float rainFogDensity = 0.015f;
    public float windFogDensity = 0f;

    [Header("Wind Intensity")]
    public float clearWindIntensity = 0.1f;
    public float rainWindIntensity = 0.5f;
    public float windWindIntensity = 1.0f;

    [Header("Rain Settings")]
    public float rainEmissionRate = 2000f;

    private WeatherState currentState;
    private Coroutine transitionCoroutine;

    private void Start()
    {
        RenderSettings.fog = true;
        
        if (rainParticles != null)
        {
            var emission = rainParticles.emission;
            emission.rateOverTime = rainEmissionRate;
        }

        StartCoroutine(WeatherCycle());
    }

    private IEnumerator WeatherCycle()
    {
        while (true)
        {
            WeatherState newState = (WeatherState)Random.Range(0, 3);
            ChangeWeather(newState);
            yield return new WaitForSeconds(timeBetweenChanges);
        }
    }

    private void ChangeWeather(WeatherState newState)
    {
        currentState = newState;

        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);

        float targetFog = 0;
        float targetWind = 0;

        switch (newState)
        {
            case WeatherState.Clear:
                targetFog = clearFogDensity;
                targetWind = clearWindIntensity;
                if (rainParticles != null) rainParticles.Stop();
                break;
            case WeatherState.Rain:
                targetFog = rainFogDensity;
                targetWind = rainWindIntensity;
                if (rainParticles != null) rainParticles.Play();
                break;
            case WeatherState.Wind:
                targetFog = windFogDensity;
                targetWind = windWindIntensity;
                if (rainParticles != null) rainParticles.Stop();
                break;
        }

        transitionCoroutine = StartCoroutine(TransitionWeather(targetFog, targetWind));
    }

    private IEnumerator TransitionWeather(float targetFog, float targetWind)
    {
        float startFog = RenderSettings.fogDensity;
        float startWind = windZone != null ? windZone.windMain : 0;
        float elapsed = 0;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            RenderSettings.fogDensity = Mathf.Lerp(startFog, targetFog, t);
            if (windZone != null) windZone.windMain = Mathf.Lerp(startWind, targetWind, t);

            yield return null;
        }

        RenderSettings.fogDensity = targetFog;
        if (windZone != null) windZone.windMain = targetWind;
    }
}
