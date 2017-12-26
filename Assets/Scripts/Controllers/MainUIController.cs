using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//TODO: try to clean this class up a bit :/
public class MainUIController : MonoBehaviour {

	public static MainUIController Instance;

	[HideInInspector]
	public IslandInteractionUI islandInteractionUI;

	Player player;

	[Header("WorldUI:")]
	[SerializeField]
	private TextMeshProUGUI infamyText;

	[SerializeField]
	private TextMeshProUGUI goldText;

	[SerializeField]
	private TextMeshProUGUI interactText;

	[SerializeField]
    private Transform worldUIContainer;

	[SerializeField]
	private CanvasGroup islandUICanvasGroup, panelCanvasGroup;

	public bool fadingInIslandUI { get; set; }= false;
    public bool fadingInPanel { get; set; } = false;
	public string scene { get; set; } = null;

	private float fadeInModifier = 1.5f;
	private float fadeOutModifier = 2f;

	// Use this for initialization
	void Awake () {
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}
		player = GameObject.FindObjectOfType<Player>();
		islandInteractionUI = GetComponent<IslandInteractionUI>();
		player.OnInfoUpdatedEvent += SetUI;
	}


	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		CheckIfIslandUIInteractable();

		if(WorldController.Instance.currentIsland != null && !player.anchorDropped)
		{
			interactText.enabled = true;
			interactText.text = "Press E to drop anchor at "+ WorldController.Instance.currentIsland.Name + ".";
		}
		else
		{
			interactText.enabled = false;
		}	
		//TODO: refactor all of the fade operations into another class
		FadeCanvasGroup(fadeIn: fadingInIslandUI, canvasGroup: islandUICanvasGroup);

		FadeCanvasGroup(fadeIn: fadingInPanel, canvasGroup: panelCanvasGroup, sceneToLoad: scene);
	}

	private void SetUI(int infamy, int gold)
	{
		infamyText.text = "Infamy: " + infamy;
		goldText.text = "Gold: " + gold;
	}

	//TODO: if no longer being used, remove
    public void ToggleWorldUI(bool value)
    {
        worldUIContainer.gameObject.SetActive(value);
    }

	public void FadeCanvasGroup(bool fadeIn, CanvasGroup canvasGroup, string sceneToLoad = null)
	{
		if(fadeIn)
		{
			canvasGroup.alpha += Time.fixedDeltaTime * fadeInModifier;
			if(sceneToLoad != null && canvasGroup.alpha == 1)
				WorldController.Instance.LoadScene(sceneToLoad);
				
		}
		else	
		{
			canvasGroup.alpha -= Time.fixedDeltaTime * fadeOutModifier;
		}
	}

	private void CheckIfIslandUIInteractable()
	{
		islandUICanvasGroup.interactable = islandUICanvasGroup.alpha == 1;
		islandUICanvasGroup.blocksRaycasts = islandUICanvasGroup.alpha == 1;
	}
}

