using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class CatTipsProvider : ScriptableObject
{
    [Header("Tips")]
    [Expandable]
    public CatTipsTextSO SlowPlayerTips;
    [Expandable]
    public CatTipsTextSO RandomLastIngredient;

    [Header("Special character tips")]
    [Expandable]
    public CatTipsTextSO DarkStrangerTips;
    [Expandable]
    public CatTipsTextSO WitchMemoryTips;
    [Expandable]
    public CatTipsTextSO InquisitionTips;


    [Header("Scale tips")]
    [Expandable]
    public CatTipsTextSO HighFameTips;
    [Expandable]
    public CatTipsTextSO LowFameTips;
    [Expandable]
    public CatTipsTextSO HighFearTips;
    [Expandable]
    public CatTipsTextSO LowFearTips;
    [Space(10)]
    [Expandable]
    public CatTipsTextSO ScaleUPTips;
    [Expandable]
    public CatTipsTextSO ScaleDOWNTips;
}
