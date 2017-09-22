using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystemUI : MonoBehaviour {

    [SerializeField]
    private Text turnText;

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
}
