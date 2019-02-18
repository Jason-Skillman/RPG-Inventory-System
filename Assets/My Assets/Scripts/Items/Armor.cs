using UnityEngine;

[CreateAssetMenu(fileName = "Armor", menuName = "Items/Armor", order = 3)]
public class Armor : Item {

	public int armor;
	public ArmorType type;

	public enum ArmorType {
		Helmet,
		Chestplate,
		Boots,
		Shield
	}
}
