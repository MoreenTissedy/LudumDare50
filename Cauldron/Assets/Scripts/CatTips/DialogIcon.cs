using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration;
    
    public void EnableIcon()
    {
        image.DOFade(1, duration * 2).SetEase(Ease.InExpo).OnComplete((() => image.raycastTarget = true));
    }

    public void DisableIcon()
    {
        image.DOFade(0, duration).SetEase(Ease.OutExpo).OnComplete(() =>image.raycastTarget = false);
    }
}
