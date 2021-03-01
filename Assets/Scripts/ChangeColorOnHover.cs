using TMPro;
using UnityEngine;


[RequireComponent(typeof(TextMeshProUGUI))]
public class ChangeColorOnHover : MonoBehaviour
{
    private Color32 startingColor;
    [SerializeField] private Color32 highlightedColor;
    private TextMeshProUGUI _text;
    void Start()
    {
        
        _text = GetComponent<TextMeshProUGUI>();
        startingColor = _text.color;
    }

    public void OnMouseEnterTrigger()
    {
        _text.color = highlightedColor;
    }

    public void OnMouseExitTrigger()
    {
        _text.color = startingColor;
    }
}
