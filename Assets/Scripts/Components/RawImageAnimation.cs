using UnityEngine;
using UnityEngine.UI;

namespace UI.Components
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageAnimation : MonoBehaviour
    {
        /// <summary>
        /// Speed change UV per second
        /// </summary>
        [SerializeField] private Vector2 _speed = new Vector2(0.025f, 0.025f);
        [SerializeField] private RawImage _image;

        private void Update()
        {
            var rect = _image.uvRect;
            rect.x += _speed.x * Time.deltaTime;
            rect.x = Normalized(rect.x);
            rect.y += _speed.y * Time.deltaTime;
            rect.y = Normalized(rect.y);
            _image.uvRect = rect;
        }

        private float Normalized(float value)
        {
            if (value < 0.0f)
                return value + 1.0f;
            else if (value > 1.0f)
                return value - 1.0f;
            else
                return value;
        }
    }
}
