using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public ItemSlot pickedUpItemSlot;

	//public InventoryTab[] inventoryTabs = new InventoryTab[7];
	public InventoryPanel[] inventoryPanels = new InventoryPanel[7];
	//public InventoryPanel panelArmorSlot;

	/// <summary>The amount of items in the entire inventory</summary>
	public int ItemCount {
		get {
			int count = 0;
			foreach(InventoryPanel panel in inventoryPanels) {
				count += panel.ItemCount;
			}
			return count;
		}
	}
	/// <summary>The max size of how many items can be held in the entire inventory</summary>
	public int Capasity {
		get {
			int capasity = 0;
			if(inventoryPanels != null) {
				foreach(InventoryPanel panel in inventoryPanels) {
					if(panel != null) {
						capasity += panel.Capasity;
					}
				}
			}
			return capasity;
		}
	}
	/// <summary>The max size of how many items could be held incuding disabled slots in the entire inventory</summary>
	public int CapasityTotal {
		get {
			int totalCapasity = 0;
			foreach(InventoryPanel panel in inventoryPanels) {
				totalCapasity += panel.CapasityTotal;
			}
			return totalCapasity;
		}
	}

	/// <summary>The index of the current selected itemSlot</summary>
	public int IndexSlotSelected { get; private set; }
	/// <summary>The index of the current selected tab</summary>
	public int IndexTabSelected { get; private set; }

	/// <summary>Is the inventory full?</summary>
	public bool IsFull { get { return ItemCount >= Capasity; } }
	/// <summary>Is the inventory open?</summary>
	public bool IsOpen { get; private set; }
	/// <summary>Have you picked up an item in the inventory</summary>
	public bool HasPickedUpItem { get { return pickedUpItemSlot.item; } }

	/// <summary>Returns the current selected itemSlot</summary>
	public ItemSlot SelectedItemSlot {
		get {
			return inventoryPanels[IndexTabSelected].GetItemSlot(IndexSlotSelected);
		}
	}
	/// <summary>Returns the current selected tab panel</summary>
	public InventoryPanel SelectedPanel {
		get {
			return inventoryPanels[IndexTabSelected];
		}
	}


	void Awake() {
		//InventoryPanel.OnSlotPanelSelectedCallback += OnSlotSelected;
		//InventoryPanel.OnSlotPanelSubmitCallback += OnSlotSumbit;
	}
	void Start() {
		//SetAll_PanelsActive(true);
		//SetAll_PanelsActive(false);

		//SelectTab(0);

		//CloseInventory();
	}
	void Update() {
		if(IsOpen) {
			/*
			//Update panel display
			if(HasPickedUpItem) {
				//Debug.Log("1");
				SelectedPanel.displayer.UpdateDisplay(pickedUpItemSlot.item);
			} else if(!SelectedItemSlot.IsEmpty) {
				//Debug.Log("2");
				SelectedPanel.displayer.UpdateDisplay(SelectedItemSlot.item);
			} else {
				SelectedPanel.displayer.ClearDisplay();
			}
			*/

			//Update itemSlot displays
			//UpdateAllActiveSlotDisplays();
			//pickedUpItemSlot.DisplayUpdateSlot();

			//Pickup 1
			//if(Input.GetButtonDown("XboxX")) {
			//	OnSlotSinglePickup(GetItemSlot(IndexTabSelected, IndexSlotSelected));
			//}
		}
	}

	/// <summary>Checks if the item can be placed into the inventory without accualy placing it into it</summary>
	public bool CanPickup(Item item) {
		//Check if the inventory is full
		if(!IsFull) {
			//Foreach
			for(int i = 0; i < inventoryPanels.Length; i++) {
				//Check if the items type and the panels type are the same
				//if(item.GetType().ToString().Equals(inventoryPanels[i].itemType.ToString())) {
					if(inventoryPanels[i].CanHoldItem(item)) {
						return true;
					} else {
						return false;
					}
				//}

			}
			Debug.LogWarning("Cant add item because the item type does not match any of the panle types");
			return false;
		} else {
			Debug.Log("Inventory is full");
			return false;
		}
	}

	/// <summary>Drops the current picked up item if their is one</summary>
	public void DropPickupItem() {
		if(HasPickedUpItem) {
			Item temp = pickedUpItemSlot.RemoveItem();
			if(SelectedItemSlot.IsEmpty) {
				if(SelectedItemSlot.AddItem(temp)) {
					//Succefully placed
				}
			} else {
				foreach(InventoryPanel panel in inventoryPanels) {
					if(panel.AddItem(temp)) {
						//Succefully placed
						break;
					}
				}
			}
		}
	}

	/*
	#region	ADD/REMOVE ITEM
	/// <summary>Adds the item into the inventory</summary>
	/// <param name="item">The item to add</param>
	public bool AddItem(Item item) {
		//Check if the inventory is full
		if(!IsFull) {
			//Foreach
			for(int i = 0; i < inventoryPanels.Length; i++) {
				//Check if the items type and the panels type are the same
				if(item.GetType().ToString().Equals(inventoryPanels[i].itemType.ToString())) {
					//Add the item
					if(inventoryPanels[i].AddItem(item)) {
						return true;
					} else {
						Debug.LogWarning("Item was not added");
						return false;
					}
				}
				//If its not the right type try again
			}
			Debug.LogWarning("Cant add item because the item type does not match any of the panle types");
			return false;
		} else {
			Debug.Log("Inventory is full");
			return false;
		}
	}
	/// <summary>Removes the item from the inventory</summary>
	/// <param name="tabIndex">The index of the tab</param>
	/// <param name="itemIndex">The index of the slot</param>
	public Item RemoveItem(int tabIndex, int itemIndex) {
		//If you have at least one item in your inventory
		if(ItemCount > 0) {
			//Remove the item
			Item item = inventoryPanels[tabIndex].RemoveItem(itemIndex);
			if(item != null) {
				//Item was succefully removed from the slot
				return item;
			}
		} else {
			Debug.LogWarning("You have no items in your inventory to remove");
		}
		return null;
	}
	/// <summary>Removes the item from the inventory</summary>
	/// <param name="name">The name of the item to remove</param>
	public Item RemoveItem(string name) {
		//If you have at least one item in your inventory
		if(ItemCount > 0) {
			//Loop through all of the panels
			for(int i = 0; i < inventoryPanels.Length; i++) {
				Item item = inventoryPanels[i].RemoveItem(name);
				if(item != null) {
					return item;
				}
				//Try the next panel
			}
			Debug.LogWarning("No item was found in any of the inventory panels with name: " + name);
		} else {
			Debug.LogWarning("You have no items in your inventory to remove");
		}
		return null;
	}
	/// <summary>Removes all of the items from the inventory</summary>
	public Item[] RemoveAllItems() {
		List<Item> allItems = new List<Item>();
		for(int i = 0; i < inventoryPanels.Length; i++) {
			Item[] items = inventoryPanels[i].RemoveAllItems();
			for(int x = 0; x < items.Length; x++) {
				allItems.Add(items[x]);
			}
		}
		Debug.Log("Size: " + allItems.Count);
		return allItems.ToArray();
	}
	#endregion
	*/
	/*
	#region SELECT TAB
	/// <summary>Selects the tab 1 over in the direction provided</summary>
	/// <param name="direction">The direction to switch too</param>
	public void SelectTab(TabDirection direction) {
		if(direction == TabDirection.Right) {
			SelectTab(IndexTabSelected + 1);
		} else {
			SelectTab(IndexTabSelected - 1);
		}
	}
	/// <summary>Selects the tab</summary>
	/// <param name="index">The index of the tab</param>
	public void SelectTab(int index) {
		int newIndex = index;
		int oldIndex = IndexTabSelected;

		if(newIndex >= 0 && newIndex < inventoryTabs.Length) {
			IndexTabSelected = newIndex;
		}

		if(newIndex >= 0 && newIndex < inventoryTabs.Length) {
			DropPickupItem();

			inventoryTabs[oldIndex].OnDeselect();
			inventoryTabs[IndexTabSelected].OnSelected();

			inventoryPanels[oldIndex].SetActive(false);
			inventoryPanels[IndexTabSelected].SetActive(true);

			inventoryPanels[oldIndex].DeselectAllSlots();
			//inventoryPanels[oldIndex].DeselectSlot(oldIndex);
			inventoryPanels[IndexTabSelected].SelectSlot(IndexSlotSelected);
		}
	}
	#endregion

	#region OPEN/CLOSE INVENTORY
	/// <summary>If the inventory is open then close it and vis versa</summary>
	public void OpenCloseInventory() {
		if(IsOpen) {
			CloseInventory();
		} else {
			OpenInventory();
		}
	}
	/// <summary>Opens the inventory</summary>
	public void OpenInventory() {
		IsOpen = true;
		gameObject.SetActive(true);

		
		//if(!SelectedItemSlot.IsEmpty) {
		//	SelectedPanel.displayer.UpdateDisplay(SelectedItemSlot.item);
		//} else {
		//	SelectedPanel.displayer.ClearDisplay();
		//}
		
	}
	/// <summary>Closes the inventory</summary>
	public void CloseInventory() {
		DropPickupItem();
		IsOpen = false;
		gameObject.SetActive(false);
	}
	#endregion
	*/
	/*
	#region METHOD MESSAGES
	bool flagThisIsStupid = false;//But i win :)
								  /// <summary>Method Message: Called when any itemSlot in the current panel is selected</summary>
								  /// <param name="itemSlot">The itemSlot that was selected</param>
								  /// <param name="index">The index of the itemSlot in the panel</param>
	private void OnSlotSelected(InventoryPanel panel, ItemSlot itemSlot, int index) {
		IndexSlotSelected = index;

		Debug.Log("3");
		SelectedPanel.displayer.UpdateDisplay(SelectedItemSlot.item);

		
		//if(HasPickedUpItem) {
		//	SelectedPanel.displayer.UpdateDisplay(pickedUpItemSlot.item);
		//} else if(!SelectedItemSlot.IsEmpty) {
		//	SelectedPanel.displayer.UpdateDisplay(SelectedItemSlot.item);
		//} else {
		//	SelectedPanel.displayer.ClearDisplay();
		//}
		

		if(flagThisIsStupid) {
			Vector2 newCoords = itemSlot.transform.position + new Vector3(pickedUpItemSlot.gameObject.GetComponent<RectTransform>().rect.width / 2, -pickedUpItemSlot.gameObject.GetComponent<RectTransform>().rect.height / 2);
			pickedUpItemSlot.transform.position = newCoords;
		} else {
			flagThisIsStupid = true;
		}

	}
	/// <summary>Method Message: Called when any itemSlot in the current panel is submited</summary>
	/// <param name="itemSlot">The itemSlot that was submited</param>
	/// <param name="index">The index of the itemSlot in the panel</param>
	private void OnSlotSumbit(InventoryPanel panel, ItemSlot itemSlot, int index) {
		if(!itemSlot.IsEmpty && !HasPickedUpItem) {
			pickedUpItemSlot.AddItem(itemSlot.RemoveItem());
		} else if(HasPickedUpItem) {
			if(itemSlot.CanHoldItem(pickedUpItemSlot.item)) {
				if(itemSlot.IsEmpty) {
					//If the slot is empty then place the item down
					itemSlot.AddItem(pickedUpItemSlot.RemoveItem());
				} else {
					//If the slot is not empty then merge the two item amounts
					itemSlot.MergeAmount(pickedUpItemSlot);
				}
			}
		}
	}
	/// <summary>Called when the single pickup button is pressed on a itemSlot</summary>
	/// <param name="itemSlot">The itemSlot that the action occured on</param>
	private void OnSlotSinglePickup(ItemSlot itemSlot) {
		if(!itemSlot.IsEmpty) {
			//Check if the item can be held
			if(pickedUpItemSlot.CanHoldItem(itemSlot.item)) {

				if(pickedUpItemSlot.IsEmpty) {
					pickedUpItemSlot.AddItem(itemSlot.RemoveAmount(1));
				} else {
					pickedUpItemSlot.MergeAmount(itemSlot, 1);
				}

			}
		}
	}
	#endregion
	*/
	/*
	#region SETTERS & GETTERS
	/// <summary>Sets all of the panels active or not</summary>
	/// <param name="value">The value to set them active</param>
	private void SetAll_PanelsActive(bool value) {
		foreach(InventoryPanel panel in inventoryPanels) {
			panel.SetActive(value);
		}
	}
	/// <summary>Updates all of the active itemSlot displays</summary>
	private void UpdateAllActiveSlotDisplays() {
		inventoryPanels[IndexTabSelected].UpdateAllSlotDisplay();
	}

	/// <summary>Returns the itemSlot at the given tabIndex and slotIndex</summary>
	/// <param name="tabIndex">The tab index</param>
	/// <param name="slotIndex">The slot index</param>
	/// <returns></returns>
	public ItemSlot GetItemSlot(int tabIndex, int slotIndex) {
		return inventoryPanels[tabIndex].GetItemSlot(slotIndex);
	}
	#endregion
	*/
	public enum TabDirection {
		Right,
		Left
	}

}
