using UnityEngine;

namespace CHG._02.Script.Agents
{
    public class AnimationEffectPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] effects;

        public void PlayEffect(int index)
        {
            if (index >= 0 && index < effects.Length) 
                effects[index].Play(true);
        }

        public void StopEffect(int index)
        {
            if (index >= 0 && index < effects.Length)
                effects[index].Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        public void StopAll()
        {
            foreach (ParticleSystem effect in effects)
                
                
                effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}