using System.Collections;
using UnityEngine;

namespace CiaoGames.RewardCenter
{
    /// <summary>
    /// Simple coroutine-based tween utility with no external dependencies.
    /// Attach to any GameObject that needs animation. Public API so studios can
    /// drive their own tweens on it if they want.
    /// </summary>
    public class CiaoTweener : MonoBehaviour
    {
        private Coroutine _scaleRoutine;
        private Coroutine _moveRoutine;
        private Coroutine _punchRoutine;
        private Coroutine _bounceRoutine;
        private Coroutine _shakeRoutine;

        /// <summary>Scale from current localScale to <paramref name="to"/> over <paramref name="duration"/> using OutBack easing.</summary>
        public Coroutine Scale(Vector3 to, float duration)
        {
            if (_scaleRoutine != null) StopCoroutine(_scaleRoutine);
            _scaleRoutine = StartCoroutine(ScaleRoutine(to, duration));
            return _scaleRoutine;
        }

        /// <summary>Move from current localPosition to <paramref name="to"/> over <paramref name="duration"/> using OutBack easing.</summary>
        public Coroutine MoveTo(Vector3 to, float duration)
        {
            if (_moveRoutine != null) StopCoroutine(_moveRoutine);
            _moveRoutine = StartCoroutine(MoveRoutine(to, duration));
            return _moveRoutine;
        }

        /// <summary>Peak-and-return the current localScale by <paramref name="punch"/> over <paramref name="duration"/>.</summary>
        public Coroutine PunchScale(Vector3 punch, float duration)
        {
            if (_punchRoutine != null) StopCoroutine(_punchRoutine);
            _punchRoutine = StartCoroutine(PunchScaleRoutine(punch, duration));
            return _punchRoutine;
        }
        
        public Coroutine Shake(float amount, float duration)
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(ShakeRoutine(amount, duration));
            return _shakeRoutine;
        }

        private IEnumerator ShakeRoutine(float amount, float duration)
        {
            var basePos = transform.localPosition;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                var damper = 1f - (elapsed / duration);          // fades intensity toward end
                var offsetX = (Random.value * 2f - 1f) * amount * damper;
                var offsetY = (Random.value * 2f - 1f) * amount * damper;
                transform.localPosition = basePos + new Vector3(offsetX, offsetY, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = basePos;
        }

        /// <summary>Endless vertical bounce around the current localPosition. Call <see cref="StopBounce"/> to end.</summary>
        public void StartBounce(float amplitude, float period)
        {
            StopBounce();
            _bounceRoutine = StartCoroutine(BounceRoutine(amplitude, period));
        }

        public void StopBounce()
        {
            if (_bounceRoutine == null) return;
            StopCoroutine(_bounceRoutine);
            _bounceRoutine = null;
        }

        private IEnumerator ScaleRoutine(Vector3 to, float duration)
        {
            var from = transform.localScale;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                var t = elapsed / duration;
                transform.localScale = Vector3.LerpUnclamped(from, to, EaseOutBack(t));
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = to;
        }

        private IEnumerator MoveRoutine(Vector3 to, float duration)
        {
            var from = transform.localPosition;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                var t = elapsed / duration;
                transform.localPosition = Vector3.LerpUnclamped(from, to, EaseOutBack(t));
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = to;
        }

        private IEnumerator PunchScaleRoutine(Vector3 punch, float duration)
        {
            var baseScale = transform.localScale;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                var t = elapsed / duration;
                var factor = Mathf.Sin(t * Mathf.PI);
                transform.localScale = baseScale + punch * factor;
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = baseScale;
        }

        private IEnumerator BounceRoutine(float amplitude, float period)
        {
            var basePos = transform.localPosition;
            var elapsed = 0f;
            while (true)
            {
                var offset = Mathf.Sin((elapsed / period) * Mathf.PI * 2f) * amplitude;
                transform.localPosition = new Vector3(basePos.x, basePos.y + offset, basePos.z);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            var p = t - 1f;
            return 1f + c3 * p * p * p + c1 * p * p;
        }
    }
}