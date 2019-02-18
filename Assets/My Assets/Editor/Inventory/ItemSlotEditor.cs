using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ItemSlot), true)]
public class ItemSlotEditor : SelectableEditor {
	
	public override void OnInspectorGUI() {
		ItemSlot main = (ItemSlot)target;

		//Title
		GUILayout.Space(5);
		EditorGUILayout.LabelField("Selectable", EditorStyles.boldLabel);
		GUILayout.Space(5);

		base.OnInspectorGUI();



		//Title
		GUILayout.Space(5);
		EditorGUILayout.LabelField("Item Slot", EditorStyles.boldLabel);
		GUILayout.Space(5);

		//Componets
		main.imageBackground = (Image)EditorGUILayout.ObjectField("Image Background", main.imageBackground, typeof(Image), true);
		main.imageSelect = (Image)EditorGUILayout.ObjectField("Image Select", main.imageSelect, typeof(Image), true);
		main.imageItem = (Image)EditorGUILayout.ObjectField("Image Item", main.imageItem, typeof(Image), true);
		main.slider = (Slider)EditorGUILayout.ObjectField("Slider", main.slider, typeof(Slider), true);
		main.textAmount = (Text)EditorGUILayout.ObjectField("Text Amount", main.textAmount, typeof(Text), true);

		GUILayout.Space(10);

		//Readonly variables
		string str = "Item: ";
		if(!main.IsEmpty) {
			str += main.item.name;
		}
		EditorGUILayout.LabelField(str);

		//Item obj
		main.item = (Item)EditorGUILayout.ObjectField("Item", main.item, typeof(Item), true);

		EditorGUILayout.LabelField("Is Empty: " + main.IsEmpty);
		EditorGUILayout.LabelField("Selected: " + main.isSelected);
		//EditorGUILayout.LabelField("Disabled: " + main.isDisabled);
		GUILayout.Space(10);

		main.isDisabled = EditorGUILayout.Toggle("Is Disabled", main.isDisabled);
		


		GUILayout.Space(10);
		EditorUtility.SetDirty(main);
	}

}
