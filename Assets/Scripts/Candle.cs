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

    [SerializeField] private float minBurstIntensity = 0.03f;
    [SerializeField] private float maxBurstIntensity = 0.06f;

    [SerializeField] private float minFlickerIntensity = 0.05f;
    [SerializeField] private float maxFlickerIntensity = 0.2f;

    [SerializeField] private float WiggleIntensity = 2;


    //TODO: Refactor lambda
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
        flickeringLight = candleLight.DOIntensity(Random.Range(minFlickerIntensity, maxFlickerIntensity), Random.Range(1.5f, 2.5f)).SetLoops(-1, LoopType.Yoyo)
            .SetDelay(Random.Range(0, 1)).SetEase(Ease.InOutBounce);
        NewShake();
    }

    private void OnDestroy()
    {
        EventsManager.onLetterReached -= FlameBurst;
        EventsManager.onStartReadingTwitchResponses -= () => ChangeColor(ghostColor);
        EventsManager.onEndReadingTwitchResponses -= () => ChangeColor(startColor);
    }

    private void NewShake()
    {
        candleLight.transform.DOShakePosition(Random.Range(0.5f, 2f), 0.001f, 2).SetEase(Ease.Linear).SetDelay(Random.Range(0f,2f)).OnComplete(NewShake);
    }

    public void FlameBurst(string s)
    {
        flickeringLight.Pause();
        candleLight.DOIntensity(Random.Range(minBurstIntensity, maxBurstIntensity), Random.Range(0.2f, 0.6f)).SetLoops(2, LoopType.Yoyo)
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