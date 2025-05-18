using UnityEngine;
using DG.Tweening;

namespace Universal
{
    [RequireComponent(typeof(RectTransform))]
    public class GrowAnimation : AnimatedButtonComponent
    {
        public float sizeCoef = 1.2f;
        public float sizeSpeed = 0.2f;
        
        private Vector3 initialScale;
        private RectTransform transf;

        private void Awake()
        {
            transf = GetComponent<RectTransform>();
            initialScale = transf.localScale;
        }

        public override void Select()
        {
            transf.DOScale(initialScale * sizeCoef, sizeSpeed).SetUpdate(true);
        }

        public override void Unselect()
        {
            transf.DOScale(initialScale, sizeSpeed).SetUpdate(true);
        }

        public override void Activate()
        {
            if (transf == null)
            {
                transf = GetComponent<RectTransform>();
            }
            transf.DOKill();
            transf.localScale = initialScale;
        }

        public override void ChangeInteractive(bool isInteractive){}
    }
}