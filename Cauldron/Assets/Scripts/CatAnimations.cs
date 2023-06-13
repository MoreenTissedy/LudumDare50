using System.Collections;
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
        [FormerlySerializedAs("idleAnimations")] [SpineAnimation()] public string[] randomActions;
        [SpineAnimation()] public string strokeAnimation;
        [SpineAnimation()] public string idleAnimation = "idle";
        [Tooltip("Interval in minutes")]
        public float minIdleInterval = 0.5f;
        public float maxIdleInterval = 1f;

        private Vector2 startDragPosition;

        [Inject] private SoundManager soundManager;

        private void Start()
        {
            StartCoroutine(RandomActionsRoutine());
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
            startDragPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            //needed to implement drag
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            soundManager.PlayCat(CatSound.Purr);
            PlayAnimationOneShot(strokeAnimation);
        }

        private void PlayAnimationOneShot(string animation)
        {
            catSkeleton.AnimationState.SetAnimation(0, animation, false);
            catSkeleton.AnimationState.AddAnimation(0, idleAnimation, true, 0f);
        }
    }
}