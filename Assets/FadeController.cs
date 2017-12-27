﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController  {

	private float FadeInModifier = 1.5f;
    private float FadeOutModifier = 2f;

	public FadeController()
	{

	}

	public FadeController(float inModifier, float outModifier)
	{
		FadeInModifier = inModifier;
		FadeOutModifier = outModifier;
	}

	public void FadeCanvasGroup(bool fadeIn, CanvasGroup canvasGroup, string sceneToLoad = null)
    {
        if (fadeIn)
        {
            canvasGroup.alpha += Time.fixedDeltaTime * FadeInModifier;
            if (sceneToLoad != null && canvasGroup.alpha == 1)
                WorldController.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            canvasGroup.alpha -= Time.fixedDeltaTime * FadeOutModifier;
        }
    }

	/// <summary>
	/// Used for fading in UI objects such as the islandUI
	/// </summary>
	/// <param name="canvasGroup">The canvas group</param>
    public void CheckIfCanvasGroupInteractable(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = canvasGroup.alpha == 1;
        canvasGroup.blocksRaycasts = canvasGroup.alpha == 1;
    }
}
