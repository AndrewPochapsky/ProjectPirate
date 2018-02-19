using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusIndicator : MonoBehaviour {
	
	RectTransform healthBar;
	EntityData entityData;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		//First child is the background of the health bar
		healthBar = transform.GetChild(1).GetComponent<RectTransform>();
		entityData = transform.parent.GetComponent<BattleEntity>().data;
	}

	public void SetHealth()
	{
		//int perc = (int)(0.5f + ((100f * entityData.CurrentHealth) / entityData.MaxHealth));
		float value = (float)entityData.CurrentHealth / entityData.MaxHealth;
		healthBar.localScale = new Vector3(value, healthBar.localScale.y, healthBar.localScale.z);
    }
}
