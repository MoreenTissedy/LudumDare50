using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vibrate : MonoBehaviour
{
	public float frequency = 1f;
	public bool verticalMotion = true;
	[Range(-1, 1)]
	public float amountVertical = 0.3f;
	public bool horizontalMotion = false;
	[Range(-1, 1)]
	public float amountHorizontal = 0f;
    // Start is called before the first frame update
    void Start()
    {
    	float scaleX = transform.localScale.x;
    	float scaleY = transform.localScale.y;
    	if (horizontalMotion)
        transform.DOScaleX(scaleX*(1+amountHorizontal), 1/frequency).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        if (verticalMotion)
        transform.DOScaleY(scaleY*(1+amountVertical), 1/frequency).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
    }
}
