using System;
using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll.RoundScroll
{
    public class RoundScrollSnappingModifier : ScrollSnappingModifier
    {
        protected override void MoveToTargetById(int id, Ease ease = Ease.Linear, Action onUpdate = null)
        {
            _moveTween?.Kill();

            var rotation = _rootTransform.localEulerAngles - _childObjects[id].eulerAngles;
            float delay = Mathf.Abs(_scrollDirection.ClampAngle(_childObjects[id].eulerAngles.z) * _clampSpeedValue);

            _moveTween = onUpdate == null ? _rootTransform.DORotate(rotation, delay).SetEase(ease) : 
                _rootTransform.DORotate(rotation, delay).SetEase(ease)
                    .OnUpdate(onUpdate.Invoke).OnComplete(onUpdate.Invoke);
        }
    }
}