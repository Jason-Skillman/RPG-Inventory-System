using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(InventoryPanel), true)]
public class InventoryPanelEditor : Editor {
	
	public override void OnInspectorGUI() {
		InventoryPanel main = (InventoryPanel)target;

		//base.OnInspectorGUI();

		//Title
		GUILayout.Space(5);
		EditorGUILayout.LabelField("Inventory Panel", EditorStyles.boldLabel);
		GUILayout.Space(5);



		//Componets
		//main.itemType = (PanelType)EditorGUILayout.EnumPopup("Type", main.itemType);
		//main.displayer = (ItemDisplayer)EditorGUILayout.ObjectField("Displayer", main.displayer, typeof(ItemDisplayer), true);
		//GUILayout.Space(10);
		
		//Readonly
		EditorGUILayout.LabelField("Inventory: " + main.ItemCount + "/" + main.Capasity);
		EditorGUILayout.LabelField("Enabled Slots: " + main.Capasity + "/" + main.CapasityTotal);
		EditorGUILayout.LabelField("Excluded Slots: " + main.CapasityTotalExcluded);
		GUILayout.Space(5);

		EditorGUILayout.LabelField("Is Slots Full: " + main.IsSlotsFull);
		EditorGUILayout.LabelField("Is Items Full: " + main.IsSlotsCountFull);
		GUILayout.Space(5);


		EditorUtility.SetDirty(main);
	}

}
