using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("配置")]
    public float MoveSpeed; // 移动速度
    public float MoveSpeedY; // Y 轴移动速度

    private GameObject m_camera; // 设置跟随移动的相机
    private Vector3 origin_position; // 记录原始位置
    private Vector3 origin_camera_position; // 记录相机原始的位置

    private void Awake()
    {
        m_camera = GameObject.FindGameObjectWithTag(ConstNames.MainCameraTag);
        origin_position = transform.position;
        origin_camera_position = m_camera.transform.position;
    }
    private void Update()
    {
        Vector3 distance = m_camera.transform.position - origin_camera_position;
        transform.position = origin_position + new Vector3(distance.x * MoveSpeed, distance.y * MoveSpeedY, 0);
    }





}
