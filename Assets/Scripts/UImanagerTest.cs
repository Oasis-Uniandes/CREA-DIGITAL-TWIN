using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UImanagerTest : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMP;
    private int textId = 0;
    private int dialogueSize = 5;

    public void buttonPressed()
    {
        textId++;
        updateText();
    }

    private void updateText()
    {
        textMP.text = "Este es el texto #" + textId;
        if (textId >= dialogueSize)
        {
            textMP.text = "Aquí podria ser el fin del texto, algo podria ocurrir aca... la ventana puede desaparecer, la escena puede cambiar, etc...";
        }
    }
}
