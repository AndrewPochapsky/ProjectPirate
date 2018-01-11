using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class BattleSystemUI : MonoBehaviour {

    private enum ButtonType { Regular, Attack };

    [SerializeField]
    private CanvasGroup panel;

    [Header("Battle Summary")]
    [SerializeField]
    private CanvasGroup summaryPanel;

    [SerializeField]
    private Button continueButton, doneButton;
    
    [SerializeField]
    private TextMeshProUGUI resultText, infamyHeader, infamyChange, currentInfamyTier, nextInfamyTier, infamyValue, itemsHeader, itemsValue;

    [SerializeField]
    private Image divider;

    [SerializeField]
    private RectTransform infamyBar;

    [Header("Other UI")]
    [SerializeField]
    private Text canMoveText;

    [SerializeField]
    private TextMeshProUGUI turnText;

    [SerializeField]
    private Transform canvas;

    [SerializeField]
    private RectTransform tileOutline;

    [SerializeField]
    private RectTransform buttonsContainer;

    [SerializeField]
    private RectTransform attackPanel;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private Button[] buttons;

    private Button[] attackButtons;

    private Button currentPressedButton, currentPressedAttackButton;

    Color pressedColor = Color.gray;
    BattleController battleController;
    FadeController fadeController;
    
    [HideInInspector]
    public bool fadeOut = false;

    private void Awake()
    {
        DOTween.Init();

        battleController = FindObjectOfType<BattleController>();
        battleController.OnTurnValueChangedEvent += UpdateTurnUI;
        battleController.OnPlayerInfoChangedEvent += UpdatePlayerUI;
        battleController.OnBattleOverEvent += OnBattleOver;
        fadeController = new FadeController();

        continueButton.interactable = false;
        summaryPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeactivatePanel();
            if (battleController.Attacking)
            {
                Pathfinding.DeselectNodes(battleController.Nodes);
                battleController.Attacking = false;
            }
        }
        //TODO: use DOTween for this
        if(!fadeOut)
            fadeController.FadeCanvasGroup(fadeOut, panel, false);
        else
            fadeController.FadeCanvasGroup(fadeOut, panel, true, "World");
    }

    private void UpdateTurnUI(BattleController.Turn turn)
    {   
        Color transitionColor = Color.black;
        if (turn == BattleController.Turn.Player)
            transitionColor = Color.blue;
        else
            transitionColor = Color.red;
        
        float fadeDuration = 0.75f;

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence
            .Append(turnText.DOFade(0, fadeDuration))
            .Append(turnText.DOText(turn + " Turn", 0))
            .Append(turnText.DOColor(transitionColor, 0))
            .Append(turnText.DOFade(1, fadeDuration));          

        //fadeSequence.SetEase(Ease.InFlash);

        buttonsContainer.gameObject.SetActive((turn == BattleController.Turn.Player));

        DeactivatePanel();
        battleController.Attacking = false;
        Pathfinding.DeselectNodes(battleController.Nodes);
    }

    private void UpdatePlayerUI(int? maxHealth, int? currentHealth, string canMove)
    {
        if(maxHealth != null)
        {
            //healthText.text = "HP:" + currentHealth + "/" + maxHealth;
        }
        
        if(canMove != null)
        {
            canMoveText.text = "Move:" + canMove;
        }
    }

    public void GenerateAttackButtons(BattleEntity player)
    {
        print("called: " + player.data.Attacks.Count);
        attackButtons = new Button[player.data.Attacks.Count];
        for(int i = 0; i < player.data.Attacks.Count; i++)
        {
            print("Generating 1");
            attackButtons[i] = GenerateButton(attackPanel, player.data.Attacks[i], player);
        }
    }

    /// <summary>
    /// Populates world space canvas with grid borders
    /// </summary>
    /// <param name="width">The width</param>
    /// <param name="height">The height</param>
    /// <param name="size">The size</param>
    public void CreateGrid(int width, int height, int size)
    {
        Vector3 nextPosition = new Vector3(0, 0, -1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                RectTransform outline = Instantiate(tileOutline, canvas) as RectTransform;
                outline.sizeDelta = new Vector2(size, size);
                outline.localPosition = nextPosition;

                Vector2 currentGridPos = new Vector2(x, y);
                nextPosition = NextGridPosition(currentGridPos, outline.localPosition, width, height, size);
            }
        }
    }

    private Vector3 NextGridPosition(Vector2 currentGridPosition, Vector3 currentPosition, int width, int height, int size)
    {
        if(currentGridPosition.x + 1 == width)
        {
            return new Vector3(0, currentPosition.y + size, -1);
        }
        return new Vector3(currentPosition.x + size, currentPosition.y, -1);
    }

    /// <summary>
    /// Executed when any button is pressed(Not end turn)
    /// </summary>
    public void OnAnyButtonPressed()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        FindPressedButton(buttons, button.GetComponent<Button>(), ButtonType.Regular);
    }

    public void OnRepairPressed()
    {

    }

    /// <summary>
    /// Executes when attacks button pressed
    /// </summary>
    public void OnAttacksButtonPressed()
    {
        attackPanel.gameObject.SetActive(true);
    }

    private void FindPressedButton(Button[] buttons, Button pressedButton, ButtonType buttonType)
    {
        foreach (Button button in buttons)
        {
            if (button == pressedButton)
            {
                button.GetComponent<Image>().color = pressedColor;
                if (buttonType == ButtonType.Attack)
                {
                    currentPressedAttackButton = button;
                }
                else if(buttonType == ButtonType.Regular)
                {
                    currentPressedButton = button;
                }
                
                if (buttonType != ButtonType.Attack && !button.CompareTag("AttacksButton"))
                {
                    attackPanel.gameObject.SetActive(false);
                }
            }
            else
            {
                button.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void DeactivatePanel()
    {
        //TODO find a better way to do this
        attackPanel.gameObject.SetActive(false);
        if(currentPressedButton != null)
        {
            currentPressedButton.GetComponent<Image>().color = Color.white;
            currentPressedButton = null;
        }
        if(currentPressedAttackButton != null)
        {
            currentPressedAttackButton.GetComponent<Image>().color = Color.white;
            currentPressedAttackButton = null;
        }
    }

    /// <summary>
    /// Generates attack buttons
    /// </summary>
    /// <param name="parent">Parent</param>
    /// <param name="attack">The attack</param>
    /// <param name="player">The player</param>
    /// <returns>The created button</returns>
    private Button GenerateButton(RectTransform parent, Attack attack, BattleEntity player)
    {
        print("Generating");
        GameObject button = (GameObject)Instantiate(buttonPrefab);
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;

        button.transform.GetChild(0).GetComponent<Text>().text = attack.Name;

        Button tempButton = button.GetComponent<Button>();
 
        tempButton.onClick.AddListener(() => OnAttackButtonClicked(attack, tempButton, player));
        return button.GetComponent<Button>();
    }

    private void OnAttackButtonClicked(Attack attack, Button button, BattleEntity player)
    {
        if(currentPressedAttackButton == null)
        {
            currentPressedAttackButton = button;
        }

        List<Node> rangeNodes = Pathfinding.GetRange(battleController.Nodes, player.nodeParent, attack.Range);

        Pathfinding.DeselectNodes(battleController.Nodes);
        Pathfinding.SelectNodes(rangeNodes, Color.cyan);

        FindPressedButton(attackButtons, button, ButtonType.Attack);

        battleController.lastSelectedAttack = attack;
        battleController.Attacking = true;
    }

    private void OnBattleOver(BattleController.BattleStatus status)
    {
        if(status == BattleController.BattleStatus.PlayerVic)
        {
            resultText.text = "Victory";
        }
        else
        {
            resultText.text = "Defeat";
        }
        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(resultText.DOFade(1, 0.5f))
            .AppendInterval(0.5f)
            .Append(continueButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
     
        continueButton.interactable = true;
        continueButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnContinuePressed()
    {
        BattleData data = Resources.Load<BattleData>("Data/BattleData");

        if(data.InfamyReward > 0) 
            infamyChange.color = Color.green;
        else 
            infamyChange.color = Color.red;


        infamyChange.text = data.InfamyReward.ToString();
        
        infamyValue.text = Mathf.Abs((int)data.Friendlies[0].Tier - data.Friendlies[0].Infamy).ToString();

        if(data.Items.Count == 0)
        {
            itemsValue.text = "none";
        }
        else
        {
            for(int i = 0; i < data.Items.Count; i++)
            {
                var item = data.Items[i];
                itemsValue.text += item.Name + "("+item.Amount + ")";
                if(i+1 != data.Items.Count)
                {
                    itemsValue.text+=", ";  
                }
            }
        }

        infamyBar.localScale = new Vector3((float)data.Friendlies[0].Infamy / (float)Entity.GetNextTier(data.Friendlies[0].Tier), infamyBar.localScale.y, infamyBar.localScale.z);

        infamyChange.alpha = 0;
        summaryPanel.gameObject.SetActive(true);

        summaryPanel.blocksRaycasts = true;

        Sequence sequence = DOTween.Sequence();
        sequence
            .Append(summaryPanel.DOFade(1, 0.5f))
            .Append(infamyHeader.DOFade(1, 0.5f))
            .Append(infamyChange.DOFade(1, 0.5f));
        
        //Scale the infamyChange text up and then down
        sequence.Insert(1, infamyChange.DOScale(2f, 0.25f));
        sequence.Insert(1.25f, infamyChange.DOScale(1f, 0.25f));

        int originalInfamyValue = data.Friendlies[0].Infamy;
        float interval = 0.25f;

        int temp = Mathf.Abs((int)data.Friendlies[0].Tier - originalInfamyValue);

        sequence
            .AppendInterval(0.25f)
            .Append(currentInfamyTier.DOText(data.Friendlies[0].Tier.ToString(), 0))
            .Append(currentInfamyTier.DOFade(1, 0.5f))
            .Append(nextInfamyTier.DOText(Entity.GetNextTier(data.Friendlies[0].Tier).ToString(), 0))
            .Append(nextInfamyTier.DOFade(1, 0.5f))
            .Append(infamyValue.DOFade(1, 0.5f))
            .Append(infamyBar.DOScale(new Vector3((float)temp / (float)Entity.GetNextTier(data.Friendlies[0].Tier), infamyBar.localScale.y, infamyBar.localScale.z), 0))
            .Append(infamyBar.GetComponent<Image>().DOFade(1, 0.5f));
        
        
        for(int i = 0; i < Mathf.Abs(data.InfamyReward); i++)
        {
            string s = "";
            float value = 0;
            
            if(data.InfamyReward > 0)
            {
                temp++;
                
                if(temp == (int)Entity.GetNextTier(data.Friendlies[0].Tier))
                {
                    Entity.InfamyTier newLevel = Entity.LevelUp(data.Friendlies[0]);
                    if(newLevel != Entity.InfamyTier.Null)
                    {
                        sequence.Append(currentInfamyTier.DOText(newLevel.ToString(), 0));

                        //TODO: Prevent the text from becoming "Null"(IT CURRENTLY DOES IF THE LEVEL IS MAX)
                        sequence.Append(nextInfamyTier.DOText(Entity.GetNextTier(newLevel).ToString(), 0));
                        temp = 0;
                    }
                }

                s = temp.ToString();
                value = (float)temp/ (float)Entity.GetNextTier(data.Friendlies[0].Tier);
            }
            else
            {
                if (temp <= 0)
                {
                    break;
                }

                temp--;

                s = temp.ToString();
                value = (float)temp/ (float)Entity.GetNextTier(data.Friendlies[0].Tier);
            }
            
            sequence.Append(infamyValue.DOText(s, 0));
            sequence.Join(infamyBar.DOScale(new Vector3(value, infamyBar.localScale.y, infamyBar.localScale.z), 0));
            sequence.AppendInterval(interval);
            interval *= 0.95f;
        }
        
        sequence
            .Append(divider.DOFade(1, 0.5f))
            .Append(itemsHeader.DOFade(1, 0.5f))
            .Append(itemsValue.DOFade(1, 0.5f))
            .Append(doneButton.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
    }

    public void OnDonePressed()
    {
        fadeOut = true;
    }
}
