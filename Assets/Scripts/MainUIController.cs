using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour {

	public static MainUIController Instance;

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

	[Header("IslandUI:")]

	[SerializeField]
	private TextMeshProUGUI islandNameText;

	[SerializeField]
	private TextMeshProUGUI islandDescText;

	[SerializeField]
	private Transform islandUIContainer;

	[SerializeField]
	private List<Transform> buttons;

	private Interaction currentInteraction;

	// Use this for initialization
	void Awake () {
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}
		player = GameObject.FindObjectOfType<Player>();
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

	public void SetIslandInfo(string name, string description)
	{
		islandNameText.text = name;
		islandDescText.text = description;
		islandUIContainer.gameObject.SetActive(true);

		foreach(Transform button in buttons)
		{
			Interaction interaction = InteractionManager.Instance.GetInteraction(WorldController.Instance.currentIsland.Interactions, button.gameObject.name);
			if(interaction.Completed)
			{
				TextMeshProUGUI text = button.GetChild(1).GetComponent<TextMeshProUGUI>();
				text.text = "Completed";
				text.color = Color.green;
				button.GetComponent<Button>().interactable = false;
			}
		}
	}

	public void OnRaiseAnchor()
	{
		islandUIContainer.gameObject.SetActive(false);
		worldUIContainer.gameObject.SetActive(true);
		player.RaiseAnchor();
	}

	public void OnAssign()
	{
		currentInteraction = GetInteraction();
		currentInteraction.Assigned = true;
		//StartInteraction(currentInteraction.Name);
	}

	private Interaction GetInteraction()
	{
		GameObject pressedButton = EventSystem.current.currentSelectedGameObject;
		return InteractionManager.Instance.GetInteraction(
			WorldController.Instance.currentIsland.Interactions, 
			pressedButton.tag);
	}

	private void StartInteraction(string name)
	{
		foreach(Transform button in buttons)
		{
			if(button.gameObject.name == name)
			{
				TextMeshProUGUI text = button.GetChild(1).GetComponent<TextMeshProUGUI>();
				text.text = "In Progress";
				text.color = Color.blue;
			}
		}
	}
}
