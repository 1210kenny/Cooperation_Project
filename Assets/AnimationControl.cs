using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    public Animator animator;

    public void Start()
    {
        // 获取Animator组件
        animator = GetComponent<Animator>();
    }
    public void set_face(string input){
        switch (input)
        {
            case "1":
                Set_Face_Default();
                Debug.Log("set_face:1");
                break;
            case "2":
                Set_Face_Fun();
                Debug.Log("set_face:2");
                break;
            case "3":
                Set_Face_Angry();
                Debug.Log("set_face:3");
                break;
            case "4":
                Set_Face_Sorrow();
                Debug.Log("set_face:4");
                break;
            case "5":
                Set_Face_Surprised();
                Debug.Log("set_face:5");
                break;
            case "6":
                Set_Face_Talk();
                Debug.Log("set_face:6");
                break;
            default:
                Set_Face_Default();
                Debug.Log("set_face:d");
                break;
        }
    }
    public void set_action(string input){
        switch (input)
        {
            case "1":
                Set_Body_Standing();
                Debug.Log("set_action:1");
                break;
            case "2":
                Set_Body_Bored();
                Debug.Log("set_action:2");
                break;
            case "3":
                Set_Body_Angry();
                Debug.Log("set_action:3");
                break;
            case "4":
                Set_Body_FormalBow();
                Debug.Log("set_action:4");
                break;
            case "5":
                Set_Body_InformalBow();
                Debug.Log("set_action:5");
                break;
            case "6":
                Set_Body_Sad();
                Debug.Log("set_action:6");
                break;
            case "7":
                Set_Body_Waving();
                Debug.Log("set_action:7");
                break;
            default:
                Set_Face_Default();
                Debug.Log("set_action:d");
                break;
        }
    }
    public void Set_Face_Default()
    {
        animator.SetTrigger("Face_Default");
    }

    public void Set_Face_Fun()
    {
        animator.SetTrigger("Face_Fun");
    }

    public void Set_Face_Joy()
    {
        animator.SetTrigger("Face_Joy");
    }

    public void Set_Face_Surprised()
    {
        animator.SetTrigger("Face_Surprised");
    }

    public void Set_Face_Angry()
    {
        animator.SetTrigger("Face_Angry");
    }
    public void Set_Face_Sorrow()
    {
        animator.SetTrigger("Face_Sorrow");
    }

    public void Set_Body_Angry()
    {
        animator.SetTrigger("Body_Angry");
    }
    public void Set_Body_FormalBow()
    {
        animator.SetTrigger("Body_FormalBow");
    }
    public void Set_Body_Bored()
    {
        animator.SetTrigger("Body_Bored");
    }
    public void Set_Body_InformalBow()
    {
        animator.SetTrigger("Body_InformalBow");
    }
    public void Set_Body_Sad()
    {
        animator.SetTrigger("Body_Sad");
    }
    public void Set_Body_Waving()
    {
        animator.SetTrigger("Body_Waving");
    }
    public void Set_Body_Standing()
    {
        animator.SetTrigger("Body_Standing");
    }
    public void Set_Face_Talk()
    {
        animator.SetTrigger("Face_Talk");
    }
}