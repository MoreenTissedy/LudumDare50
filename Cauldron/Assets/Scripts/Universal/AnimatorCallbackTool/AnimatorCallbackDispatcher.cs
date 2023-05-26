using Client.Common.AnimatorCallbackTool;
using UnityEngine;

namespace Client.Common.AnimatorTools
{
    public class AnimatorCallbackDispatcher : StateMachineBehaviour
    {
        public enum EventType
        {
            OnEnter,
            OnExit
        }
        
        [SerializeField] private EventType _eventType;
        
        [Tooltip("Optional key to identify the callback, you can also use state name or tag.")]
        [SerializeField] private string _key;

        [Tooltip("If true, will search the first receiver up the hierarchy, otherwise will check only the animator gameobject.")]
        [SerializeField] private bool _lookUpHierarchy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_eventType is EventType.OnEnter)
            {
                SendCallback(animator, stateInfo.tagHash, stateInfo.shortNameHash);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_eventType is EventType.OnExit)
            {
                SendCallback(animator, stateInfo.tagHash, stateInfo.shortNameHash);
            }
        }

        private void SendCallback(Animator animator, int tagHash, int nameHash)
        {
            AnimatorCallbackInfo info = new AnimatorCallbackInfo ()
            {
                TagHash = tagHash,
                NameHash = nameHash,
                Key = _key,
                Dispatcher = animator,
                Type = _eventType
            };
            
            if (animator.TryGetComponent(out IAnimatorCallbackReceiver receiver))
            {
                receiver.OnAnimationCallback(info);
            }
            if (animator.TryGetComponent(out AnimatorCallbackDispatcherMediator mediator))
            {
                mediator.OnAnimationCallback(info);
            }
            if (_lookUpHierarchy && receiver is null)
            {
                receiver = animator.GetComponentInParent<IAnimatorCallbackReceiver>();
                receiver?.OnAnimationCallback(info);
            }
        }
        
    }
}