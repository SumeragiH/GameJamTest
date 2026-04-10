using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;

    private float horizontalInput;
    private float verticalInput;

    [Header("相机移动速度")]
    public float moveSpeed = 5f;//移动速度
    [Header("相机是否移动")]
    public bool isCheckInput = true;//是否检测输入

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        EventCenter.Instance.AddListener<float>("上下移动", MoveVertical);
        EventCenter.Instance.AddListener<float>("左右移动", MoveHorizontal);
    }

    private void Update()
    {
        if (!isCheckInput)//如果不检测输入，直接返回
            return;
        //检测输入，并通过事件中心分发相应的事件
        CheckInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }
    /// <summary>
    /// 检测输入，并通过事件中心分发相应的事件
    /// </summary>
    /// <param name="horizontal">水平输入值</param>
    /// <param name="vertical">垂直输入值</param>
    private void CheckInput(float horizontal, float vertical)
    {
        horizontalInput = horizontal;
        verticalInput = vertical;
        EventCenter.Instance.EventTrigger("左右移动", horizontalInput);
        EventCenter.Instance.EventTrigger("上下移动", verticalInput);
    }

    private void MoveVertical(float vertical)
    {
        mainCamera.transform.Translate(Vector3.forward * vertical * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveHorizontal(float horizontal)
    {
        mainCamera.transform.Translate(Vector3.right * horizontal * moveSpeed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 禁止移动
    /// </summary>
    /// <param name="b"></param>
    public void SetCheckInput(bool b)
    {
        isCheckInput = b;
    }
}
