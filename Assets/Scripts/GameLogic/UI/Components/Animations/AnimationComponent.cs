using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameLogic.UI.Components
{
    public class AnimationComponent : MonoBehaviour
    {
        [ContextMenu("Animate")]
        public virtual UniTask  Animate()
        {
            return UniTask.CompletedTask;
        }
    }
}