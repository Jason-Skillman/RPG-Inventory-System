using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tester_Pickup : MonoBehaviour {

	public bool IsEmpty {
		get {
			return itemSlotSelect.IsEmpty;
		}
	}
	public ItemSlot itemSlotSelect;
	public InventoryPanel inventoryPanel;
	

	void Awake() {
		InventoryPanel.OnSlotSubmitCallback += OnSlotSubmitCallback_Function;
	}

	public void OnSlotSubmitCallback_Function(BaseEventData eventData, InventoryPanel panel, ItemSlot itemSlot, int index) {
		if(IsEmpty) {
			if(itemSlot.IsEmpty) return;
			
			Item itemRemoved = itemSlot.RemoveItem();
			if(itemRemoved)
				itemSlotSelect.AddItem(itemRemoved);
		} else {
			if(itemSlot.IsEmpty) {
				Item itemRemoved = itemSlotSelect.RemoveItem();
				if(itemRemoved)
					itemSlot.AddItem(itemRemoved);
			}
		}
	}

}
