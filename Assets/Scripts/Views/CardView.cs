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
        public event Action<CardView> OnClicked;

        [Header("UI")]
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _backSprite;

        [Header("Flip Animation")]
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _flipDuration = 0.3f;

        
        private CardData _data;
        private Coroutine _flipRoutine;

        public bool IsRevealed { get; private set; }
        public bool IsAnimating => _flipRoutine != null;
        public bool IsLocked { get; private set; }
        public float FlipDuration => _flipDuration;
        public CardData Data => _data;

        public void Init(CardData cardData)
        {
            _data = cardData;

            if (_rect == null) _rect = (RectTransform)transform;
            IsLocked = false;
            IsRevealed = false;
            _image.sprite = _backSprite;
            _rect.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsLocked || IsRevealed || IsAnimating) return;

            Reveal();
            OnClicked?.Invoke(this);
        }

        public void Hide()
        {
            StopAllCoroutines();
            IsRevealed = false;
            StartFlip(toFront: false);
        }

        public void SetIsLocked(bool value) => IsLocked = value;

        public void Complete()
        {
            _image.gameObject.SetActive(false);
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
