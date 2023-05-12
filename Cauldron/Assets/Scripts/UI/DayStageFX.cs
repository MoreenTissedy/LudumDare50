using CauldronCodebase;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DayStageFX : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpTextComponent;
    [SerializeField] private string text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Sounds sound;
    public float duration;

    public SoundManager SoundManager;

    private void Start()
    {
        //TODO: Add sound
        //SoundManager.Play(sound);
        
        tmpTextComponent.text = text;
        canvasGroup.DOFade(1, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
    }
}
