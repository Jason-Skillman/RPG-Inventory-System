using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ExcludeList))]
public class InventoryPanel : MonoBehaviour {

	//public ItemDisplayer displayer;												//Reference to this panel's item displayer
	public ItemSlot itemPickup;
	
	//All of the enabled ItemSlots in this panel
	private ItemSlot[] itemSlots;
	//All of the ItemSlots in this panel
	private ItemSlot[] itemSlots_Total;

	private ExcludeList excludeList;

	/// <summary>The amount of itemSlots that are not empty in the panel</summary>
	public int ItemCount {
		get {
			if(itemSlots != null) {
				int count = 0;
				foreach(ItemSlot itemSlot in itemSlots) {
					if(!itemSlot.IsEmpty) {
						count++;
					}
				}
				return count;
			}
			return 0;
		}
	}
	//public int ItemCount { get; private set; }
	/// <summary>The total amount of ItemSlots that exist excuding disabled slots</summary>
	public int Capasity {
		get {
			if(itemSlots != null) return itemSlots.Length;
			else return 0;
		}
	}
	/// <summary>The total amount of ItemSlots that exist</summary>
	public int CapasityTotal {
		get {
			if(itemSlots_Total != null) return itemSlots_Total.Length;
			else return 0;
		}
	}
	/// <summary>The total amount of excluded ItemSlots that exist</summary>
	public int CapasityTotalExcluded {
		get {
			if(excludeList.itemSlots != null) return excludeList.itemSlots.Length;
			else return 0;
		}
	}

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
	public delegate void Callback(BaseEventData eventData, InventoryPanel panel, ItemSlot itemSlot, int index);
	public static Callback OnSlotSelectedCallback, OnSlotSubmitCallback;


	void Awake() {
		excludeList = GetComponent<ExcludeList>();

		ItemSlot.OnSelectedCallback += OnSlotSelected;
		ItemSlot.OnSubmitCallback += OnSlotSubmit;
	}
	void Start() {
		CalculateItemSlots();
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
		//Every single itemSlot in the InventoryPanel
		ItemSlot[] allItemSlots = GetComponentsInChildren<ItemSlot>();

		//Create the total and exclude list
		//List<ItemSlot> temp_itemSlots_Excluded = new List<ItemSlot>();
		List<ItemSlot> temp_itemSlots_Total = new List<ItemSlot>();
		//Loop through AllItemSlots
		for(int y = 0; y < allItemSlots.Length; y++) {
			//Loop through itemSlots_excludeList
			for(int x = 0; x < excludeList.itemSlots.Length; x++) {
				//If it is in the list
				if(allItemSlots[y] == excludeList.itemSlots[x]) {
					//temp_itemSlots_Excluded.Add(allItemSlots[y]);
				} else {	//If it is not in the list
					temp_itemSlots_Total.Add(allItemSlots[y]);
				}
			}
		}
		//itemSlots_Excluded = temp_itemSlots_Excluded.ToArray();
		itemSlots_Total = temp_itemSlots_Total.ToArray();
		

		//Create the usable ItemSlot list
		List<ItemSlot> temp_itemSlots = new List<ItemSlot>();
		//Loop through AllItemSlots
		for(int y = 0; y < allItemSlots.Length; y++) {
			if(!allItemSlots[y].isDisabled) {
				//Scan the exclude list
				bool flag = true;
				foreach(ItemSlot itemSlot_excuded in excludeList.itemSlots) {
					if(allItemSlots[y] == itemSlot_excuded) {
						flag = false;
					}
				}
				//If its not in the exclude list then add it
				if(flag)  temp_itemSlots.Add(allItemSlots[y]);
			}
		}
		itemSlots = temp_itemSlots.ToArray();
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
			//Check if there is already an existing stack somewhere in the InventoryPanel
			for(int i = 0; i < itemSlots.Length; i++) {
				//If the slot is not empty and the item in that slot matches the new item
				if(!itemSlots[i].IsEmpty && itemSlots[i].item.name == item.name) {
					//Is their is some room in the ItemSlot for the new item to go
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
						//ItemCount++;
					} else return false;
					
					break;
				}
			}
		} while(item.amount > 0);
		return true;
	}

	/// <summary>Removes the item at the index</summary>
	/// <param name="item">The index of the item to remove</param>
	public Item RemoveItem(int index, int amount = -1) {
		if(ItemCount <= 0) {
			Debug.LogWarning("Cant remove item, there are no items in this InventoryPanel");
			return null;
		}

		//Extract the item from the ItemSlot
		Item itemRemoved = itemSlots[index].RemoveItem(amount);
		if(!itemRemoved) return null;

		//if(amount <= 0)  ItemCount--;
		return itemRemoved;
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
					if(amount <= 0) {
						//Remove the item
						//ItemCount--;
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
				//ItemCount--;
				counter++;
			}
		}
		return items;
	}
	#endregion
	
	#region METHOD MESSAGES
	/// <summary>Method Message: Called when any ItemSlot is selected</summary>
	private void OnSlotSelected(BaseEventData eventData, ItemSlot itemSlot) {
		for(int i = 0; i < itemSlots.Length; i++) {
			//If the selected item slot is in our InventoryPanel
			if(itemSlots[i] == itemSlot) {
				
				//Called when any ItemSlot in this InventoryPanel is selected
				if(OnSlotSelectedCallback != null)
					OnSlotSelectedCallback(eventData, this, itemSlot, i);
				break;
			}
		}
		for(int i = 0; i < excludeList.itemSlots.Length; i++) {
			if(excludeList.itemSlots[i] == itemSlot) {
				if(OnSlotSelectedCallback != null)
					OnSlotSelectedCallback(eventData, this, itemSlot, i);
				break;
			}
		}
	}

	/// <summary>Method Message: Called when any itemSlot is submited</summary>
	private void OnSlotSubmit(BaseEventData eventData, ItemSlot itemSlot) {
		for(int i = 0; i < itemSlots.Length; i++) {
			//If the selected item slot is in our InventoryPanel
			if(itemSlots[i] == itemSlot) {

				//Called when any ItemSlot in this InventoryPanel is submited
				if(OnSlotSubmitCallback != null)
					OnSlotSubmitCallback(eventData, this, itemSlot, i);
				break;
			}
		}
		for(int i = 0; i < excludeList.itemSlots.Length; i++) {
			if(excludeList.itemSlots[i] == itemSlot) {
				
				foreach(string typeString in excludeList.limitaionTypes) {
					Type limitaionType = System.Reflection.Assembly.GetExecutingAssembly().GetType(typeString);
					
					//Debug.Log(excludeList.itemSlots[i].name, excludeList.itemSlots[i]);
					//Debug.Log(excludeList.itemSlots[i].item.GetType() + "    " + typeString);
					//if(itemSlots_excludeList.itemSlots[i].item.GetType() == limitaionType) {
						//Debug.Log("mactch");
					//}
				}
				
				if(OnSlotSubmitCallback != null)
					OnSlotSubmitCallback(eventData, this, itemSlot, i);
				break;
			}
		}
	}
	#endregion
	
	#region MISC
	/// <summary>Selects the ItemSlot with the given index in this panel</summary>
	public void SelectSlot(int index = 0) {
		DeselectAllSlots();
		if(itemSlots[index] != null)
			itemSlots[index].OnSelect(null);
	}
	/// <summary>Deselects the ItemSlot with the given index in this panel</summary>
	public void DeselectSlot(int index) {
		if(itemSlots[index] != null)
			itemSlots[index].OnDeselect(null);
	}
	/// <summary>Deselects all of the ItemSlots in this panel</summary>
	public void DeselectAllSlots() {
		foreach(ItemSlot itemSlot in itemSlots) {
			itemSlot.OnDeselect(null);
		}
	}

	[ContextMenu("EnableAllItemSlots")]
	/// <summary>Enable all of the ItemSlots in this InventoryPanel</summary>
	public void EnableAllItemSlots() {
		foreach(ItemSlot itemSlot in itemSlots_Total) {
			itemSlot.Enable();
		}
		CalculateItemSlots();
	}
	[ContextMenu("DisableAllItemSlots")]
	/// <summary>Disable all of the ItemSlots in this InventoryPanel</summary>
	public void DisableAllItemSlots() {
		foreach(ItemSlot itemSlot in itemSlots_Total) {
			itemSlot.Disable();
		}
		CalculateItemSlots();
	}

	/// <summary>Updates all of the ItemSlot displays in this panel</summary>
	public void UpdateAllSlotDisplay() {
		foreach(ItemSlot itemSlot in itemSlots) {
			itemSlot.DisplayUpdateSlot();
		}
		CalculateItemSlots();
	}

	public ItemSlot GetItemSlot(int index) {
		return itemSlots[index];
	}
	#endregion

}
