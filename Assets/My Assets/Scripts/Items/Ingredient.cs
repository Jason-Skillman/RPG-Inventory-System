using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Items/Ingredient", order = 5)]
public class Ingredient : Item {

	public IngredientType ingredientType;

	public enum IngredientType {
		Base,
		Part,
		Essence
	}

	#region BASE
	public UseType useType;

	public enum UseType {
		Self,
		Throw,
		ThrowBomb,
		ThrowTrap,
		ThrowSticky,
		PlaceBomb,
		PlaceTrap
	}
	#endregion

	#region PART
	public int powerNumber;
	public int powerPercent;
	#endregion

	#region ESSENCE
	public PotionEffect potionEffect;

	public enum PotionEffect {
		Health,
		Fire,
		Poison,
	}
	#endregion
	
}
