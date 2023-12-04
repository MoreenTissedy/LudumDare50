using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

namespace CauldronCodebase
{ 
    
    public class NightPanelCard : MonoBehaviour
    {
        [SerializeField] private Image cardImage;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private float offscreenLeft = -100, offScreenRight = 2000;

        [Header("Animation settings")] [SerializeField] [Tooltip("Время анимаций в секундах")]
        private float enterTime = 1f; 
        [SerializeField] private float exitTime = 1f, moveTime = 0.5f;
        [SerializeField][Tooltip("Поворот в градусах")] private float enterRotation = -10, exitRotation = 10;
        [SerializeField][Tooltip("Характер движения")] private Ease enterMotion, exitMotion, moveMotion = Ease.Linear;
        [SerializeField][Tooltip("Начальная оттяжка при сбросе карты")] private float exitStartTime = 0.2f, exitStartDist = 10;
        [SerializeField] [Tooltip("Задержка исчезания карты при сбросе")] private float fadeDelay = 0.5f;
        [SerializeField] [Tooltip("Параметры толчка карты другой картой при раздаче")]
        private float punchAngle = -5, punchStrength = -15, punchTime = 0.5f, punchDelay = 0.8f;
        [SerializeField]
        private int punchVibrato = 3;
        
        private Vector3 initialPosition;
        public event Action<NightPanelCard> InPlace;
        private Image[] imagesToFade;

        private SoundManager soundManager;
        
        public void Init(Sprite picture, Vector3 initialPosition, SoundManager soundManager)
        {
            if (picture != null)
            {
                cardImage.sprite = picture;
            }
            else
            {
                cardImage.sprite = defaultSprite;
            }

            this.soundManager = soundManager;
            this.initialPosition = initialPosition;
            imagesToFade = GetComponentsInChildren<Image>();
            transform.SetAsLastSibling();
        }

        public void Enter(Vector3 point, float zRotation)
        {
            gameObject.SetActive(true);
            soundManager.Play(Sounds.NightCardEnter);
            foreach (var image in imagesToFade)
            {
                image.color = Color.white;
            }

            transform.DOLocalMove(point, enterTime).From(new Vector3(offscreenLeft, 0, 0)).SetEase(enterMotion)
                .OnComplete(() => InPlace?.Invoke(this));
            transform.DORotate(new Vector3(0, 0, zRotation), enterTime).
                From(new Vector3(0, 0, enterRotation));
        }

        public void Punch()
        {
            DOTween.Sequence().AppendInterval(enterTime*punchDelay)
                .Append(transform.DOPunchPosition(new Vector3(punchStrength, 0, 0), punchTime, punchVibrato))
                .Join(transform.DOPunchRotation(new Vector3(0, 0, punchAngle), punchTime, punchVibrato));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Exit()
        {
            soundManager.Play(Sounds.NightCardExit);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMoveX(initialPosition.x - exitStartDist, exitStartTime));
            sequence.Append(transform.DOLocalMoveX(offScreenRight, exitTime).SetEase(exitMotion));
            sequence.Join(transform.DORotate(new Vector3(0, 0, exitRotation), exitTime));
            foreach (var image in imagesToFade)
            {
                sequence.Insert(exitStartTime+exitTime*fadeDelay, image.DOFade(0, exitTime));
            }
        }

        public void Move(Vector3 point, float zRotation)
        {
            transform.DOLocalMove(point, moveTime).SetEase(moveMotion);
            transform.DORotate(new Vector3(0, 0, zRotation), moveTime);
        }
    }
}