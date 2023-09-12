using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menulist : MonoBehaviour
{
    public GameObject menu;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Return()
    {
        menu.SetActive(false);
        Time.timeScale = (1);
    }
    public void Restart()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = (1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void change()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = (1);
    }
}
