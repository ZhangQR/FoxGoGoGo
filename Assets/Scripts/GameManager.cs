using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public enum EnemyType
{
    Frog,
    Opossum,
    MaxEnemyType
};

public class GameManager : MonoBehaviour
{
    public GameObject startWall;
    public GameObject gamePassWall;
    public GameObject scorePanel;
    public int totalCherry;
    public int totalGem;
    private int cherryNum = 0;
    private int gemNum = 0;
    public int CherryNum {
        get { return cherryNum; }
        set {
            cherryNum = value;
            dialogueRunner.variableStorage.SetValue("$cherryNum", value);
        } }
    public int GemNum
    {
        get { return gemNum; }
        set
        {
            gemNum = value;
            dialogueRunner.variableStorage.SetValue("$gemNum", value);
        }
    }
    private static GameManager m_instance;

    private DialogueRunner dialogueRunner;
    static GameManager()
    { }

    public static GameManager Instance()
    {
        if (m_instance == null)
        {
            m_instance = new GameObject("GameManager").AddComponent<GameManager>();
        }
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.variableStorage.SetValue("$totalGem", new Yarn.Value(totalGem));
        dialogueRunner.variableStorage.SetValue("$totalCherry", new Yarn.Value(totalCherry));
        dialogueRunner.variableStorage.SetValue("$cherryNum", new Yarn.Value(CherryNum));
        dialogueRunner.variableStorage.SetValue("$gemNum", new Yarn.Value(GemNum));
    }

    public void SpawnEnemy(Vector3 position,EnemyType type,Transform parent,float wait_time)
    {
        StartCoroutine(LaterSpawnEnemies(position, type, parent, wait_time));
        // Instantiate(Resources.Load(ConstNames.FrogPath), position, Quaternion.identity, parent);
    }

    IEnumerator LaterSpawnEnemies(Vector3 position, EnemyType type, Transform parent, float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        switch (type)
        {
            case EnemyType.Frog:
                Instantiate(Resources.Load(ConstNames.FrogPath), position, Quaternion.identity, parent);
                break;
            case EnemyType.Opossum:
                Instantiate(Resources.Load(ConstNames.OpossumPath), position, Quaternion.identity, parent);
                break;
            default:
                Debug.LogError("please enter true EnemiesType");
                break;
        }
    }

    public void StartGame()
    {
        Debug.Log(startWall);
        startWall.SetActive(false);
        gamePassWall.SetActive(true);
        scorePanel.SetActive(true);
    }

    public void GamePass()
    {
        startWall.SetActive(true);
        gamePassWall.SetActive(false);
        scorePanel.SetActive(false);
    }



    //private void OnGUI()
    //{
    //    if (GUILayout.Button("defaultVariables"))
    //    {
    //        MDialogVar storage = (MDialogVar)dialogueRunner.variableStorage;
    //        StringBuilder sb = new StringBuilder(string.Format("defaultVariables:{0}\n", storage.defaultVariables.Length));
    //        foreach (var t in storage.defaultVariables)
    //        {
    //            sb.Append(t.name);
    //            sb.Append("  ");
    //            sb.Append(t.value);
    //            sb.Append("\n");
    //        }
    //        GUILayout.TextField(sb.ToString());
    //        Debug.Log(sb.ToString());
    //    }
    //    if (GUILayout.Button("Gethaha"))
    //    {
    //        Debug.Log(dialogueRunner.variableStorage.GetValue("$haha"));
    //    }
    //    if (GUILayout.Button("Gettotal$total_time"))
    //    {
    //        Debug.Log(dialogueRunner.variableStorage.GetValue("$total_time"));
    //    }
    //}
}
