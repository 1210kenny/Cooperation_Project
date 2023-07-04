using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        // 获取Animator组件
        animator = GetComponent<Animator>();
    }

    public void Set_Face_Default()
    {
        animator.SetTrigger("Default");
    }

    public void Set_Face_Fun()
    {
        animator.SetTrigger("Fun");
    }

    public void Set_Face_Joy()
    {
        animator.SetTrigger("Joy");
    }

    public void Set_Face_Surprised()
    {
        animator.SetTrigger("Surprised");
    }

    public void Set_Face_Angry()
    {
        animator.SetTrigger("Angry");
    }
    public void Set_Face_Sorrow()
    {
        animator.SetTrigger("Sorrow");
    }

    public void Set_Body_Angry()
    {
        animator.SetTrigger("Body_angry");
    }
    public void Set_Body_FormalBow()
    {
        animator.SetTrigger("Body_FormalBow");
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
        animator.SetTrigger("Standing");
    }
}