using System;
using CauldronCodebase;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartGameFX : MonoBehaviour
{
    public SoundManager soundManager;

    public async void PlaySound()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f));
        soundManager.Play(Sounds.GameStart);
        await UniTask.Delay(TimeSpan.FromSeconds(5f));
        Destroy();
    }

    public void Destroy()
    {
        Destroy(transform.root.gameObject);
    }
}
