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
	public static Callback OnSlotPanelSelectedCallback, OnSlotPanelSubmitCallback;


	void Awake() {
		//ItemSlot.OnSelectedCallback += OnSlotSelected;
		//ItemSlot.OnSubmitCallback += OnSlotSubmit;
	}
	void Start() {
		CalculateItemSlots();
	}
	void Update() {
		//if(!EditorApplication.isPlaying) {	//Editor
		//	CalculateItemSlots();
		//}
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
		//Check if the incoming item matches this panels type
		//if(item.GetType().ToString().Equals(itemType.ToString()) || itemType == PanelType.All) {

			do {
				//Check if there is already an existing stack
				for(int i = 0; i < itemSlots.Length; i++) {
					//If the slot is not empty and the item in that slot matches the new item
					if(!itemSlots[i].IsEmpty && itemSlots[i].item.name == item.name) {
						//Is their at least one spot
						if(itemSlots[i].item.amount < itemSlots[i].item.maxStack) {
							int amountLeftInSlot = itemSlots[i].item.maxStack - itemSlots[i].item.amount;

							//would over flow
							if(item.amount > amountLeftInSlot) {
								itemSlots[i].AddAmount(amountLeftInSlot);
								item.amount -= amountLeftInSlot;

								if(IsSlotsCountFull) {
									Debug.LogWarning("Item filled up the last slot");
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

				if(!IsSlotsFull) {
					//Create a new stack
					if(item.amount > 0) {
						//Create a new item stack
						for(int i = 0; i < itemSlots.Length; i++) {
							//Find the first item slot that is empty
							if(itemSlots[i].IsEmpty) {
								
								Item newItem = Instantiate(item);
								if(item.amount > item.maxStack) {
									//temp = new Item(item.item, item.maxStack);
									newItem.amount = newItem.maxStack;
									item.amount -= item.maxStack;
								} else {
									//temp = new Item(item.item, item.amount);
									//newItem.amount = 
									item.amount = 0;
								}

								//Add the given item to the slot
								if(itemSlots[i].AddItem(newItem)) {
									//Item was succefully added the slot
									ItemCount++;
									//return true;
								} else {
									Debug.LogWarning("Item was not added");
									return false;
								}
								break;
							}
						}
					}
				} else {
					Debug.LogWarning("This panel is full");
					return false;
				}

			} while(item.amount > 0);
			return true;
		//} else {
		//	Debug.LogWarning("Cant add item because the item is not the right type");
		//	return false;
		//}
	}

	/// <summary>Removes the item at the index</summary>
	/// <param name="item">The index of the item to remove</param>
	public Item RemoveItem(int index) {
		//If you have at least one item in your inventory
		if(ItemCount > 0) {
			//Remove the item
			Item item = itemSlots[index].RemoveItem();
			if(item != null) {
				//Item was succefully removed from the slot
				ItemCount--;
				return item;
			} else {
				Debug.LogWarning("Inventory slot is already empty");
			}
		} else {
			Debug.LogWarning("There are no items in this inventory panel to remove");
		}
		return null;
	}

	/// <summary>Removes the item with the name provided</summary>
	/// <param name="item">The name of the item to remove</param>
	public Item RemoveItem(string name) {
		//If you have ant least one item in your inventory
		if(ItemCount > 0) {
			//Loop through all of the ItemSlots
			for(int i = 0; i < itemSlots.Length; i++) {
				//See if the ItemSlot is not empty else skip over it
				if(!itemSlots[i].IsEmpty) {
					//Are the names the same?
					Item item = itemSlots[i].item;
					if(item.name.Equals(name)) {
						//Remove the item
						ItemCount--;
						return itemSlots[i].RemoveItem();
					}
				}
			}
			Debug.LogWarning("No item was found in this inventory panel with name: " + name);
		} else {
			Debug.LogWarning("There are no items in this inventory panel to remove");
		}
		return null;
	}

	/// <summary>Removes all of the items in the panel</summary>
	public Item[] RemoveAllItems() {
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
			//If the selected item slot is in our inventory panel
			if(itemSlots[i] == itemSlot) {
				//PUT YOUR CODE HERE
				OnSlotPanelSelectedCallback(this, itemSlot, i);

			}
		}
	}

	/// <summary>Method Message: Called when any itemSlot is submited</summary>
	private void OnSlotSubmit(ItemSlot itemSlot) {
		for(int i = 0; i < itemSlots.Length; i++) {
			//If the selected item slot is in our inventory panel
			if(itemSlots[i] == itemSlot) {
				//PUT YOUR CODE HERE

				OnSlotPanelSubmitCallback(this, itemSlot, i);

			}
		}
	}
	#endregion
	
	#region SETTERS & GETTERS
	/// <summary>Selects the itemSlot with the given index in this panel</summary>
	public void SelectSlot(int index) {
		if(itemSlots[index] != null) {
			itemSlots[index].Select();
		}
	}
	/// <summary>Deselects the itemSlot with the given index in this panel</summary>
	public void DeselectSlot(int index) {
		if(itemSlots[index] != null) {
			itemSlots[index].OnDeselect(null);
		}
	}
	/// <summary>Deselects all of the itemSlots in this panel</summary>
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
	/// <summary>Updates all of the itemSlot displays in this panel</summary>
	public void UpdateAllSlotDisplay() {
		foreach(ItemSlot itemSlot in itemSlots) {
			itemSlot.DisplayUpdateSlot();
		}
	}
	#endregion

}

/*
public enum PanelType {
	All,
	Melee,
	Range,
	Armor,
	Misc,
	Ingredient,
	Potion,
	Special
}
*/
