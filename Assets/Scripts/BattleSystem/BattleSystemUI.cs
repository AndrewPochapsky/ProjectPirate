using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleSystemUI : MonoBehaviour {

    private enum ButtonType { Regular, Attack, Consumable };

    [SerializeField]
    private Text turnText, healthText, canMoveText;

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

    private void Awake()
    {
        battleController = FindObjectOfType<BattleController>();
        battleController.OnTurnValueChangedEvent += UpdateTurnUI;
        battleController.OnPlayerInfoChangedEvent += UpdatePlayerUI;
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
    }

    private void UpdateTurnUI(BattleController.Turn turn)
    {
        this.turnText.text = turn.ToString();
        buttonsContainer.gameObject.SetActive((turn == BattleController.Turn.Player));

        DeactivatePanel();
        battleController.Attacking = false;
        Pathfinding.DeselectNodes(battleController.Nodes);
    }

    private void UpdatePlayerUI(int? maxHealth, int? currentHealth, string canMove)
    {
        if(maxHealth != null)
        {
            healthText.text = "HP:" + currentHealth + "/" + maxHealth;
        }
        
        if(canMove != null)
        {
            canMoveText.text = "Move:" + canMove;
        }
    }

    public void GenerateAttackButtons(Entity player)
    {
        attackButtons = new Button[player.Attacks.Count];
        for(int i = 0; i < player.Attacks.Count; i++)
        {
            attackButtons[i] = GenerateButton(attackPanel, player.Attacks[i], player);
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
        Vector3 nextPosition = Vector3.zero;
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

    private Button GenerateButton(RectTransform parent, Attack attack, Entity player)
    {
        GameObject button = (GameObject)Instantiate(buttonPrefab);
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;

        button.transform.GetChild(0).GetComponent<Text>().text = attack.Name;

        Button tempButton = button.GetComponent<Button>();
 
        tempButton.onClick.AddListener(() => OnAttackButtonClicked(attack, tempButton, player));
        return button.GetComponent<Button>();
    }

    private void OnAttackButtonClicked(Attack attack, Button button, Entity player)
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
}
