using Microsoft.CognitiveServices.Speech.Transcription;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;

public class settingButton : MonoBehaviour
{

    public GameObject mainUiCanvas;
    public GameObject windowCanvas;
    private bool visible = false;

    //chatGPT對話 物件 (讀取對話紀錄使用)
    public ChatGPT chatGPT;
    private GameObject windowView;
    public GameObject GptChatItem;
    public GameObject UserChatItem;

    public void click()
    {
        if(visible)
        {
            Destroy(windowView);
            visible = false;
            /*
            GameObject conversationView = windowView.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            for (int i = conversationView.transform.childCount-1; i >= 0 ; i--) {
                Destroy(conversationView.transform.GetChild(i).gameObject);
            }
            */
        }
        else
        {
            // 建構對話條
            var testitemGround = Instantiate(windowCanvas, mainUiCanvas.transform);
            // 對話條插入至對話窗口
            testitemGround.transform.SetParent(mainUiCanvas.transform);
            windowView = testitemGround;

            visible = true;
            GameObject conversationView = windowView.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
            for (int i = 0; i< chatGPT.m_DataList.Count; i++)
            {
                if (chatGPT.m_DataList[i].role == "user")
                {
                    string text = chatGPT.m_DataList[i].content;

                    var vChatWindow = conversationView.transform.localPosition;
                    var itemGround = Instantiate(UserChatItem, vChatWindow, Quaternion.identity);
                    itemGround.transform.SetParent(conversationView.transform);
                    itemGround.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = text;
                }
                else if(chatGPT.m_DataList[i].role == "assistant")
                {
                    string text = chatGPT.m_DataList[i].content;

                    var vChatWindow = conversationView.transform.localPosition;
                    var itemGround = Instantiate(GptChatItem, vChatWindow, Quaternion.identity);
                    itemGround.transform.SetParent(conversationView.transform);
                    itemGround.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = text;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
