using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn;
using TMPro;

public class Sign : MonoBehaviour
{
    [Header("赋值")]
    public GameObject TempImage;
    public Image EnterImage;
    public Image Dialog;
    public TMP_Text pressKey;
    public HeadPortrait headChange;


    private Collider2D trigger;
    private DialogueRunner runner;
    private MDialogUI dialogUI;
    private void Awake()
    {
        trigger = GetComponent<Collider2D>();
        runner = FindObjectOfType<DialogueRunner>();
        dialogUI = FindObjectOfType<MDialogUI>();
    }
    private void Start()
    {
        if (dialogUI.changeHeadSprite == null)
        {
            dialogUI.changeHeadSprite = new MyStringEvent();
        }
        dialogUI.changeHeadSprite.AddListener((string name) => {
            headChange.setHead(name);
        });
        
        //if(dialogUI.OnComplete == null)
        //{
        //    dialogUI.OnComplete = new UnityEngine.Events.UnityEvent();
        //}
        
        dialogUI.OnComplete.AddListener(() =>
        {
            Dialog.gameObject.SetActive(false);
        });

        //runner.AddFunction("changeHead", 1, (Value[] parameters) =>
        //{
        //    headChange.setHead(parameters[0].AsString);
        //});
    }

    private void Update()
    {
        if (EnterImage.IsActive() && Input.GetKeyDown(KeyCode.R))
        {
            Dialog.gameObject.SetActive(true);
            EnterImage.gameObject.SetActive(false);
            runner.StartDialogue(ConstNames.StartNode);
        }

        if (pressKey.IsActive() && dialogUI.IsWaitForOption()){
            pressKey.gameObject.SetActive(false);
        }else if(!pressKey.IsActive() && !dialogUI.IsWaitForOption())
        {
            pressKey.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ConstNames.PlayerTag))
        {
            EnterImage.gameObject.SetActive(true);
            TempImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ConstNames.PlayerTag))
        {
            EnterImage.gameObject.SetActive(false);
            TempImage.gameObject.SetActive(true);
            Dialog.gameObject.SetActive(false);
        }
    }
}
