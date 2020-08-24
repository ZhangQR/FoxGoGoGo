using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text cherry_text;
    public TMP_Text gem_text;

    private void Update()
    {
        cherry_text.text = "x" + GameManager.Instance().CherryNum;
        gem_text.text = "x" + GameManager.Instance().GemNum;
    }

}
