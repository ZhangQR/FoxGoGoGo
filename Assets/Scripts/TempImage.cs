using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempImage : MonoBehaviour
{
    [Header("配置")]
    public float Speed = 0.1f;

    [Header("赋值")]
    public TMP_Text tmp_text;

    private string totalText;
    private IEnumerator loopWord;
    private void Awake()
    {
        totalText = tmp_text.text;
        loopWord = ShowText(Speed);
    }
    private void OnEnable()
    {
        StartCoroutine(loopWord);
    }
    private IEnumerator ShowText(float interval_time)
    {
        int i = 0;
        while (true)
        {
            tmp_text.text = totalText.Substring(0, i);
            i++;
            if(i == totalText.Length+1)
            {
                i = 0;
            }
            yield return new WaitForSeconds(interval_time);
        }
    }



}
