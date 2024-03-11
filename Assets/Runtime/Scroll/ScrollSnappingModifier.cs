using System;
using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ScrollSnappingModifier : BaseScrollModifier
    {
        protected Tween _moveTween;
        protected float _clampSpeedValue;
        protected Transform _rootTransform;
        
        private Ease _animationEase;

        public void OnStartScroll()
        {
            _moveTween?.Kill(); // stop snap moving
        }

        public void SetSnappingModifierParameters(Transform rootTransform, float clampValue, Ease ease)
        {
            _rootTransform = rootTransform;
            _clampSpeedValue = clampValue;
            _animationEase = ease;
        }

        public void SnappingContent(int id, Action onUpdate = null) // Snap root transform to current id of object
        {
            MoveToTargetById(id, _animationEase, onUpdate);
        }

        protected virtual void MoveToTargetById(int id, Ease ease = Ease.Linear, Action onUpdate = null)
        {
            _moveTween?.Kill();

            Vector2 target = _rootTransform.localPosition - _childObjects[id].position;
            float delay = Mathf.Abs(_scrollDirection.GetCurrentVector(_childObjects[id].position, _scrollType)) * _clampSpeedValue;

            _moveTween = onUpdate == null
                ? _rootTransform.DOMove(target, delay).SetEase(Ease.Linear)
                : _rootTransform.DOMove(target, delay).SetEase(Ease.Linear)
                    .OnUpdate(onUpdate.Invoke).OnComplete(onUpdate.Invoke);
        }
    }
}