using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    private Light ligh;
    public float triggerRange;
    public float maxIntensity;
    public float changeSpeed;
    public bool isDynamic;
    private GameObject player;

    private void Awake()
    {
        ligh = this.GetComponent<Light>();
        player = GameObject.FindGameObjectWithTag(ConstNames.PlayerTag);
        if (player == null)
        {
            Debug.LogError("do not find player");
        }
    }

    private void Start()
    {
        changeSpeed = Random.Range(3, 6);
        
    }

    void Update()
    {
        if (isDynamic)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < triggerRange)
            {
                ligh.intensity = maxIntensity - Vector3.Distance(player.transform.position, transform.position);
            }
            else
            {
                ligh.intensity = Mathf.PingPong(Time.time * changeSpeed, maxIntensity);
            }
        }
        else
        {
            ligh.intensity = Mathf.PingPong(Time.time * changeSpeed, maxIntensity);
        }
    }
}
