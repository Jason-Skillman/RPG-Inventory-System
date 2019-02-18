using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class Tester_InventoryPanel : MonoBehaviour {

	public InventoryPanel inventoryPanel;
	

	public void Start() {
		
	}
	
	[ContextMenu("Tester_")]
	public void Tester_() {
		//Debug.Log(itemSlot.CanHoldItem(item));
	}
	
}
