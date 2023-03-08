using UnityEngine;

namespace Framework.Utils
{
    public class AnimationCurveHelper
    {
        private readonly AnimationCurve _animationCurve;
        private float _time;

        public AnimationCurveHelper(AnimationCurve animationCurve)
        {
            _animationCurve = animationCurve;
        }

        public void Reset()
        {
            _time = 0;
        }

        public float GetValue()
        {
            return _animationCurve.Evaluate(_time);
        }

        public void IncreaseUpdateTime()
        {
            _time += Time.deltaTime;
        }
        
        public void IncreaseFixedTime()
        {
            _time += Time.fixedDeltaTime;
        }
    }
}