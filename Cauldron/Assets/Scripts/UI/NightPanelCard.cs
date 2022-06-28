using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CauldronCodebase
{ 
    
    public class NightPanelCard : MonoBehaviour
    {
        [SerializeField] private Image cardImage;
        [SerializeField] private float enterTime = 1f, exitTime = 1f, moveTime = 0.5f;
        [SerializeField] private float offscreenLeft = -100, offScreenRight = 2000;
        [SerializeField] private float enterRotation = -10, exitRotation = 10;
        [SerializeField] private Ease enterMotion, exitMotion, moveMotion = Ease.Linear;
        [SerializeField] private float exitStartTime = 0.2f, exitStartDist = 10;
        
        private Vector3 initialPosition;
        public event Action<NightPanelCard> InPlace;
        private Image[] imagesToFade;
        
        public void Init(Sprite picture, Vector3 initialPosition)
        {
            if (picture != null)
            {
                cardImage.sprite = picture;
            }
            this.initialPosition = initialPosition;
            imagesToFade = GetComponentsInChildren<Image>();
        }

        public void Enter(Vector3 point, float zRotation)
        {
            gameObject.SetActive(true);
            foreach (var image in imagesToFade)
            {
                image.color = Color.white;
            }
            transform.DOLocalMove(point, enterTime).
                From(new Vector3(offscreenLeft, 0, 0)).
                SetEase(enterMotion).
                OnComplete(() => InPlace?.Invoke(this));
            transform.DORotate(new Vector3(0, 0, zRotation), enterTime).
                From(new Vector3(0, 0, enterRotation));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Exit()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMoveX(initialPosition.x - exitStartDist, exitStartTime));
            sequence.Append(transform.DOLocalMoveX(offScreenRight, exitTime).SetEase(exitMotion));
            sequence.Join(transform.DORotate(new Vector3(0, 0, exitRotation), exitTime));
            foreach (var image in imagesToFade)
            {
                sequence.Join(image.DOFade(0, exitTime));
            }
        }

        public void Move(Vector3 point, float zRotation)
        {
            transform.DOLocalMove(point, moveTime).SetEase(moveMotion);
            transform.DORotate(new Vector3(0, 0, zRotation), moveTime);
        }
    }
}