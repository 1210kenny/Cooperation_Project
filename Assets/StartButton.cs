using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{

    // �o�Ӥ�k�|�b���s�Q�I���ɳQ�I�s
    public void OnButtonClicked()
    {
        // �ϥ� SceneManager ��������
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}

