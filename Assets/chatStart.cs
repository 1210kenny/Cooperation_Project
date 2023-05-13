using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class chatStart : MonoBehaviour
{
    //對話窗口
    public GameObject chatWindow;
    //對話條
    public Text chatItem;


    // Start is called before the first frame update
    void Start()
    {
        print("start");
        // 設置畫面幀數
        Application.targetFrameRate = 60;
        // 取得對話窗口位置
        var vChatWindow = chatWindow.transform.localPosition;
        // 建構對話條
        var itemGround = Instantiate(chatItem, vChatWindow, Quaternion.identity);

        // 對話條插入至對話窗口
        itemGround.transform.parent = chatWindow.transform;
        itemGround.text = "你好! 我是ChatGPT，有甚麼我幫的上的嗎?";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
