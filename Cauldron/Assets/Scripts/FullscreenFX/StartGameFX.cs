using System;
using Cysharp.Threading.Tasks;

namespace CauldronCodebase
{
    public class StartGameFX : BaseFX
    {
        public event Action OnEnd;

        public void Destroy()
        {
            OnEnd?.Invoke();
            OnEnd = null;
            Destroy(transform.root.gameObject);
        }

        public override UniTask Play()
        {
            Sound.Play(Sounds.GameStart);
            return UniTask.CompletedTask;
        }
    }
}
