using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    // 配置
    public float ladder_speed = 5.0f;

    // Reference
    public Transform DownTrigger;
    public Transform DownPosition;
    public Transform UpTrigger;
    public Transform UpPosition;
    public Collider2D coll;
    public Collider2D trigger;

    private void Update()
    {
        Collider2D down_coll = Physics2D.OverlapCircle(DownTrigger.position, 0.2f, LayerMask.GetMask(ConstNames.PlayerLayer));
        Collider2D up_coll = Physics2D.OverlapCircle(UpTrigger.position, 0.2f, LayerMask.GetMask(ConstNames.PlayerLayer));

        if (down_coll != null)
        {
            GameObject ob = down_coll.gameObject;
            if (ob.CompareTag(ConstNames.PlayerTag) && Input.GetButtonDown(ConstNames.CrouchButton))
            {
                coll.enabled = false;
                ob.transform.position = DownPosition.position;
                ob.GetComponent<PlayerController>().EnterLadder(ladder_speed);
            }
        }
        if (up_coll != null)
        {
            GameObject ob = up_coll.gameObject;
            if (ob.CompareTag(ConstNames.PlayerTag) && Input.GetButtonDown(ConstNames.JumpButton))
            {
                coll.enabled = false;
                ob.transform.position = UpPosition.position;
                ob.GetComponent<PlayerController>().EnterLadder(ladder_speed);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(ConstNames.PlayerTag))
        {
            coll.enabled = true;
            collision.gameObject.GetComponent<PlayerController>().ExitLadder();
        }
    }
}
