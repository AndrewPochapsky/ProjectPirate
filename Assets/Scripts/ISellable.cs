using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISellable {
	string Name { get; }
	int Amount { get; set; }
	int SellPrice { get; }
}
