using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Empty Item", order = 0)]
public class Item : ScriptableObject {

	public new string name;
	public string description;
	public int maxStack = 1;
	public int amount = 1;
	public Sprite sprite;

}
