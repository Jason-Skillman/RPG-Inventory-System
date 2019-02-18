using UnityEngine;

[CreateAssetMenu(fileName = "Range", menuName = "Items/Range", order = 2)]
public class Range : Item {

	public float damageMin = 1;
	public float damageMax = 10;
	public float durabilityMax = 100;
	public float durability = 100;
	public int pierce;
	public int criticalChance;

	public RangeType type;

	//Hold
	public float holdTime;			//Number of seconds until reached max power


	//Click
	public float fireDelay;			//The delay in seconds until you can fire again


	public enum RangeType {
		Hold,
		Click
	}

}
