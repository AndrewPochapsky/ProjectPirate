using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO: try to clean this class up a bit :/
public class IslandInteractionUI : MonoBehaviour
{

    Player player;
    Dictionary<string, int> resourceDictionary; 


    [SerializeField]
    private TextMeshProUGUI islandNameText;

    [SerializeField]
    private TextMeshProUGUI islandDescText;

    [SerializeField]
    private TextMeshProUGUI resourceText;

    [SerializeField]
    private Transform islandUIContainer;

	[SerializeField]
	private Transform mainButtons;

	[SerializeField]
	private Transform resourcesButtonsContainer;

    [SerializeField]
    private RectTransform assignCrewContainer;

    [SerializeField]
    private TextMeshProUGUI assignCrewHeader;

    private RectTransform assignCrewPanel;

    [SerializeField]
    private GameObject crewMemberButton;

	[SerializeField]
    private GameObject resourceButton;

    [SerializeField]
    private Button raiseAnchorButton;

    [SerializeField]
    private List<Transform> interactionButtons;

    private List<Button> crewButtons;

    [SerializeField]
    private List<Button> assignCrewButtons;

    private List<Button> actualAssignCrewButtons;

    private Interaction currentInteraction;

    private List<Timer> timers;
    private List<Transform> resourceButtons;

    private int count = 0;

    // Use this for initialization
    void Awake()
    {
        
        player = GameObject.FindObjectOfType<Player>();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        crewButtons = new List<Button>();
        resourceButtons = new List<Transform>();
        timers = new List<Timer>();
        resourceDictionary = new Dictionary<string, int>();

        assignCrewPanel = assignCrewContainer.GetChild(2).GetComponent<RectTransform>();
        //Generates the crew member buttons used for assigning tasks
        foreach (CrewMember member in player.entityData.Crew)
        {
            crewButtons.Add(GenerateCrewButton(assignCrewPanel, member));
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(WorldController.Instance.currentIsland != null)
            CheckForTimers();
    }

    private void CheckForTimers()
    {
        foreach (Timer timer in timers)
        {
            if (!timer.Finished)
            {
                if (timer.Duration <= TimeController.Instance.minutes - timer.StartTime)
                {
                    timer.Finished = true;
                    //TODO: remove time object from list
                    if (timer.Interaction.OneTime)
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

    public void SetIslandInfo(string name, string description)
    {
        islandNameText.text = name;
        islandDescText.text = description;
        islandUIContainer.gameObject.SetActive(true);

        resourceButtons = new List<Transform>();
        actualAssignCrewButtons = new List<Button>();
        resourceDictionary = new Dictionary<string, int>();

        foreach(var button in assignCrewButtons)
        {
            actualAssignCrewButtons.Add(button);
        }

        foreach (Resource r in WorldController.Instance.currentIsland.Resources)
        {
            GenerateResourceButton(resourcesButtonsContainer.GetComponent<RectTransform>(), r);
        }

        for (int i = 0; i < interactionButtons.Count; i++)
        {
            Transform button = interactionButtons[i];
            TextMeshProUGUI text = button.GetChild(1).GetComponent<TextMeshProUGUI>();

            WorldController.Instance.currentIsland.Interactions[i].AssignCrewButton = actualAssignCrewButtons[i];

            Interaction interaction = InteractionManager.Instance.GetInteraction(WorldController.Instance.currentIsland.Interactions, button.gameObject.name);
            SetInteractionButtonText(button);
        }
        Interaction survey = GetInteraction("survey");
        if(survey.Completed)
        {
            resourceText.text = WorldController.Instance.currentIsland.FormattedResourceList();
        }
        else
        {
            resourceText.text = "???";
        }

       
    }

    //Runs when Raise Anchor button pressed
    public void OnRaiseAnchor()
    {
        foreach(Transform button in resourceButtons)
        {
            Destroy(button.gameObject);
        }
        resourceButtons = new List<Transform>();

        assignCrewContainer.gameObject.SetActive(false);

        resourcesButtonsContainer.gameObject.SetActive(false);
        mainButtons.gameObject.SetActive(true);

        //islandUIContainer.gameObject.SetActive(false);
		//MainUIController.Instance.ToggleWorldUI(true);
        
        MainUIController.Instance.fadingInIslandUI = false;

        player.RaiseAnchor();


        timers = new List<Timer>();
        foreach(CrewMember member in player.entityData.Crew)
        {   
            if(member.Task != null)
            {
                member.Task.assignee = null;
                member.Task = null;
            }  
        }
    }

    //Runs when Survey Island Button Pressed
    public void OnSurvey()
    {
        Interaction interaction = GetInteraction("survey");
        DoInteraction(0, interaction, false);
    }

    public void OnGatherResources()
    {
        //Interaction interaction = GetInteraction("gatherResources");
        //DoInteraction(1, interaction);
		mainButtons.gameObject.SetActive(false);
        assignCrewContainer.gameObject.SetActive(false);
		resourcesButtonsContainer.gameObject.SetActive(true);
    }

	public void OnBack_GatherResources()
	{
		mainButtons.gameObject.SetActive(true);
        assignCrewContainer.gameObject.SetActive(false);
        resourcesButtonsContainer.gameObject.SetActive(false);
	}

	/// <summary>
	/// The listener method for each resource button
	/// </summary>
	/// <param name="resource">The resource</param>
	private void OnResourceGathered(Resource resource)
	{
        int index = resourceDictionary[resource.Name];
        Interaction interaction = GetInteraction(resource.Name);
        
		//Call DoInteraction
        DoInteraction(index, interaction, true);
	}

    //Called when any assignCrew button pressed
    public void OnAssign(string tag)
    {
        UpdateCrewButtons();
        Button pressedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        
        if(tag == string.Empty)
            currentInteraction = GetInteraction(pressedButton.tag);
        else
            currentInteraction = GetInteraction(tag);

        assignCrewHeader.text = currentInteraction.DisplayName;

        assignCrewContainer.gameObject.SetActive(true);
    }

    private void DoInteraction(int index, Interaction interaction, bool isResource)
    {
        if (interaction.assignee != null && !interaction.Completed)
        {
            assignCrewContainer.gameObject.SetActive(false);
            TextMeshProUGUI statusText = null;
            if(!isResource)
            {
                statusText = interactionButtons[index].GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            else
            {
                statusText = resourceButtons[index].GetChild(1).GetComponent<TextMeshProUGUI>();
            }
            
            interaction.InProgress = true;

            if(!isResource)
            {
                interactionButtons[index].GetComponent<Button>().interactable = false;
            }
               
            else
            {
                resourceButtons[index].GetComponent<Button>().interactable = false;
            }
                

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
    
	private Button GenerateResourceButton(RectTransform parent, Resource resource)
	{
		GameObject button = (GameObject)Instantiate(resourceButton);
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;
        SetInteractionButtonText(button.transform, resource);

        Button assignCrewButton = button.transform.GetChild(2).GetComponent<Button>();
        
        button.GetComponent<Button>().onClick.AddListener(() => OnResourceGathered(resource));
        assignCrewButton.onClick.AddListener(() => OnAssign(resource.Name));
        resourceButtons.Add(button.transform);
        actualAssignCrewButtons.Add(assignCrewButton);

        resourceDictionary.Add(resource.Name, resourceButtons.Count-1);

        Interaction interaction = GetInteraction(resource.Name);

        interaction.AssignCrewButton = assignCrewButton;
        

		return button.GetComponent<Button>();
	}

    private void OnCrewMemberButtonPressed(CrewMember crewMember)
    {
        if(crewMember.Task != null)
        {
            crewMember.Task.assignee = null;
        }
        crewMember.Task = currentInteraction;
        
        currentInteraction.assignee = crewMember;

        //Interaction.UnAssignDuplicateTasks(crewMember, player);

        foreach(Transform button in interactionButtons)
            SetInteractionButtonText(button);
        
        for (int i = 0; i < WorldController.Instance.currentIsland.Resources.Count; i++)
        {
            SetInteractionButtonText(resourceButtons[i], WorldController.Instance.currentIsland.Resources[i]);
        }

        assignCrewContainer.gameObject.SetActive(false);
    }

    private void UpdateCrewButtons()
    {
        foreach (Button button in crewButtons)
        {
            foreach (CrewMember member in player.entityData.Crew)
            {
                //If crew member's name matches that on the button - pretty bad way to do this
                if (member.Name == button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text)
                {
                    SetCrewButtonText(button.gameObject, member);
                    TextMeshProUGUI taskText = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    if (member.Task != null && member.Task.InProgress)
                    {
                        button.interactable = false;
                        taskText.text += " in progress";
                    }
                    else if (member.Task != null)
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

    //TODO: consider merging this with UpdateCrewButtons()
    private void SetCrewButtonText(GameObject button, CrewMember crewMember)
    {
        TextMeshProUGUI nameText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI taskText = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        nameText.text = crewMember.Name;

        if (crewMember.Task == null)
        {
            taskText.text = "*not assigned*";
            taskText.color = Color.red;
        }
        else
        {
            taskText.text = crewMember.Task.DisplayName;
            taskText.color = Color.black;
        }
    }

    private void SetInteractionButtonText(Transform button, Resource resource = null)
    {
        Interaction interaction = GetInteraction(button.gameObject.name);
        
        if(resource != null)
            interaction = GetInteraction(resource.Name);

        TextMeshProUGUI nameText = button.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI statusText = button.GetChild(1).GetComponent<TextMeshProUGUI>();

        Button assignCrewButton = button.GetChild(2).GetComponent<Button>();
        TextMeshProUGUI assignCrewText = assignCrewButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if(resource != null && interaction.Completed)
        {
            statusText.text = "Depleted";
        
            interaction.AssignCrewButton.interactable = false;
            if (assignCrewButton != null)
                assignCrewText.text = "Depleted";
        }
        else if(resource != null && !GetInteraction(interaction.Prerequisite.ToString()).Completed)
        {
            nameText.text = "???";
            statusText.text = "";
            if (assignCrewButton != null)
                assignCrewButton.interactable = false;
        }
        else if (interaction.Prerequisite != Interaction.Type.none && !GetInteraction(interaction.Prerequisite.ToString()).Completed)
        {
            statusText.text = "requires: " + interaction.Prerequisite.ToString();
            statusText.color = Color.red;
            button.GetComponent<Button>().interactable = false;
            assignCrewButton.interactable = false;
        }
        else if (!interaction.OneTime && !interaction.Completed && interaction.assignee == null)
        {
            if(resource != null)
                nameText.text = resource.Name;
            assignCrewButton.interactable = true;
            statusText.text = "*not assigned*";
            statusText.color = Color.red;
            if (assignCrewButton != null)
                assignCrewText.text = "Assign Crew";
        }
        else if (interaction.OneTime && interaction.Completed || interaction.Completed)
        {
            statusText.text = "Completed";
            if (assignCrewButton != null)
                assignCrewText.text = "Completed";
            statusText.color = Color.blue;
        }
        else if (interaction.assignee != null)
        {
            statusText.text = interaction.assignee.Name;
            statusText.color = Color.blue;
            button.GetComponent<Button>().interactable = true;
        }
        else
        {
            statusText.text = "*Not Assigned*";
            assignCrewButton.interactable = true;
            assignCrewText.text = "Assign Crew";
            statusText.color = Color.red;
        }
    }

    private bool CheckIfCanRaiseAnchor()
    {
        TextMeshProUGUI statusText = raiseAnchorButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        if (WorldController.Instance.currentIsland != null)
        {
            foreach (Interaction interaction in WorldController.Instance.currentIsland.Interactions)
            {
                if (interaction.InProgress)
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
        switch (interaction.InteractionType)
        {
            case Interaction.Type.survey:
                resourceText.text = WorldController.Instance.currentIsland.FormattedResourceList();
                //update the resource buttons
                for(int i = 0; i < resourceButtons.Count; i++)
                {
                    SetInteractionButtonText(resourceButtons[i], WorldController.Instance.currentIsland.Resources[i]);
                }
				break;

            case Interaction.Type.gatherResources:
                Resource resource = (Resource)WorldController.Instance.currentIsland.Resources.Where(r => r.Name == interaction.DisplayName).First();
                int amount = UnityEngine.Random.Range(1, resource.Amount);
                player.AddResource(resource, amount, WorldController.Instance.currentIsland);

                if(resource.Amount == 0)
                {
                    //Destroy(interaction.AssignCrewButton.transform.parent.gameObject);
                    interaction.Completed = true;
                }
                resourceText.text = WorldController.Instance.currentIsland.FormattedResourceList();
               

                print(player.FormattedInventory());

                break;
        }

        interaction.assignee.Task = null;
        interaction.assignee = null;

        foreach(Transform button in interactionButtons)
            SetInteractionButtonText(button);

        for (int i = 0; i < WorldController.Instance.currentIsland.Resources.Count; i++)
        {
            SetInteractionButtonText(resourceButtons[i], WorldController.Instance.currentIsland.Resources[i]);
        }
    }
}

