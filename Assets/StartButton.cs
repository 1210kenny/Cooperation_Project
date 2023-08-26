using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    // 這個方法會在按鈕被點擊時被呼叫
    public void OnButtonClicked()
    {
        // 使用 SceneManager 切換場景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}

