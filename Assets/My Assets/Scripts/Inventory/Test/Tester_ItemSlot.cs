using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class Tester_ItemSlot : MonoBehaviour {

	public ItemSlot itemSlot;
	public Item item;
	

	public void Start() {
		item = Instantiate(item);
	}
	
	[ContextMenu("Tester_CanHoldItem")]
	public void Tester_CanHoldItem() {
		Debug.Log(itemSlot.CanHoldItem(item));
	}

	[ContextMenu("Tester_AddItem")]
	public void Tester_AddItem() {
		Item itemCopy = Instantiate(item);
		Debug.Log(itemSlot.AddItem(itemCopy));
	}

	[ContextMenu("Tester_RemoveItem")]
	public void Tester_RemoveItem() {
		Item itemRemoved = itemSlot.RemoveItem();
		Debug.Log("Item removed: " + itemRemoved.name);
	}

	[ContextMenu("Tester_RemoveItemAmount")]
	public void Tester_RemoveItemAmount() {
		Item itemRemoved = itemSlot.RemoveItem(2);
		Debug.Log("Item removed: " + itemRemoved.name + ", amount: " + itemRemoved.amount);
	}

	[ContextMenu("Tester_MergeAmount")]
	public void Tester_MergeAmount() {
		Item itemCopy = Instantiate(item);
		Debug.Log("Amount left over: " + itemSlot.MergeAmount(itemCopy));
	}

	[ContextMenu("Tester_AddAmount")]
	public void Tester_AddAmount() {
		Debug.Log("Amount left over: " + itemSlot.AddAmount(5));
	}

	[ContextMenu("Tester_RemoveAmount")]
	public void Tester_RemoveAmount() {
		Item itemLeftover = itemSlot.RemoveAmount(10);
		Debug.Log(itemLeftover.name + " : " + itemLeftover.amount);
	}

	[ContextMenu("Tester_DeleteAmount")]
	public void Tester_DeleteAmount() {
		Debug.Log(itemSlot.DeleteAmount(10));
	}

}
