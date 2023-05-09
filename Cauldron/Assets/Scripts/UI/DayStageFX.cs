using DG.Tweening;
using TMPro;
using UnityEngine;

public class DayStageFX : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpTextComponent;
    [SerializeField] private string text;
    [SerializeField] private CanvasGroup canvasGroup;
    public float duration;

    private void Start()
    {
        tmpTextComponent.text = text;

        canvasGroup.DOFade(1, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
