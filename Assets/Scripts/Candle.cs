using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Candle : MonoBehaviour
{
    private Light candleLight;
    private Tween flickeringLight;
    private float startIntensity;

    [SerializeField] private Color startColor;
    [SerializeField] private Color ghostColor;


    private void Awake()
    {
        EventsManager.onLetterReached += FlameBurst;
        EventsManager.onStartReadingTwitchResponses += () => ChangeColor(ghostColor);
        EventsManager.onEndReadingTwitchResponses += () => ChangeColor(startColor);
    }

    private void Start()
    {
        candleLight = GetComponentInChildren<Light>();
        startIntensity = candleLight.intensity;
        flickeringLight = candleLight.DOIntensity(4f, Random.Range(1.5f, 2.5f)).SetLoops(-1, LoopType.Yoyo)
            .SetDelay(Random.Range(0, 1)).SetEase(Ease.InOutBounce);
    }

    private void OnDestroy()
    {
        EventsManager.onLetterReached -= FlameBurst;
        EventsManager.onStartReadingTwitchResponses -= () => ChangeColor(ghostColor);
        EventsManager.onEndReadingTwitchResponses -= () => ChangeColor(startColor);
    }

    public void FlameBurst(string s)
    {
        flickeringLight.Pause();
        candleLight.DOIntensity(Random.Range(7, 10), Random.Range(0.2f, 0.6f)).SetLoops(2, LoopType.Yoyo)
            .SetDelay(Random.Range(0, 0.1f)).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                candleLight.DOIntensity(startIntensity, 1f).OnComplete(() =>
                {
                    flickeringLight.Restart();
                    flickeringLight.Play();
                });
            });
    }

    public void ChangeColor(Color targetColor)
    {
        candleLight.DOColor(targetColor, 0.5f);
    }
}