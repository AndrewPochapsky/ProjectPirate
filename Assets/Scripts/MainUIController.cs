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
	private RectTransform assignCrewContainer;

	private RectTransform assignCrewPanel;

	[SerializeField]
	private GameObject crewMemberButton;

	[SerializeField]
	private Button raiseAnchorButton;

	[SerializeField]
	private List<Transform> interactionButtons;

	private List<Button> crewButtons;

	[SerializeField]
	private List<Button> assignCrewButtons;

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
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		crewButtons = new List<Button>();
		assignCrewPanel = assignCrewContainer.GetChild(0).GetComponent<RectTransform>();
		foreach(CrewMember member in player.crew)
		{
			crewButtons.Add(GenerateCrewButton(assignCrewPanel, member));
		}
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

		//TODO: Find better solution for this
		if(!CanRaiseAnchor())
		{
			raiseAnchorButton.interactable = false;
			TextMeshProUGUI statusText = raiseAnchorButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); 
			statusText.text = "*Job in Progress*";
			statusText.color = Color.red;
		}
		else
		{
			raiseAnchorButton.interactable = true;
			TextMeshProUGUI statusText = raiseAnchorButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); 
			statusText.text = "*Ready*";
			statusText.color = Color.blue;
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

		foreach(Transform button in interactionButtons)
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

	public void OnSurvey()
	{
		Interaction interaction = GetInteraction("survey");
		if(interaction.assignee != null && !interaction.Completed)
		{
			assignCrewContainer.gameObject.SetActive(false);
			interactionButtons[0].GetChild(1).GetComponent<TextMeshProUGUI>().text = interaction.BaseTimeRequired.ToString();
			interaction.InProgress = true;
			interactionButtons[0].GetComponent<Button>().interactable = false;
			assignCrewButtons[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "*In Progress*";
			assignCrewButtons[0].interactable = false;
			
		}
	}

	public void OnAssign()
	{
		UpdateCrewButtons();
		Button pressedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
		currentInteraction = GetInteraction(pressedButton.tag);
		assignCrewContainer.gameObject.SetActive(true);
			
	}

	private Interaction GetInteraction(string tag)
	{
		return InteractionManager.Instance.GetInteraction(
			WorldController.Instance.currentIsland.Interactions, 
			tag);
	}

	private Button GenerateCrewButton(RectTransform parent, CrewMember crewMember)
    {
        GameObject button = (GameObject)Instantiate(crewMemberButton);
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;

		SetCrewButtonText(button, crewMember);

        Button tempButton = button.GetComponent<Button>();
 
        tempButton.onClick.AddListener(() => OnCrewMemberButtonPressed(crewMember));
        return button.GetComponent<Button>();
    }

	private void OnCrewMemberButtonPressed(CrewMember crewMember)
	{
		crewMember.Task = currentInteraction;
		currentInteraction.assignee = crewMember;
		SetInteractionButtonText();

		Interaction.UnAssignDuplicateTasks(crewMember, player);

		assignCrewContainer.gameObject.SetActive(false);
	}

	private void UpdateCrewButtons()
	{
		foreach(Button button in crewButtons)
		{
			foreach(CrewMember member in player.crew)
			{
				//If crew member's name matches that on the button - pretty bad way to do this
				if(member.Name == button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
				{
					SetCrewButtonText(button.gameObject, member);
					TextMeshProUGUI taskText = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
					if(member.Task != null && member.Task.InProgress)
					{
						button.interactable = false;
						taskText.text += " in progress";
					}
					else if(member.Task != null)
					{
						taskText.text += " ready";
						button.interactable = true;
					}
					else
					{
						button.interactable = true;
					}
				}
			}
		}
	}

	private void SetCrewButtonText(GameObject button, CrewMember crewMember)
	{
		TextMeshProUGUI nameText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		TextMeshProUGUI taskText = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText.text = crewMember.Name;

		if(crewMember.Task == null)
		{
			taskText.text = "*not assigned*";
			taskText.color = Color.red;
		}
		else
		{
			taskText.text = crewMember.Task.Name;
			taskText.color = Color.black;
		}
	}

	private void SetInteractionButtonText()
	{
		foreach(Transform button in interactionButtons)
		{
			Interaction interaction = InteractionManager.Instance.GetInteraction(WorldController.Instance.currentIsland.Interactions, button.gameObject.name);
			TextMeshProUGUI text = button.GetChild(1).GetComponent<TextMeshProUGUI>();
			if(interaction.assignee != null)
			{
				text.text = interaction.assignee.Name;
				text.color = Color.blue;
			}
			else
			{
				text.text = "*Not Assigned*";
				text.color = Color.red;
			}
		}
	}

	//If any task is in progress then you cant raise anchor
	private bool CanRaiseAnchor()
	{	
		if(WorldController.Instance.currentIsland != null)
		{
			foreach(Interaction interaction in WorldController.Instance.currentIsland.Interactions)
			{
				if(interaction.InProgress)
				{
					return false;
				}
			}
		}
		
		return true;
	}

}

