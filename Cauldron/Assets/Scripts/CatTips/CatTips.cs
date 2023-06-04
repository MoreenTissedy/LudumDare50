using System;
using System.Collections.Generic;
using CauldronCodebase;
using Random = UnityEngine.Random;

public static class CatTipsGenerator
{
    public static CatTips CreateTips(CatTipsTextSO tips)
    {
        return new CatTips(tips.TextList[Random.Range(0, tips.TextList.Count)]);
    }

    public static CatTips CreateTips(CatTipsTextSO firstTips, CatTipsTextSO secondTips)
    {
        return new CatTips(firstTips.TextList[Random.Range(0, firstTips.TextList.Count)] + " " + 
                           secondTips.TextList[Random.Range(0, secondTips.TextList.Count)]);
    }

    public static CatTips CreateTipsWithIngredient(CatTipsTextSO tips, IngredientsData.Ingredient ingredient)
    {
        return new CatTips(String.Format(tips.TextList[Random.Range(0, tips.TextList.Count)], ingredient.friendlyName.ToLower()));
    }

    public static CatTips CreateTipsWithIngredients(CatTipsTextSO tips, List<IngredientsData.Ingredient> ingredients)
    {
        return new CatTips(String.Format(tips.TextList[Random.Range(0, tips.TextList.Count)],
            ingredients[0].friendlyName.ToLower(),
            ingredients[1].friendlyName.ToLower(),
            ingredients[2].friendlyName.ToLower()));
    }
}

[Serializable]
public class CatTips
{
    public string TipsText;

    public CatTips(string tip)
    {
        TipsText = tip;
    }
}
