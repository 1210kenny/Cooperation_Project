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
}