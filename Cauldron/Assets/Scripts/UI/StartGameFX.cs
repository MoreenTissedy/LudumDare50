using CauldronCodebase;
using UnityEngine;

public class StartGameFX : MonoBehaviour
{
    public SoundManager SoundManager;

    public void PlaySound()
    {
        SoundManager.Play(Sounds.GameStart);
    }

    public void Destroy()
    {
        Destroy(transform.root.gameObject);
    }
}
