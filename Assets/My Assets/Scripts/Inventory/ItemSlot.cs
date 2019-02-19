using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class ItemSlot : Selectable, ISubmitHandler {
	
	/// <summary>The main item reference</summary>
	public Item item;

	//Display Componets
	public Image imageBackground;
	public Image imageSelect;
	public Image imageItem;
	public Text textAmount;
	public Slider slider;
	
	/// <summary>Is their an item in this slot</summary>
	public bool IsEmpty { get { return item == null; } }
	/// <summary>Is this slot currently selected</summary>
	public bool isSelected = false;
	/// <summary>Is this slot disabled</summary>
	public bool isDisabled = false;
	
	//Delegates
	public delegate void Callback(ItemSlot itemSlot);
	public static Callback OnSelectedCallback, OnDeselectedCallback, OnSubmitCallback;


	protected override void Awake() {
		base.Awake();
		if(!Application.isPlaying) return;
	}


	protected override void Start() {
		base.Start();
		if(!Application.isPlaying) return;
		
		DisplayUpdateSlot();
		
		if(isDisabled) Disable();
		else Enable();
	}

	void Update() {
		if(!Application.isPlaying) return;
		
		//Update the slider/durability
		if(!IsEmpty) {
			if(item.GetType() == typeof(Melee)) {
				Melee melee = (Melee)item;
				if(melee.durability != melee.durabilityMax) {
					slider.gameObject.SetActive(true);
					slider.maxValue = melee.durabilityMax;
					slider.value = (float)Math.Ceiling(melee.durability);
				} 
			} else if(item.GetType() == typeof(Range)) {
				Range range = (Range)item;
				if(range.durability != range.durabilityMax) {
					slider.gameObject.SetActive(true);
					slider.maxValue = range.durabilityMax;
					slider.value = (float)Math.Ceiling(range.durability);
				}
			} else {
				slider.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>Checks if the item can be added to this itemSlot without accualy adding it.</summary>
	public bool CanHoldItem(Item inItem) {
		if(IsEmpty) {
			return true;
		} else if(inItem.name.Equals(item.name)) {
			if(item.amount < item.maxStack) {
				return true;
			}
		}
		return false;
	}

	#region ADD/REMOVE ITEM
	/// <summary>Adds the given item to this ItemSlot, returns true if the item was placed in the slot and false if it was not.</summary>
	/// <param name="itemReceived">The item to add to the slot</param>
	public bool AddItem(Item itemReceived) {
		if(isDisabled) {
			Debug.LogWarning("Cant add item, this itemSlot is disabled");
			return false;
		}
		if(!IsEmpty) {
			Debug.LogWarning("Cant add item, this itemSlot is already occupied");
			return false;
		}

		item = itemReceived;
		DisplayUpdateSlot();

		return true;
	}
	/// <summary>Removes the item in the slot and returns it, returns null if slot is empty.</summary>
	public Item RemoveItem() {
		if(isDisabled) {
			Debug.LogWarning("Cant remove item, this itemSlot is disabled");
			return null;
		}
		if(IsEmpty) {
			Debug.LogWarning("Cant remove item, the itemSlot is already empty");
			return null;
		}
		
		Item itemTemp = Instantiate(item);
		item = null;

		DisplayClearSlot();

		return itemTemp;
	}
	#endregion

	#region ADD/REMOVE AMOUNT
	/// <summary>Merges the given amount to this itemSlot and leaves it and returns the left over amount. Returns -1 if it fails.</summary>
	/// <param name="inItem">The item merging</param>
	public int MergeAmount(Item inItem) {
		if(isDisabled) {
			Debug.LogWarning("Cant merge items, this itemSlot is disabled");
			return -1;
		}
		if(IsEmpty) {
			Debug.LogWarning("Cant merge items, this itemSlot is empty");
			return -1;
		}
		if(item.GetType() != inItem.GetType()) {
			Debug.LogWarning("Cant merge items, the incoming item is not the same type as this itemSlot");
			return -1;
		}
		
		//Can we fit all of the incoming amount into our item
		if(item.amount + inItem.amount <= item.maxStack) {
			item.amount += inItem.amount;
			//inItem.amount = 0;
			inItem = null;
			
			DisplayUpdateSlot();
			return 0;
		}

		//We cant fit everything so we are going to have some left over
		int leftover = (item.amount + inItem.amount - item.maxStack);
		inItem.amount = leftover;
		item.amount = item.maxStack;

		DisplayUpdateSlot();
		return leftover;
	}
	[Obsolete]
	public void MergeAmount_OLD(ItemSlot newItemSlot) {
		int leftover = 0;
		if(item.amount + newItemSlot.item.amount <= item.maxStack) {
			item.amount += newItemSlot.item.amount;    //Take all of the amount and add it into this itemSlot
			newItemSlot.RemoveItem();                   //Remove the item
			DisplayUpdateSlot();
		} else {
			//we are going to have some left over
			leftover = (item.amount + newItemSlot.item.amount - item.maxStack);
			newItemSlot.item.amount = leftover;
			item.amount = item.maxStack;

			DisplayUpdateSlot();
		}
		//return leftover;
	}
	[Obsolete]
	public void MergeAmount_OLD(ItemSlot newItemSlot, int amount) {

		if(newItemSlot.item.amount < amount) {
			return;
		}

		//Can the incoming amount fit into my slot
		if(item.amount + amount <= item.maxStack) {
			//Can we take that much out of the other slot
			if(newItemSlot.item.amount - amount >= 0) {
				item.amount += amount;
				newItemSlot.item.amount -= amount;
				if(newItemSlot.item.amount == 0) newItemSlot.RemoveItem();
			}
		}
	}

	/// <summary>Adds the given amount to the item stack and returns the amount leftover, if any.</summary>
	/// <param name="amount">The amount to add</param>
	public int AddAmount(int amount) {
		if(isDisabled) {
			Debug.LogWarning("Cant add amount, this itemSlot is disabled");
			return 0;
		}
		if(IsEmpty) {
			Debug.LogWarning("Cant add amount, this itemSlot is empty");
			return 0;
		}
		
		//Can we fit all of the incoming amount into our item
		if(item.amount + amount <= item.maxStack) {
			item.amount += amount;

			DisplayUpdateSlot();
			return 0;
		}

		//We cant fit everything so we are going to have some left over
		int leftover = (item.amount + amount - item.maxStack);
		item.amount = item.maxStack;

		DisplayUpdateSlot();
		return leftover;
	}
	/// <summary>Removes the given amount from this item stack and returns it as a new item with that amount.</summary>
	/// <param name="amount">The amount to remove</param>
	public Item RemoveAmount(int amount) {
		if(isDisabled) {
			Debug.LogWarning("Cant remove amount, this itemSlot is disabled");
			return null;
		}
		if(IsEmpty) {
			Debug.LogWarning("Cant remove amount, this itemSlot is empty");
			return null;
		}
		if(amount > item.amount) {
			Debug.LogWarning("Cant remove amount, this itemSlot does not have enough to remove");
			return null;
		}

		Item itemCopy = Instantiate(item);
		itemCopy.amount = amount;
		item.amount -= amount;

		if(item.amount <= 0) RemoveItem();

		DisplayUpdateSlot();
		return itemCopy;
	}
	/// <summary>Deletes the given amount from this item stack and returns true if it did.</summary>
	/// <param name="amount">The amount to delete</param>
	public bool DeleteAmount(int amount) {
		if(isDisabled) {
			Debug.LogWarning("Cant delete amount, this itemSlot is disabled");
			return false;
		}
		if(IsEmpty) {
			Debug.LogWarning("Cant delete amount, this itemSlot is empty");
			return false;
		}

		item.amount -= amount;

		if(item.amount <= 0) RemoveItem();

		DisplayUpdateSlot();
		return true;
	}
	#endregion

	#region DISPLAY
	/// <summary>Updates the display with the Item data</summary>
	[ContextMenu("DisplayUpdateSlot")]
	public void DisplayUpdateSlot() {
		if(!IsEmpty) {
			//Shows the item sprite
			imageItem.sprite = item.sprite;
			imageItem.color = new Color(255, 255, 255, 1);
		
			//Show the amount in the stack if it is greater then 1
			if(item.amount > 1) textAmount.text = item.amount.ToString();
			else textAmount.text = "";
		} else {
			DisplayClearSlot();
		}
	}

	/// <summary>Clears the display of the Item data</summary>
	[ContextMenu("DisplayClearSlot")]
	private void DisplayClearSlot() {
		//Clear the item sprite
		imageItem.sprite = null;
		imageItem.color = new Color(255, 255, 255, 0);

		//Clear the select sprite
		imageSelect.enabled = false;

		//Clear the stack amount
		textAmount.text = "";

		//Clear the slider
		slider.gameObject.SetActive(false);
	}
	#endregion
	
	#region ENABLE/DISABLE
	/// <summary>Sets the slot to be enabled</summary>
	[ContextMenu("Enable")]
	public void Enable() {
		isDisabled = false;

		DisplayUpdateSlot();
		
		//Reset the background slot 
		imageBackground.color = new Color(0.54f, 0.54f, 0.54f);

		//Update the navagation
		Navigation navAuto = new Navigation();
		navAuto.mode = Navigation.Mode.Automatic;
		navigation = navAuto;
	}

	/// <summary>Sets the slot to be disabled</summary>
	[ContextMenu("Disable")]
	public void Disable() {
		isDisabled = true;
		
		item = null;
		DisplayClearSlot();

		//Dim the background slot 
		imageBackground.color = new Color(0.2f, 0.2f, 0.2f);

		//Update the navagation
		Navigation navNone = new Navigation();
		navNone.mode = Navigation.Mode.None;
		navigation = navNone;
	}
	#endregion

	#region UNITY MESSAGES
	/// <summary>Selectable: Called when this UIGameObject is selected</summary>
	public override void OnSelect(BaseEventData eventData) {
		if(!Application.isPlaying) return;

		isSelected = true;
		imageSelect.enabled = true;

		if(OnSelectedCallback != null) OnSelectedCallback(this);
	}
	
	/// <summary>Selectable: Called when this UIGameObject is deselected</summary>
	public override void OnDeselect(BaseEventData eventData) {
		if(!Application.isPlaying) return;

		isSelected = false;
		imageSelect.enabled = false;

		if(OnDeselectedCallback != null) OnDeselectedCallback(this);
	}
	
	///<summary>ISubmitHandler: Called when you submit this button</summary>
	public void OnSubmit(BaseEventData eventData) {
		if(!Application.isPlaying) return;

		if(OnSubmitCallback != null) OnSubmitCallback(this);
	}
	#endregion
	
}
