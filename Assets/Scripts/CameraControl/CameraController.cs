using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;

    private float horizontalInput;
    private float verticalInput;
    private float ScrolledInput;

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
        EventCenter.Instance.AddListener<float>("放大",Enlarge);
        EventCenter.Instance.AddListener<float>("缩小",Shrink);
    }

    private void Update()
    {
        if (!isCheckInput)//如果不检测输入，直接返回
            return;
        //检测输入，并通过事件中心分发相应的事件
        CheckInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        CheckRoller(Input.GetAxis("Mouse ScrollWheel"));

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

    /// 检测滚轮输入，暂时先不经过事件中心分发，直接在这里处理
    private void CheckRoller(float scroll)
    {
        ScrolledInput = scroll;
        if (ScrolledInput>0)
        {
            EventCenter.Instance.EventTrigger("放大", ScrolledInput);
        }
        if (ScrolledInput < 0)
        {
            EventCenter.Instance.EventTrigger("缩小", ScrolledInput);
        }
    }

    private void MoveVertical(float vertical)
    {
        mainCamera.transform.Translate(Vector3.up * vertical * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveHorizontal(float horizontal)
    {
        mainCamera.transform.Translate(Vector3.right * horizontal * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Enlarge(float scroll)
    {
        //镜头向前移动
        if (0 <= mainCamera.transform.position.z && mainCamera.transform.position.z <= 4)
        {
            mainCamera.transform.Translate(Vector3.forward * scroll * moveSpeed * 15 * Time.deltaTime, Space.Self);
        }
        if(mainCamera.transform.position.z>4)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 4);
        }
    }

    private void Shrink(float scroll)
    {
        //镜头向后移动,注意这里的scroll是负数，所以相当于向后移动
        if (0<=mainCamera.transform.position.z&&mainCamera.transform.position.z<=4 )
        {
            mainCamera.transform.Translate(Vector3.forward * scroll * moveSpeed * 15 * Time.deltaTime, Space.Self);
        }
        if (mainCamera.transform.position.z < 0)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
        }
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
