using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

public class ParticleFadeController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particles;

    private bool isShown;

    private void Awake()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
    }
    
    public async UniTaskVoid Show()
    {
        if (isShown) return;
        isShown = true;
        
        foreach (var particle in particles)
        {
            var particleColorOverLifetime = particle.colorOverLifetime;
            particleColorOverLifetime.enabled = true;
            particle.Play();
        }

        await UniTask.Delay(500);
        
        foreach (var particle in particles)
        {
            var particleColorOverLifetime = particle.colorOverLifetime;
            particleColorOverLifetime.enabled = false;
        }
    }
    
    public void Hide()
    {
        if (!isShown) return;
        isShown = false;
        
        foreach (var particle in particles)
        {
            particle.time = particle.main.startLifetime.constant;
            var particleColorOverLifetime = particle.colorOverLifetime;
            particleColorOverLifetime.enabled = true;
            particle.Stop();
        }
    }
}
