using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCube : MonoBehaviour
{
    public TextMesh textMesh;
    public int posX, posY;
    public string Text
    {
        get { return textMesh.text; }
        set { textMesh.text = value; }
    }
}
