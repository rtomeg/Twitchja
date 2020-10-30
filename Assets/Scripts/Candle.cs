using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Candle : MonoBehaviour
{
    private Light candleLight;

    private Tween flickeringLight;

    private float startIntensity;

    private void Start()
    {
        candleLight = GetComponentInChildren<Light>();
        startIntensity = candleLight.intensity;
        flickeringLight = candleLight.DOIntensity(4f, Random.Range(1.5f, 2.5f)).SetLoops(-1, LoopType.Yoyo).SetDelay(Random.Range(0, 1)).SetEase(Ease.InBounce);
    }

    public void FlameBurst()
    {
        flickeringLight.Pause();
        candleLight.DOIntensity(6f, 1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            candleLight.DOIntensity(startIntensity, 1f).OnComplete(() =>
            {
                flickeringLight.Restart();
                flickeringLight.Play();
            });
        });
    }

    public void ChangeColor()
    {
    }
}