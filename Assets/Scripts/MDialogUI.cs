/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using Yarn.Unity;
using Yarn;
using System;
using TMPro;

[Serializable]
public class MyStringEvent : UnityEngine.Events.UnityEvent<string>
{

}

public class MDialogUI : DialogueUIBehaviour
{
    [Header("配置")]
    [Tooltip("文字播放的速度")]
    public float TextSpeed = 0.2f;
    
    [Tooltip("用来显示的按钮")]
    public List<Button> optionButtons;
    
    [Tooltip("用来显示文本的地方")]
    public Text text;
    public override Dialogue.HandlerExecutionType RunCommand(Command command, Action onCommandComplete)
    {
        Debug.Log("RunCommand");
        return Dialogue.HandlerExecutionType.ContinueExecution;
    }

    [HideInInspector]
    public MyStringEvent changeHeadSprite;
    [HideInInspector]
    public UnityEngine.Events.UnityEvent OnComplete;

    private bool WaittingForOption = false;
    private bool usrRequeirNextLine = false;

    public override Dialogue.HandlerExecutionType RunLine(Line line, ILineLocalisationProvider localisationProvider, Action onLineComplete)
    {
        StartCoroutine(DoRunLine(line, localisationProvider, onLineComplete));
        return Dialogue.HandlerExecutionType.PauseExecution;
    }

    private IEnumerator DoRunLine(Line line, ILineLocalisationProvider localisationProvider, Action onLineComplete)
    {
        usrRequeirNextLine = false;
        StringBuilder stringBuild = new StringBuilder();
        string line_text = localisationProvider.GetLocalisedTextForLine(line);

        // 改变头像
        if (line_text.StartsWith("小狐狸"))
        {
            changeHeadSprite?.Invoke(ConstNames.PlayerHead);
        }else if (line_text.StartsWith("告示牌")){
            changeHeadSprite?.Invoke(ConstNames.NpcHead);
        }
        else
        {
            Debug.LogError(line_text + " is not start with 小狐狸 or 告示牌");
        }

        foreach(var c in line_text)
        {
            if(usrRequeirNextLine)
            {
                text.text = line_text;
                break;
            }
            stringBuild.Append(c);
            text.text = stringBuild.ToString();
            yield return new WaitForSeconds(TextSpeed);
        }
        usrRequeirNextLine = false;
        while (usrRequeirNextLine == false)
        {
            yield return null;
        }
        onLineComplete();
    }

    public void RequireNextLine()
    {
        usrRequeirNextLine = true;
    }

    // 将所有的按钮原本的事件给清除，然后加上自己的事件
    public override void RunOptions(OptionSet optionsCollection, ILineLocalisationProvider localisationProvider, Action<int> selectOption)
    {
        StartCoroutine(DoRunOptions(optionsCollection, localisationProvider, selectOption));
    }

    private IEnumerator DoRunOptions(OptionSet optionsCollection, ILineLocalisationProvider localisationProvider, Action<int> selectOption)
    {
        WaittingForOption = true;

        if (optionButtons.Count < optionsCollection.Options.Length)
        {
            Debug.LogError("按钮的数量小于需求的数量");
        }

        int i = 0;
        foreach (var option in optionsCollection.Options)
        {
            optionButtons[i].gameObject.SetActive(true);
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() =>
            {
                WaittingForOption = false;
                selectOption(option.ID);
            });

            // 查找 Button 的子对象，来改变按钮上显示的字
            Text t = optionButtons[i].GetComponentInChildren<Text>();
            if (t != null)
            {
                t.text = localisationProvider.GetLocalisedTextForLine(option.Line);
            }
            TMP_Text tt = optionButtons[i].GetComponentInChildren<TMP_Text>();
            if (tt != null)
            {
                tt.text = localisationProvider.GetLocalisedTextForLine(option.Line);
            }
            i++;
        }

        // 一直在等待按钮选择
        while (WaittingForOption)
        {
            yield return null;
        }

        foreach(Button btn in optionButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public bool IsWaitForOption()
    {
        return WaittingForOption;
    }

    public override void DialogueComplete()
    {
        OnComplete?.Invoke();
    }
}

 
