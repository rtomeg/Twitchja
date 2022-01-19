using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


public class SpookyWords : MonoBehaviour
{
    [SerializeField] private GameObject spookyWord;
    [SerializeField] private float spawnDuration = 1;
    [SerializeField] private int margin = 100;
    private List<GameObject> activeSpookyWords = new List<GameObject>();
    [SerializeField] private int maxSpookyWords = 300;

    private void Awake()
    {
        EventsManager.onCommandReceived += ShowSpookyWord;
    }

    private void OnDestroy()
    {
        EventsManager.onCommandReceived -= ShowSpookyWord;
    }

    private void ShowSpookyWord(string user,string word)
    {
        if (activeSpookyWords.Count >= maxSpookyWords) return;

        GameObject wordSpawned = Instantiate(spookyWord,
            new Vector3(Random.Range(margin, Screen.width - margin), Random.Range(margin, Screen.height - margin)),
            Quaternion.Euler(new Vector3(0, 0, Random.Range(-20, 20))),
            transform);

        activeSpookyWords.Add(wordSpawned);

        if (wordSpawned.TryGetComponent(out TextMeshProUGUI _tmp))
        {
            _tmp.SetText(word);
            _tmp.DOFade(0.5f, spawnDuration).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                activeSpookyWords.Remove(wordSpawned);
                Destroy(wordSpawned);
            });
            wordSpawned.transform.DOShakeRotation(spawnDuration * 2, 10f);
        }
    }
}