using UnityEngine;

namespace CauldronCodebase
{
    public class EndTutorialSMB : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.GetComponent<CatTutorial>().End();
        }
    }
}