using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class CarvedWord : MonoBehaviour
{
    private TextMeshPro _textMeshPro;
    [SerializeField] private float apparitionTime;

    private void Awake()
    {
        EventsManager.onOuijaResponseEnded += ShowWord;
        _textMeshPro = GetComponent<TextMeshPro>();
    }
    private void OnDestroy()
    {
        EventsManager.onOuijaResponseEnded -= ShowWord;
    }

    private void ShowWord(string word)
    {
        _textMeshPro.SetText(word);
        _textMeshPro.DOFade(1, apparitionTime).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutBounce);
    }
}