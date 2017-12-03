using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {

	private int minutes = 1430;

	public string formattedTime { get; private set; }

	public float timeModifier { get; private set; } = 1f;

	// Use this for initialization
	void Start () {
		formattedTime = GetFormattedTime();
		InvokeRepeating("IncrementTime", 0f, timeModifier);
	}
	
	// Update is called once per frame
	void Update () {
		//print(GetFormattedTime());
	}

	private void IncrementTime()
	{
		//TODO: verify if this is how the time works, currently it is 23:59 -> 00:00
		minutes++;
		if(minutes == 1440)
			minutes = 0;

		formattedTime = GetFormattedTime();
	}

	public string GetFormattedTime()
	{
		string s = string.Empty;
		int h = minutes / 60;
		int m = minutes % 60;

		if(h / 10 == 0)
			s += "0" + h;
		else
			s += h;

		s += ":";

		if(m / 10 == 0)
			s += "0" + m;
		else
			s  += m;

		return s;
	}
}
