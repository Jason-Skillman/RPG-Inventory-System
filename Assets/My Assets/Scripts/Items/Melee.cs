using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Items/Melee", order = 1)]
public class Melee : Item {

	public float damageMin = 1;
	public float damageMax = 10;
	public float durabilityMax = 100;
	public float durability = 100;
	public int pierce;
	public int criticalChance;

	public float speed = 1;

	//public ConditionType conditionType;
	//public int conditionPower;
	//public float conditionChance;
	
}
