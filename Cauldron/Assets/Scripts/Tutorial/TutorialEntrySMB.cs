using UnityEngine;

namespace CauldronCodebase
{
    public class TutorialEntrySMB : StateMachineBehaviour
    {
        [SerializeField][TextArea(10,10)] private string text;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.gameObject.GetComponent<CatTutorial>().Show(text);
        }
    }
}