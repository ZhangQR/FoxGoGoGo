using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Yarn;

public class EnterKey : MonoBehaviour
{
    [Header("配置")]
    public KeyCode continueKey = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown(continueKey))
        {
            FindObjectOfType<MDialogUI>().RequireNextLine();
        }
    }
}
