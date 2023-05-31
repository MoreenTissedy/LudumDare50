using System;
using Random = UnityEngine.Random;

[Serializable]
public class CatTips
{
    public string TipsText;

    public CatTips(string tip)
    {
        TipsText = tip;
    }
    
    public static CatTips CreateTips(CatTipsTextSO tips)
    {
        return new CatTips(tips.TextList[Random.Range(0, tips.TextList.Count)]);
    }

    public static CatTips CreateTips(CatTipsTextSO firstTips, CatTipsTextSO secondTips)
    {
        return new CatTips(firstTips.TextList[Random.Range(0, firstTips.TextList.Count)] + " " + 
                           secondTips.TextList[Random.Range(0, secondTips.TextList.Count)]);
    }
}
