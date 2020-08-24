using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Door : MonoBehaviour
{
    [Header("配置")]
    public Transform BirthPosition; // 人物从门中出来的位置
    public Door TargetDoor; // 出来的门
    public int CherryNum; // 需要收集的 Cherry 数量 
    public int GenNum; // 需要收集的宝石数量
    public Image ButtonImage; // 按钮提示框
    public Image RequireImage; // 需求提示框
    public TMP_Text CherryRequire; // 需求樱桃的数量文本
    public TMP_Text GemRequire; // 需求钻石的数量文本

    [Header("组件")]
    [SerializeField] private Collider2D coll;

    private bool IsDoorOpen = false;

    private void Update()
    {
        if (ButtonImage.IsActive())
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject ob = GameObject.FindGameObjectWithTag(ConstNames.PlayerTag);
                ob.transform.position = new Vector3(TargetDoor.BirthPosition.position.x,
                    TargetDoor.BirthPosition.position.y, ob.transform.position.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ConstNames.PlayerTag))
        {
            if (CherryNum > GameManager.Instance().CherryNum || GenNum > GameManager.Instance().GemNum)
            {
                CherryRequire.text = string.Format("{0}/{1}", GameManager.Instance().CherryNum, CherryNum);
                GemRequire.text = string.Format("{0}/{1}", GameManager.Instance().GemNum, GenNum);
                RequireImage.gameObject.SetActive(true);
            }
            else
            {
                IsDoorOpen = true;
                ButtonImage.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ConstNames.PlayerTag))
        {
            RequireImage.gameObject.SetActive(false);
            ButtonImage.gameObject.SetActive(false);
        }
    }
}
