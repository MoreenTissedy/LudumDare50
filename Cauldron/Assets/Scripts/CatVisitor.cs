using UnityEngine;
namespace CauldronCodebase
{
    [RequireComponent(typeof(CatAnimations))]
    public class CatVisitor : Visitor
    {
        [field: SerializeField] public float WalkingTime { get; private set; }
        private CatAnimations catAnimations;
        protected override void Awake()
        {
            catAnimations = GetComponent<CatAnimations>();
        }

        public override void Enter()
        {
            catAnimations.SetInteractable(false);
            catAnimations.Move(false);
        }

        public override void ExitWithDestroy()
        {
            catAnimations.Move(true);
            catAnimations.SetInteractable(true);
        }
    }
}
