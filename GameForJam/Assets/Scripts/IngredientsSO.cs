using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace  DefaultNamespace
{
[CreateAssetMenu(menuName = "Ingredient", fileName = "New Ingredient")]
public class IngredientsSO : ScriptableObject
{
    [SerializeField] string ingredientName;
    [SerializeField] Sprite ingredientImage;
    [SerializeField] Ingredients id;

    public string IngredientName {get {return ingredientName;}}
    public Sprite IngredientImage {get {return ingredientImage;}}
    

}
}
