using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class chatStart : MonoBehaviour
{
    //��ܵ��f
    public GameObject chatWindow;
    //��ܱ�
    public Text chatItem;

    // Start is called before the first frame update
    void Start()
    {
        print("start");
        // �]�m�e���V��
        Application.targetFrameRate = 60;
        // ���o��ܵ��f��m
        var vChatWindow = chatWindow.transform.localPosition;
        // �غc��ܱ�
        var itemGround = Instantiate(chatItem, vChatWindow, Quaternion.identity);

        // ��ܱ����J�ܹ�ܵ��f
        itemGround.transform.parent = chatWindow.transform;
        itemGround.text = "�A�n! �ڬOChatGPT�A���ƻ�������W����?";
    }

    // Update is called once per frame
    void Update()
    {

    }
}