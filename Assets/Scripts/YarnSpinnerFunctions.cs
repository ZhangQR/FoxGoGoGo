using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;

public class YarnSpinnerFunctions : MonoBehaviour
{
    // 用于存放已经访问过的 yarnSpinner 节点
    private HashSet<string> _visitedNode;
    private DialogueRunner dialogueRunner;

    private void Awake()
    {
        _visitedNode = new HashSet<string>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    private void Start()
    {
        dialogueRunner.AddFunction("visited", 1, (Value[] param) =>
          {
              return _visitedNode.Contains(param[0].AsString);
          });

        dialogueRunner.onNodeComplete.AddListener(OnNodeComplete);
        // dialogueRunner.AddFunction("startGame", 0, StartGame);
        dialogueRunner.AddFunction("rejectGame", 0, RejectGame);
        dialogueRunner.AddFunction("finishGame", 0, FinishGame);
    }

    // 将此方法添加到 dialogueRunner 的 onNodeComplete 
    public void OnNodeComplete(string name)
    {
        _visitedNode.Add(name);
    }

    [YarnCommand("startGame")]
    public void StartGame()
    {
        GameManager.Instance().StartGame();
    }

    private void RejectGame(Value[] param)
    {
        Application.Quit();
    }


    private void FinishGame(Value[] param)
    {
        GameManager.Instance().GamePass();
    }
}
