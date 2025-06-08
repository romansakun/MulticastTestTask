using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameLogic.UI.Components
{
    public class AnimationSequenceComponent : AnimationComponent
    {
        [SerializeField] private bool _joinAllAnimations;
        [SerializeField] private List<AnimationComponent> _animatedComponents;

        public override async UniTask Animate()
        {
            if (_joinAllAnimations)
            {
                await UniTask.WhenAll(_animatedComponents.Select(x => x.Animate())); 
            }
            else
            {
                foreach (var animation in _animatedComponents)
                {
                    await animation.Animate();
                }
            }
        }
    }
}