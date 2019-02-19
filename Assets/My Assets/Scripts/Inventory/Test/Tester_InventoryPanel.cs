using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class Tester_InventoryPanel : MonoBehaviour {

	public InventoryPanel inventoryPanel;
	public Item item;


	public void Start() {
		item = Instantiate(item);
	}

	[ContextMenu("Tester_CanHoldItem")]
	public void Tester_() {
		Item temp = Instantiate(item);
		Debug.Log(inventoryPanel.CanHoldItem(temp));
	}
	
}
