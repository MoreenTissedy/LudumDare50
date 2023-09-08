using UnityEngine;

[CreateAssetMenu]
public class CatTipsProvider : ScriptableObject
{
    [Header("Tips")]
    public CatTipsTextSO SlowPlayerTips;
    public CatTipsTextSO RandomLastIngredient;

    [Header("Special character tips")]
    public CatTipsTextSO DarkStrangerTips;
    public CatTipsTextSO WitchMemoryTips;
    public CatTipsTextSO InquisitionTips;


    [Header("Scale tips")]
    public CatTipsTextSO HighFameTips;
    public CatTipsTextSO LowFameTips;
    public CatTipsTextSO HighFearTips;
    public CatTipsTextSO LowFearTips;
    [Space(10)]
    public CatTipsTextSO ScaleUPTips;
    public CatTipsTextSO ScaleDOWNTips;
}
