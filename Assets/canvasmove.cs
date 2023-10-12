using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class canvasmove : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float showDuration = 1.0f;
    public float hideDuration = 1.0f;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false; 
    }

    public void ShowCanvas()
    {
        canvasGroup.DOFade(1, showDuration);
        canvasGroup.blocksRaycasts = true; 
    }

    public void HideCanvas()
    {
        canvasGroup.DOFade(0, hideDuration);
        canvasGroup.blocksRaycasts = false; 
    }
}