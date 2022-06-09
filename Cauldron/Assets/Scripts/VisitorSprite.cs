using UnityEngine;

namespace CauldronCodebase
{
    public class VisitorSprite: Visitor
    {
        private SpriteRenderer rendSprite;
        protected override void Awake()
        {
            rendSprite = GetComponent<SpriteRenderer>();
            rendSprite.enabled = false;
        }

        public override void Enter()
        {
            rendSprite.enabled = true;
        }

        public override void Exit()
        {
            rendSprite.enabled = false;
        }
    }
}