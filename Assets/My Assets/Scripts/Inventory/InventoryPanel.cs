using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class InventoryPanel : MonoBehaviour {

	//public PanelType itemType;													//The type of items that can be put into this panel
	//public ItemDisplayer displayer;												//Reference to this panel's item displayer
	
	private ItemSlot[] itemSlots;                                                //All of the enabled itemslots in this panel

	/// <summary>The amount of itemSlots that are not empty in the panel</summary>
	public int ItemCount { get; private set; }
	/// <summary>The max size of how many items can be held in the panel</summary>
	public int Capasity {
		get {
			if(itemSlots != null) return itemSlots.Length;
			else return 0;
		}
	}
	/// <summary>The max size of how many items could be held incuding disabled slots</summary>
	public int TotalCapasity { get; private set; }

	/// <summary>Is all of the slots in the panel full</summary>
	public bool IsSlotsFull { get { return (ItemCount >= Capasity); } }
	/// <summary>Is all of the slots max item count full. Is every stack maxed out?</summary>
	public bool IsSlotsCountFull {
		get {
			if(!IsSlotsFull) {
				return false;
			}

			if(itemSlots != null) {
				for(int i = 0; i < itemSlots.Length; i++) {
					if(itemSlots[i].IsEmpty || itemSlots[i].item.amount < itemSlots[i].item.maxStack) {
						return false;
					}
				}
			}
			return true;
		}
	}

	//Delegates
	public delegate void Callback(InventoryPanel panel, ItemSlot itemSlot, int index);
	public static Callback OnSlotSelectedCallback, OnSlotSubmitCallback;


	void Awake() {
		ItemSlot.OnSelectedCallback += OnSlotSelected;
		ItemSlot.OnSubmitCallback += OnSlotSubmit;
	}
	void Start() {
		CalculateItemSlots();
	}
	void Update() {
		
	}
	
	/// <summary>Checks if the item can be added to the panel without accualy adding it</summary>
	public bool CanHoldItem(Item item) {
		if(IsSlotsCountFull) {
			return false;
		}
		
		for(int i = 0; i < itemSlots.Length; i++) {
			if(IsSlotsFull) {
				if(!itemSlots[i].IsEmpty && itemSlots[i].item.name == item.name) {
					if(itemSlots[i].item.amount < itemSlots[i].item.maxStack) {
						return true;
					}
				}
			} else {
				return true;
			}
		}
		return false;
	}

	[ContextMenu("CalculateItemSlots")]
	/// <summary>Recalculates how many slots are available for this slot to use</summary>
	public void CalculateItemSlots() {
		ItemSlot[] itemSlotArr = GetComponentsInChildren<ItemSlot>();
		TotalCapasity = itemSlotArr.Length;

		//Extract only the itemSlots that are not disabled
		List<ItemSlot> itemSlotList = new List<ItemSlot>();
		for(int i = 0; i < itemSlotArr.Length; i++) {
			if(!itemSlotArr[i].isDisabled) {
				itemSlotList.Add(itemSlotArr[i]);
			}
		}
		itemSlots = itemSlotList.ToArray();

		//Check if their are any items in the items slots
		ItemCount = 0;	//Reset the count
		foreach(ItemSlot itemSlot in itemSlots) {
			if(!itemSlot.IsEmpty) {
				ItemCount++;
			}
		}
	}
	
	
	#region ADD/REMOVE ITEM
	/// <summary>Adds the item to the item panel</summary>
	/// <param name="item">The item to add</param>
	public bool AddItem(Item item) {
		if(!item) {
			Debug.LogWarning("Cant add item, the incoming item is null");
			return false;
		}
		if(item.amount <= 0) {
			Debug.LogWarning("Cant add item, the incoming item is empty");
			return false;
		}
		if(IsSlotsCountFull) {
			Debug.LogWarning("Cant add item, this InventoryPanel is completly full");
			return false;
		}
		do {
			//Check if there is already an existing stack somewhere in the inventoryPanel
			for(int i = 0; i < itemSlots.Length; i++) {
				//If the slot is not empty and the item in that slot matches the new item
				if(!itemSlots[i].IsEmpty && itemSlots[i].item.name == item.name) {
					//Is their is some room in the itemSlot for the new item to go
					if(itemSlots[i].item.amount < itemSlots[i].item.maxStack) {
						int amountLeftInSlot = itemSlots[i].item.maxStack - itemSlots[i].item.amount;

						//Overflow
						if(item.amount > amountLeftInSlot) {
							itemSlots[i].AddAmount(amountLeftInSlot);
							item.amount -= amountLeftInSlot;

							if(IsSlotsCountFull) {
								Debug.Log("Item filled up the last slot");
								return false;
							}
						} else {
							itemSlots[i].AddAmount(item.amount);
							item.amount = 0;
						}

						if(IsSlotsFull) {
							return true;
						}
					}
				}
			}

			if(IsSlotsFull) {
				Debug.LogWarning("Cant add item, the InventoryPanel's slots are full");
				return false;
			}

			//Create a new stack to store the item
			for(int i = 0; i < itemSlots.Length; i++) {
				//Find the first item slot that is empty
				if(itemSlots[i].IsEmpty) {

					//Create the new stack and fill its properties
					Item newItem = Instantiate(item);
					if(item.amount > item.maxStack) {
						newItem.amount = newItem.maxStack;
						item.amount -= item.maxStack;
					} else {
						item.amount = 0;
					}

					//Add the given item to the slot
					if(itemSlots[i].AddItem(newItem)) {
						//Item was succefully added the slot
						ItemCount++;
					} else return false;
					
					break;
				}
			}
		} while(item.amount > 0);
		return true;
	}

	/// <summary>Removes the item at the index</summary>
	/// <param name="item">The index of the item to remove</param>
	public Item RemoveItem(int index) {
		if(ItemCount <= 0) {
			Debug.LogWarning("Cant remove item, there are no items in this InventoryPanel");
			return null;
		}
		
		//Extract the item from the ItemSlot
		Item item = itemSlots[index].RemoveItem();
		if(!item) {
			Debug.LogWarning("Cant remove item, the ItemSlot is already empty");
			return null;
		}

		//Remove the item from the ItemSlot
		ItemCount--;
		return item;
	}

	/// <summary>Removes the item with the name provided</summary>
	/// <param name="item">The name of the item to remove</param>
	/// /// <param name="amount">The amount to remove</param>
	public Item RemoveItem(string name, int amount = -1) {
		if(ItemCount <= 0) {
			Debug.LogWarning("Cant remove item, there are no items in this InventoryPanel");
			return null;
		}

		//Loop through all of the ItemSlots
		for(int i = 0; i < itemSlots.Length; i++) {
			//See if the ItemSlot is not empty else skip over it
			if(!itemSlots[i].IsEmpty) {
				//Are the names the same?
				Item item = itemSlots[i].item;
				if(item.name.Equals(name)) {
					if(amount < 0) {
						//Remove the item
						ItemCount--;
						return itemSlots[i].RemoveItem();
					} else {
						Item itemRemoved = itemSlots[i].RemoveItem(amount);
						if(itemRemoved) return itemRemoved;
						else return null;
					}
				}
			}
		}

		Debug.LogWarning("Cant remove item, no item was found in this InventoryPanel with name: " + name);
		return null;
	}
	
	/// <summary>Removes all of the items in the panel</summary>
	public Item[] RemoveAllItems() {
		if(ItemCount <= 0) {
			Debug.LogWarning("Cant remove items, there are no items in this InventoryPanel");
			return null;
		}

		Item[] items = new Item[ItemCount];
		int counter = 0;
		for(int i = 0; i < itemSlots.Length; i++) {
			//Only delete the slots with items in them
			if(!itemSlots[i].IsEmpty) {
				items[counter] = itemSlots[i].RemoveItem();
				ItemCount--;
				counter++;
			}
		}
		return items;
	}
	#endregion
	
	#region METHOD MESSAGES
	/// <summary>Method Message: Called when any itemSlot is selected</summary>
	private void OnSlotSelected(ItemSlot itemSlot) {
		for(int i = 0; i < itemSlots.Length; i++) {
			//If the selected item slot is in our InventoryPanel
			if(itemSlots[i] == itemSlot) {
				
				//Called when any ItemSlot in this InventoryPanel is selected
				OnSlotSelectedCallback(this, itemSlot, i);
			}
		}
	}

	/// <summary>Method Message: Called when any itemSlot is submited</summary>
	private void OnSlotSubmit(ItemSlot itemSlot) {
		for(int i = 0; i < itemSlots.Length; i++) {
			//If the selected item slot is in our InventoryPanel
			if(itemSlots[i] == itemSlot) {

				//Called when any ItemSlot in this InventoryPanel is submited
				OnSlotSubmitCallback(this, itemSlot, i);
			}
		}
	}
	#endregion
	
	#region SETTERS & GETTERS
	/// <summary>Selects the ItemSlot with the given index in this panel</summary>
	public void SelectSlot(int index = 0) {
		if(itemSlots[index] != null) {
			itemSlots[index].Select();
		}
	}
	/// <summary>Deselects the ItemSlot with the given index in this panel</summary>
	public void DeselectSlot(int index) {
		if(itemSlots[index] != null) {
			itemSlots[index].OnDeselect(null);
		}
	}
	/// <summary>Deselects all of the ItemSlots in this panel</summary>
	public void DeselectAllSlots() {
		foreach(ItemSlot itemSlot in itemSlots) {
			itemSlot.OnDeselect(null);
		}
	}

	public ItemSlot GetItemSlot(int index) {
		return itemSlots[index];
	}

	/// <summary>Sets this panel active or not</summary>
	public void SetActive(bool value) {
		gameObject.SetActive(value);
		CalculateItemSlots();
	}
	/// <summary>Updates all of the ItemSlot displays in this panel</summary>
	public void UpdateAllSlotDisplay() {
		foreach(ItemSlot itemSlot in itemSlots) {
			itemSlot.DisplayUpdateSlot();
		}
	}
	#endregion

}
