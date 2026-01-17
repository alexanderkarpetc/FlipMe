using System;
using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Views
{
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        public event Action<SpriteData> OnClicked;

        [Header("UI")]
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _backSprite;

        [Header("Flip Animation")]
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _flipDuration = 0.3f;

        public bool IsRevealed { get; private set; }

        private bool IsAnimating => _flipRoutine != null;

        private SpriteData _data;
        private Coroutine _flipRoutine;

        public void Init(SpriteData spriteData)
        {
            _data = spriteData;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsRevealed) return;
            if (IsAnimating) return;

            Reveal();
            OnClicked?.Invoke(_data);
        }

        private void Reveal()
        {
            if (IsAnimating) return;

            IsRevealed = true;
            StartFlip(toFront: true);
        }

        private void StartFlip(bool toFront)
        {
            StopFlipIfAny();
            _flipRoutine = StartCoroutine(FlipRoutine(toFront));
        }

        private void StopFlipIfAny()
        {
            if (_flipRoutine != null)
            {
                StopCoroutine(_flipRoutine);
                _flipRoutine = null;
            }
        }

        private IEnumerator FlipRoutine(bool toFront)
        {
            var half = _flipDuration * 0.5f;

            yield return ScaleX(1f, 0f, half);

            // swap sprite
            _image.sprite = toFront ? _data.Sprite : _backSprite;

            yield return ScaleX(0f, 1f, half);

            _rect.localScale = Vector3.one;
            _flipRoutine = null;
        }

        private IEnumerator ScaleX(float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                var s = _rect.localScale;
                s.x = to;
                _rect.localScale = s;
                yield break;
            }

            var time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                var normalizedTime = Mathf.Clamp01(time / duration);
                var step = Mathf.SmoothStep(from, to, normalizedTime);

                var localScale = _rect.localScale;
                localScale.x = step;
                _rect.localScale = localScale;

                yield return null;
            }
        }
    }
}
