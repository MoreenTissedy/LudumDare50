using UnityEngine;

namespace CauldronCodebase
{
    public class TutorialEntrySMB : StateMachineBehaviour
    {
        [SerializeField][TextArea(10,10)] protected string text;
        protected CatTutorial catTutorial;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            catTutorial = animator.gameObject.GetComponent<CatTutorial>();
            catTutorial.Show(text);
        }
    }
}