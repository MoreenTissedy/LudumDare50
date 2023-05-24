using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CauldronCodebase
{
    public abstract class BaseFX : MonoBehaviour
    {
        protected SoundManager Sound;

        public void Init(SoundManager soundManager)
        {
            Sound = soundManager;
        }
        public abstract UniTask Play();
    }
}