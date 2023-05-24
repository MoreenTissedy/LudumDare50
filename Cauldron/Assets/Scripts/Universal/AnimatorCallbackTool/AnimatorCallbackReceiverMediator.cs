using System.Collections.Generic;
using Client.Common.AnimatorCallbackTool;
using UnityEngine;

namespace Client.Common.AnimatorTools
{
    public class AnimatorCallbackReceiverMediator: MonoBehaviour
    {
        [SerializeField] private List<AnimatorCallbackDispatcherMediator> _dispatchers = new(1);
        
        private IAnimatorCallbackReceiver _target;
        
        public void AddDispatcher(Animator animator)
        {
            if (!animator.TryGetComponent(out AnimatorCallbackDispatcherMediator dispatcher))
            {
                dispatcher = animator.gameObject.AddComponent<AnimatorCallbackDispatcherMediator>();
            }

            if (!_dispatchers.Contains(dispatcher))
            {
                _dispatchers.Add(dispatcher);
                dispatcher.Register(this);
            }
        }

        public void ClearDispatchers()
        {
            foreach (AnimatorCallbackDispatcherMediator dispatcher in _dispatchers)
            {
                dispatcher.Unregister(this);
            }
            _dispatchers.Clear();
        }

        private void Awake()
        {
            _target = GetComponent<IAnimatorCallbackReceiver>();
        }

        public void OnAnimationCallback(AnimatorCallbackInfo info)
        {
            _target.OnAnimationCallback(info);
        }
    }
}