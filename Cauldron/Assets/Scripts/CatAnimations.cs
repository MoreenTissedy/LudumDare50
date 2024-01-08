using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace CauldronCodebase
{
    public class CatAnimations : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private SkeletonAnimation catSkeleton;
        [FormerlySerializedAs("idleAnimations")] [SpineAnimation] public string[] randomActions;
        [SpineAnimation] public string strokeAnimation;
        [SpineAnimation] public string dragAnimation;
        [SpineAnimation] public string moveAnimation;
        [SpineAnimation] public string idleAnimation = "idle";
        [Tooltip("Interval in minutes")]
        public float minIdleInterval = 0.5f;
        public float maxIdleInterval = 1f;

        [Header("Drag")]
        [SerializeField] private Vector2 dragAnimationOffset;
        [SerializeField] private float dragThreshold;
        public bool IsDragged { get; private set; }
        private bool dragAnimated;
        private Vector2 startDragPosition;

        [Header("Movement")]
        [SerializeField] private CatPath catPath;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float catDropSpeed;
        private Vector3 startPosition;
        private float strokeAnimationDuration;


        private Tween moveTween;
        private Tween dropTween;
        
        private Coroutine randomAction;
        private Coroutine catLanding;

        [Inject] private SoundManager soundManager;

        private void Start()
        {
            startPosition = transform.position;
            strokeAnimationDuration = catSkeleton.skeleton.Data.FindAnimation(strokeAnimation).Duration;
            
            randomAction = StartCoroutine(RandomActionsRoutine());
        }

        private void DropAnimation()
        {
            var closestWaypoint = catPath.FindClosestWaypoint(transform.position.x, true).position;
            
            var moveDirection = (closestWaypoint - transform.position).normalized;
            if (Vector2.Dot(Vector2.left, moveDirection) < 0)
            {
                catSkeleton.skeleton.ScaleX = -1;
            }
            else
            {
                catSkeleton.skeleton.ScaleX = 1;
            }

            dropTween = transform.DOMoveY(closestWaypoint.y, catDropSpeed)
                                .SetSpeedBased(true)
                                .SetEase(Ease.InSine)
                                .OnComplete(() => catLanding = StartCoroutine(CatLanding()));
        }

        private IEnumerator CatLanding()
        {
            catSkeleton.AnimationState.SetAnimation(0, strokeAnimation, false);

            yield return new WaitForSeconds(strokeAnimationDuration);
            
            Move(true);
        }

        private void Move(bool toStartSpot)
        {
            var path = catPath.GetPath(transform, toStartSpot);
            
            catSkeleton.AnimationState.SetAnimation(0, moveAnimation, true);

            moveTween = transform.DOPath(path, movementSpeed, PathType.CatmullRom)
                                .SetSpeedBased(true)
                                .SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                    catSkeleton.skeleton.ScaleX = 1;
                                    catSkeleton.AnimationState.SetAnimation(0, idleAnimation, true);
                                    randomAction = StartCoroutine(RandomActionsRoutine());
                                });
        }

        private IEnumerator RandomActionsRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minIdleInterval, maxIdleInterval) * 60);
                PlayRandomAction();
            }
        }

        private void PlayRandomAction()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            soundManager.PlayCat(CatSound.Annoyed);
            PlayAnimationOneShot(randomActions[Random.Range(0, randomActions.Length)]);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(randomAction != null)
                StopCoroutine(randomAction);
            if(catLanding != null)
                StopCoroutine(catLanding);
            
            dropTween?.Kill();
            moveTween?.Kill();

            startDragPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            CheckDragBegin(eventData.position);
            
            if (IsDragged)
            {
                var targetPosition = Camera.main.ScreenToWorldPoint((Vector3)eventData.position + Vector3.forward * Camera.main.nearClipPlane) 
                                            + (Vector3)dragAnimationOffset;
                transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
                EnableDragAnimation();
            }
        }

        private void CheckDragBegin(Vector2 pos)
        {
            if(IsDragged) return;
            
            IsDragged = (startDragPosition - pos).magnitude > dragThreshold;
        }

        private void EnableDragAnimation()
        {
            if(dragAnimated) return;

            dragAnimated = true;
            catSkeleton.AnimationState.SetAnimation(0, dragAnimation, true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsDragged)
            {
                IsDragged = false;
                dragAnimated = false;
                DropAnimation();
            }
            else
            {
                soundManager.PlayCat(CatSound.Annoyed);
                PlayAnimationOneShot(strokeAnimation);
            }
        }

        private void PlayAnimationOneShot(string animation)
        {
            catSkeleton.AnimationState.SetAnimation(0, animation, false);
            catSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }
    }
}