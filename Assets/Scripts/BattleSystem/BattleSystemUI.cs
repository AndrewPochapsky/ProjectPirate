using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemUI : MonoBehaviour {

    [SerializeField]
    private Text turnText;

    [SerializeField]
    private Transform canvas;

    [SerializeField]
    private RectTransform tileOutline;

    BattleController battleController;

    private void Awake()
    {
        battleController = FindObjectOfType<BattleController>();
        battleController.OnUIValuesChangedEvent += UpdateUI;
    }

    private void UpdateUI(string _turnText)
    {
        this.turnText.text = _turnText;
    }

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

    private Vector3 NextGridPosition(Vector2 currentGridPosition,Vector3 currentPosition, int width, int height, int size)
    {
        if(currentGridPosition.x + 1 == width)
        {
            return new Vector3(0, currentPosition.y + size, -1);
        }
        return new Vector3(currentPosition.x + size, currentPosition.y, -1);
    }

    
}
