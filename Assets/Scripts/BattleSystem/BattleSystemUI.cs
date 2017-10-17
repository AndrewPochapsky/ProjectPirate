using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleSystemUI : MonoBehaviour {

    [SerializeField]
    private Text turnText;

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

    private Button currentPressedButton;

    Color pressedColor = Color.gray;
    BattleController battleController;

    private void Awake()
    {
        battleController = FindObjectOfType<BattleController>();
        battleController.OnUIValuesChangedEvent += UpdateUI;
    }

    private void Start()
    {
        GenerateButton(attackPanel);
        GenerateButton(attackPanel);
        GenerateButton(attackPanel);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DeactivatePanel();
        }
    }

    private void UpdateUI(BattleController.Turn turn)
    {
        this.turnText.text = turn.ToString();
        buttonsContainer.gameObject.SetActive((turn == BattleController.Turn.Player));
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
        FindPressedButton(buttons, button.tag);
    }

    /// <summary>
    /// Executes when attacks button pressed
    /// </summary>
    public void OnAttacksButtonPressed()
    {
        attackPanel.gameObject.SetActive(true);
    }

    private void FindPressedButton(Button[] buttons, string tag)
    {
        foreach (Button button in buttons)
        {
            if (button.CompareTag(tag))
            {
                button.GetComponent<Image>().color = pressedColor;
                currentPressedButton = button;
                if (!button.CompareTag("AttacksButton"))
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
        }
    }

    private void GenerateButton(RectTransform parent)
    {
        GameObject button = (GameObject)Instantiate(buttonPrefab);
        button.transform.SetParent(parent, false);
        button.transform.localScale = Vector3.one;

        //How to add onClick events to the buttons
        /* Button tempButton = button.GetComponent<Button>();
             int tempInt = i;
 
             tempButton.onClick.AddListener(() => ButtonClicked(tempInt));*/
    }
}
