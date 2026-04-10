using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClick : MonoBehaviour
{
    private GameObject Plot; // ����õ��ĵؿ�

    [Header("����Ƿ���Ե��")]
    public bool isClick = true;

    // �Ƽ�����ǰ�������߲�
    private int plotLayer;

    private void Start()
    {
        // ����Plot��
        plotLayer = LayerMask.NameToLayer("Plot");

        // ע���¼�
        EventCenter.Instance.AddListener<GameObject>("����ؿ�", ClickPlot);
    }

    public void Update()
    {
        if (!isClick) return;

        // ���������
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���߼��
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                // �жϵ�����ǲ��ǵؿ��
                if (hit.collider.gameObject.layer == plotLayer)
                {
                    // �����¼�
                    EventCenter.Instance.EventTrigger<GameObject>("����ؿ�", hit.collider.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// �¼����շ���
    /// </summary>
    private void ClickPlot(GameObject plot)
    {
        Plot = plot;
        // ������������ӵ���ؿ����߼���������ʾ��Ϣ���ƶ������
    }
}