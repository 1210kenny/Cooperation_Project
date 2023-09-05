using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class charcaterselection : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject[] characterlist;
    private int index;
    private void Start()
    {
        index=PlayerPrefs.GetInt("CharacterSelected");


        characterlist=new GameObject[transform.childCount];

        for(int i=0;i<transform.childCount;i++)
        {
            characterlist[i]=transform.GetChild(i).gameObject;
        }

        foreach(GameObject go in characterlist)
         go.SetActive(false);
         
        //first index
        if(characterlist[index])
          characterlist[index].SetActive(true);
        


    }

    public void ToggleLeft()
    {

        //off model
        characterlist[index].SetActive(false);

        index--;
        if(index<0)
         index=characterlist.Length-1;

        //on model

        characterlist[index].SetActive(true);

    }

    public void ToggleRight()
    {

        //off model
        characterlist[index].SetActive(false);

        index++;
        if(index==characterlist.Length)
         index=0;

        //on model

        characterlist[index].SetActive(true);

    }

    public void Comfirmbutton()
    {
        PlayerPrefs.SetInt("CharacterSelected",index);
        SceneManager.LoadScene("SampleScene");
    }
}
