using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Instructions : MonoBehaviour
{

    private string spanishText = "\n Para invocar la sabiduría del Más Allá, es importante seguir los pasos del ritual:\n \n     1. <b>Formula tu pregunta en voz alta</b>, de forma clara y precisa mientras sujetas el planchette triangular con decisión.\n \n     2. No tengas miedo: <b>sigue sujetando el planchette</b> para que el portal con el Más Allá no se cierre antes de tiempo.\n \n    3. Cuando creas que los espíritus han comunicado todo lo que querían decir, <b>deja que sean sus manos las que muevan el planchette.</b>";
    private string englishText = "\n To invoke the wisdom from Beyond, it is important to follow the steps of the ritual:\n \n     1. <b>Inquire the spirits out loud</b>, talking clearly while firmly holding the heart-shaped planchette.  \n \n     2. Don't be afraid: <b>keep holding the planchette </b> preventing The Portal to close too soon. \n \n     3. Once the spirits have shown you their wisdom, <b>let them move the planchette for you.</b>";

    [SerializeField] private TextMeshProUGUI instructionsText;
    

    public void SpanishLanguage()
    {
        instructionsText.SetText(spanishText);
    }

    public void EnglishLanguage()
    {
        instructionsText.SetText(englishText);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
