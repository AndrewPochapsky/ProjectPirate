using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IslandUI : MonoBehaviour {

	TextMeshProUGUI nameText;
	TextMeshProUGUI visitedText;
	Transform player;

	int viewingRange = 1300;

	RectTransform canvas;
	// Use this for initialization
	void Awake () {
		canvas = transform.GetChild(1).GetComponent<RectTransform>();
		nameText = canvas.GetChild(0).GetComponent<TextMeshProUGUI>();
		visitedText = canvas.GetChild(1).GetComponent<TextMeshProUGUI>();
		player = GameObject.FindObjectOfType<Player>().transform;
	}

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		InvokeRepeating("CheckIfPlayerInRange", 0, 0.5f);
	}

	public void SetUI(string name, bool visited)
	{
		nameText.text = name;
		if(visited)
		{
			visitedText.text = "Visited";
		}
		else
		{
			visitedText.text = "Not Visited";
		}
	}

	private void CheckIfPlayerInRange()
	{
		if(this != null && player != null)
			canvas.gameObject.SetActive(Vector3.Distance(player.position, transform.position) <= viewingRange);
	}
}
