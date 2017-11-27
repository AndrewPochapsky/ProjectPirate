using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainUIController : MonoBehaviour {

	Player player;

	[SerializeField]
	private TextMeshProUGUI infamyText;
	[SerializeField]
	private TextMeshProUGUI goldText;

	// Use this for initialization
	void Awake () {
		player = GameObject.FindObjectOfType<Player>();
		player.OnInfoUpdatedEvent += SetUI;
	}
	
	private void SetUI(int infamy, int gold)
	{
		infamyText.text = "Infamy: " + infamy;
		goldText.text = "Gold: " + gold;
	}
}
