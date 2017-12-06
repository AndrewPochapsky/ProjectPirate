using System.Collections;
using System;
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
	private TextMeshProUGUI resourceText;

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

	private List<Timer> timers;

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
		timers = new List<Timer>();
		assignCrewPanel = assignCrewContainer.GetChild(0).GetComponent<RectTransform>();
		//Generates the crew member buttons used for assigning tasks
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

		foreach(Timer timer in timers)
		{
			if(!timer.Finished)
			{
				if(timer.Duration <= TimeController.Instance.minutes - timer.StartTime)
				{
					timer.Finished = true;
					//TODO: remove time object from list
					if(timer.Interaction.OneTime)
					{
						timer.Text.text = "Completed";
						timer.Interaction.Completed = true;
					}
				
					timer.Interaction.InProgress = false;
					CheckIfCanRaiseAnchor();
					FinishInteraction(timer.Interaction);
				}
				else
				{
					timer.Text.text = TimeController.Instance.GetFormattedTime(Mathf.Abs(timer.Duration - Mathf.Abs(timer.StartTime - TimeController.Instance.minutes)), false);
				}
			}
			
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

		for(int i = 0 ; i < interactionButtons.Count; i++)
		{
			Transform button = interactionButtons[i];
			TextMeshProUGUI text = button.GetChild(1).GetComponent<TextMeshProUGUI>();

			WorldController.Instance.currentIsland.Interactions[i].AssignCrewButton = assignCrewButtons[i];
			
			Interaction interaction = InteractionManager.Instance.GetInteraction(WorldController.Instance.currentIsland.Interactions, button.gameObject.name);
			if(interaction.OneTime && interaction.Completed)
			{	
				text.text = "Completed";
				text.color = Color.blue;
				button.GetComponent<Button>().interactable = false;
			}
			if(interaction.Prerequisite != Interaction.Type.none && !GetInteraction(interaction.Prerequisite.ToString()).Completed)
			{
				text.text = "requires: " + interaction.Prerequisite.ToString();
				text.color = Color.red;
				button.GetComponent<Button>().interactable = false;
				//if(interaction.AssignCrewButton != null)
				interaction.AssignCrewButton.interactable = false;
			}
			
		
		}
	}

	//Runs when Raise Anchor button pressed
	public void OnRaiseAnchor()
	{
		islandUIContainer.gameObject.SetActive(false);
		worldUIContainer.gameObject.SetActive(true);
		player.RaiseAnchor();
	}

	//Runs when Survey Island Button Pressed
	public void OnSurvey()
	{
		Interaction interaction = GetInteraction("survey");
		DoInteraction(0, interaction);
	}

	public void OnGatherResources()
	{
		Interaction interaction = GetInteraction("gatherResources");
		DoInteraction(1, interaction);
	}

	//Called when any assignCrew button pressed
	public void OnAssign()
	{
		UpdateCrewButtons();
		Button pressedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
		currentInteraction = GetInteraction(pressedButton.tag);
		assignCrewContainer.gameObject.SetActive(true);
	}

	private void DoInteraction(int index, Interaction interaction)
	{
		if(interaction.assignee != null && !interaction.Completed)
		{
			assignCrewContainer.gameObject.SetActive(false);

			TextMeshProUGUI statusText = interactionButtons[index].GetChild(1).GetComponent<TextMeshProUGUI>();

			interaction.InProgress = true;

			interactionButtons[index].GetComponent<Button>().interactable = false;

			TextMeshProUGUI assignCrewText = interaction.AssignCrewButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			assignCrewText.text = "*In Progress*";

			interaction.AssignCrewButton.interactable = false;

			CheckIfCanRaiseAnchor();
			
			timers.Add(new Timer(TimeController.Instance.minutes, interaction.Duration, statusText, interaction));
		}
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
			taskText.text = crewMember.Task.InteractionType.ToString();
			taskText.color = Color.black;
		}
	}

	private void SetInteractionButtonText()
	{
		foreach(Transform button in interactionButtons)
		{
			Interaction interaction = InteractionManager.Instance.GetInteraction(WorldController.Instance.currentIsland.Interactions, button.gameObject.name);
			TextMeshProUGUI text = button.GetChild(1).GetComponent<TextMeshProUGUI>();
			
			if(interaction.Prerequisite != Interaction.Type.none && !GetInteraction(interaction.Prerequisite.ToString()).Completed)
			{
				text.text = "requires: " + interaction.Prerequisite.ToString();
				text.color = Color.red;
				button.GetComponent<Button>().interactable = false;
				interaction.AssignCrewButton.interactable = false;
			}
			else if(!interaction.OneTime && !interaction.Completed && interaction.assignee == null)
			{
				interaction.AssignCrewButton.interactable = true;
				text.text = "*not assigned*";
				text.color = Color.red;
				interaction.AssignCrewButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Assign Crew";
			}
			else if(interaction.OneTime && interaction.Completed)
			{
				text.text = "Completed";
				interaction.AssignCrewButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Completed";
				text.color = Color.blue;
			}
			else if(interaction.assignee != null)
			{
				text.text = interaction.assignee.Name;
				text.color = Color.blue;
				button.GetComponent<Button>().interactable = true;
			}
			else
			{
				text.text = "*Not Assigned*";
				text.color = Color.red;
			}
			
		}
	}

	private bool CheckIfCanRaiseAnchor()
	{	
		TextMeshProUGUI statusText = raiseAnchorButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>(); 
		if(WorldController.Instance.currentIsland != null)
		{
			foreach(Interaction interaction in WorldController.Instance.currentIsland.Interactions)
			{
				if(interaction.InProgress)
				{
					raiseAnchorButton.interactable = false;
					statusText.text = "*Job in Progress*";
					statusText.color = Color.red;
					return false;
				}
			}
		}

		raiseAnchorButton.interactable = true;
		statusText.text = "*Ready*";
		statusText.color = Color.blue;
		return true;
	}

	private void FinishInteraction(Interaction interaction)
	{
		switch(interaction.InteractionType)
		{
			case Interaction.Type.survey:
				resourceText.text = WorldController.Instance.currentIsland.FormattedResourceList();
				break;
			
			case Interaction.Type.gatherResources:
				print("Finished gathering resources");
				break;
		}

		interaction.assignee.Task = null;
		interaction.assignee = null;

		SetInteractionButtonText();
	}
}

