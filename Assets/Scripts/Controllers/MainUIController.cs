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

	
	[SerializeField]
	private TextMeshProUGUI interactText;

	[SerializeField]
    private Transform worldUIContainer;

	[SerializeField]
	private RectTransform infamyBar;

	[SerializeField]
	private TextMeshProUGUI currentInfamyTier, nextInfamyTier, infamyValue;

	[SerializeField]
	private CanvasGroup islandUICanvasGroup, panelCanvasGroup;

	public bool fadingInIslandUI { get; set; }= false;
    public bool fadingInPanel { get; set; } = false;
	public string scene { get; set; } = null;

	FadeController fadeController;

	// Use this for initialization
	void Awake () {
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}

		fadeController = new FadeController();

		player = GameObject.FindObjectOfType<Player>();
		islandInteractionUI = GetComponent<IslandInteractionUI>();
		player.OnInfoUpdatedEvent += UpdateInfamy;
	}


	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		fadeController.CheckIfCanvasGroupInteractable(islandUICanvasGroup);

		if(WorldController.Instance.currentIsland != null && !player.anchorDropped)
		{
			interactText.enabled = true;
			interactText.text = "Press E to drop anchor at "+ WorldController.Instance.currentIsland.Name + ".";
		}
		else
		{
			interactText.enabled = false;
		}	
		//TODO: try to find a better solution than just calling this in update constantly
		fadeController.FadeCanvasGroup(fadingInIslandUI, islandUICanvasGroup, false);

		fadeController.FadeCanvasGroup(fadingInPanel, panelCanvasGroup, false,scene);
	}

	//TODO: if no longer being used, remove
    public void ToggleWorldUI(bool value)
    {
        worldUIContainer.gameObject.SetActive(value);
    }

	public void UpdateInfamy(EntityData data)
	{
		currentInfamyTier.text = data.Tier.ToString();
		nextInfamyTier.text = Entity.GetNextTier(data.Tier).ToString();

		int relativeInfamyValue = Mathf.Abs((int)data.Tier - data.Infamy);
		
		infamyValue.text = relativeInfamyValue.ToString();

		float value = (float)relativeInfamyValue / (float)Entity.GetNextTier(data.Tier);

		infamyBar.localScale = new Vector3(value, infamyBar.localScale.y, infamyBar.localScale.z);
	}
	
}

