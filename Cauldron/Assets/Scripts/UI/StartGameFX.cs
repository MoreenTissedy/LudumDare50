using CauldronCodebase;
using UnityEngine;
using Zenject;

public class StartGameFX : MonoBehaviour
{
    public SoundManager soundManager;

    public void PlaySound()
    {
        soundManager.Play(Sounds.GameStart);
    }

    public void Destroy()
    {
        Destroy(transform.root.gameObject);
    }
}
