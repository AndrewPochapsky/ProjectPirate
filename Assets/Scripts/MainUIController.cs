using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
		if(WorldController.Instance.currentIsland != null && !player.anchorDropped)
		{
			interactText.enabled = true;
			interactText.text = "Press E to drop anchor at "+ WorldController.Instance.currentIsland.Name + ".";
		}
		else
		{
			interactText.enabled = false;
		}	
	}

	private void SetUI(int infamy, int gold)
	{
		infamyText.text = "Infamy: " + infamy;
		goldText.text = "Gold: " + gold;
	}

    public void ToggleWorldUI(bool value)
    {
        worldUIContainer.gameObject.SetActive(value);
    }
}

