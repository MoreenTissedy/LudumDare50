using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
public class PotionTooltip : MonoBehaviour
{
  IngredientDroppable[] ingredientsInScene = new IngredientDroppable[10];
  Image[] ingImages = new Image[3];
  static Recipe recipy;
  RecipeBookEntry recipeBook;
  SpriteRenderer[] currentIngredients = new SpriteRenderer [3];
  
  static bool isVisible = false;

  void Start()
  {
    ingredientsInScene = FindObjectsOfType<IngredientDroppable>();
  }

  public void SwitchToolTip()
  {
    //Debug.Log(recipy);
    //Debug.Log(GetComponentInParent<RecipeBookEntry>().currentRecipe);
    if (!isVisible)
    {
      ShowTooltip();
    }
    else if ((isVisible) && (recipy != GetComponentInParent<RecipeBookEntry>().currentRecipe))
    {
      HideTooltip();
      ShowTooltip();
    }
    else
    {
      HideTooltip();
    }
  }

  void ShowTooltip()
  {
    isVisible = true;
    recipy = GetComponentInParent<RecipeBookEntry>().currentRecipe;
    recipeBook = GetComponentInParent<RecipeBookEntry>();
    ingImages[0] = recipeBook.ingredient1;
    ingImages[1] = recipeBook.ingredient2;
    ingImages[2] = recipeBook.ingredient3;
    for (int i = 0; i<3; i++)
    {
        for (int c =0; c< 10; c++)
        {
            //Debug.Log(ingredientsInScene[c].name);
            //Debug.Log(ingredientsInScene[c].GetComponentInChildren<SpriteRenderer>().sprite.name);    
            if(ingImages[i].sprite == ingredientsInScene[c].GetComponentInChildren<SpriteRenderer>().sprite)
            {
                currentIngredients[i] = ingredientsInScene[c].GetComponentInChildren<SpriteRenderer>();
                currentIngredients[i].color = Color.black; 
                //Debug.Log(ingImages[i].sprite.name);
            }
        }
    }
  }

  void HideTooltip()
  {
    //Debug.Log("выключить");
    for (int i = 0; i<10; i++)
    {
       ingredientsInScene[i].GetComponentInChildren<SpriteRenderer>().color = Color.white; 
       //Debug.Log(temp[i].sprite.name);
    }
    isVisible = false;
  }

}
}