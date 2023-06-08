using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration;

    private bool active;
    
    public void EnableIcon()
    {
        if (active)
        {
            return;
        }
        active = true;
        image.DOFade(1, duration * 2).SetEase(Ease.InExpo).OnComplete((() => image.raycastTarget = true));
    }

    public void DisableIcon()
    {
        if (!active)
        {
            return;
        }
        active = false;
        image.DOFade(0, duration).SetEase(Ease.OutExpo).OnComplete(() =>image.raycastTarget = false);
    }
}
