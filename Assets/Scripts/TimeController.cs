using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour {

	public static TimeController Instance;

	public int minutes{ get; private set; } = 1430;

	public string formattedTime { get; private set; }

	public float timeModifier { get; private set; } = 1f;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	// Use this for initialization
	void Start () {
		formattedTime = GetFormattedTime(minutes, true);
		InvokeRepeating("IncrementTime", 0f, timeModifier);
	}

	private void IncrementTime()
	{
		//TODO: verify if this is how the time works, currently it is 23:59 -> 00:00
		minutes++;
		if(minutes == 1440)
			minutes = 0;

		formattedTime = GetFormattedTime(minutes, true);
	}

	public string GetFormattedTime(int _minutes, bool standard)
	{
		string s = string.Empty;
		int h = _minutes / 60;
		int m = _minutes % 60;
		//Gives the format like 00:00
		if(standard)
		{
			if(h / 10 == 0)
				s += "0" + h;
			else
				s += h;

			s += ":";

			if(m / 10 == 0)
				s += "0" + m;
			else
				s  += m;
		}
		else
		{
			if(h > 0)
			{
				s+=h +"h ";
			}
			s+=m+"min";
		}
		

		return s;
	}
}

public class Timer
{
	public int StartTime { get; private set; }
	public int Duration { get; private set; }
	public TMPro.TextMeshProUGUI Text { get; private set; }
	public Interaction Interaction { get; private set; }
	public bool Finished { get; set; }

	public Timer(int startTime, int duration, TMPro.TextMeshProUGUI text, Interaction interaction)
	{
		StartTime = startTime;
		Duration = duration;
		Text = text;
		Interaction = interaction;
		Finished = false;
	}
}
