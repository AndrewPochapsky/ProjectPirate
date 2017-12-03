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
	private List<Transform> interactionButtons;

	private List<Button> crewButtons;

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
			interactionButtons[0].GetChild(1).GetComponent<TextMeshProUGUI>().text = "in progress";
		}
	}

	public void OnAssign()
	{
		UpdateCrewButtons();
		currentInteraction = GetInteraction(EventSystem.current.currentSelectedGameObject.tag);
		assignCrewContainer.gameObject.SetActive(true);
		//StartInteraction(currentInteraction.Name);
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

		UnAssignDuplicateTasks(crewMember);

		assignCrewContainer.gameObject.SetActive(false);
	}

	private void UpdateCrewButtons()
	{
		foreach(Button button in crewButtons)
		{
			foreach(CrewMember member in player.crew)
			{
				if(member.Name == button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
				{
					SetCrewButtonText(button.gameObject, member);
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

	private void UnAssignDuplicateTasks(CrewMember crewMember)
	{
		foreach(CrewMember member in player.crew)
		{
			if(crewMember.Name != member.Name &&  crewMember.Task == member.Task)
			{
				member.Task = null;
			}
		}
	}
}
