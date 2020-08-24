using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadPortrait : MonoBehaviour
{
    [System.Serializable]
    public struct HeadStruct
    {
        public string name;
        public Sprite image;
    };

    public List<HeadStruct> headList;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void setHead(string name)
    {
        foreach(var temp in headList)
        {
            if(temp.name == name)
            {
                image.sprite = temp.image;
                break;
            }
        }
    }
}
