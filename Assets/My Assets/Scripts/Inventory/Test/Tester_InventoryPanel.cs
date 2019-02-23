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

		//InventoryPanel.OnSlotSelectedCallback += OnSlotSelectedCallback_Function;
	}

	[ContextMenu("Tester_CanHoldItem")]
	public void Tester_CanHoldItem() {
		Item temp = Instantiate(item);
		Debug.Log(inventoryPanel.CanHoldItem(temp));
	}

	[ContextMenu("Tester_AddItem")]
	public void Tester_AddItem() {
		Item temp = Instantiate(item);
		Debug.Log(inventoryPanel.AddItem(temp));
	}

	[ContextMenu("Tester_RemoveItemIndex")]
	public void Tester_RemoveItemIndex() {
		Item itemTemp = inventoryPanel.RemoveItem(0, 6);
		Debug.Log(itemTemp.name);
	}

	[ContextMenu("Tester_RemoveItemString")]
	public void Tester_RemoveItemString() {
		Item itemTemp = inventoryPanel.RemoveItem("Skull", 10);
		if(itemTemp)
			Debug.Log(itemTemp.name);
	}

	[ContextMenu("Tester_RemoveAllItems")]
	public void Tester_RemoveAllItems() {
		Item[] itemArr = inventoryPanel.RemoveAllItems();
		foreach(Item item in itemArr) {
			Debug.Log(item.name);
		}
	}

	[ContextMenu("Tester_SelectSlot")]
	public void Tester_SelectSlot() {
		inventoryPanel.SelectSlot(0);
	}

	[ContextMenu("Tester_DeselectSlot")]
	public void Tester_DeselectSlot() {
		inventoryPanel.DeselectSlot(0);
	}

	[ContextMenu("Tester_DeselectAllSlots")]
	public void Tester_DeselectAllSlots() {
		inventoryPanel.DeselectAllSlots();
	}


	public void OnSlotSelectedCallback_Function(InventoryPanel panel, ItemSlot itemSlot, int index) {
		Debug.Log("Selected", itemSlot);
	}

}
