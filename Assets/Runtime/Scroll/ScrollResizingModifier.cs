using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ScrollResizingModifier : BaseScrollModifier
    {
        protected float MinObjectSize { get; private set; } = 0.6f;
        protected float MaxObjectSize { get; private set; } = 1.0f;
        
        private Tween _scaleTween;

        protected override void OnUpdateContentPosition()
        {
            UpdateScrollObjectsScale();
        }

        public void SetObjectSize(float minSize, float maxSize)
        {
            MinObjectSize = minSize;
            MaxObjectSize = maxSize;
        }

        protected override void OnInitChilds()
        {
            foreach (var child in _childObjects)
            {
                child.localScale = GetCurrentScaleByPosition(child.position);
            }
        }

        private Vector3 GetCurrentScaleByPosition(Vector2 position)
        {
            float delta = Mathf.Abs(_scrollDirection.GetCurrentVector(position, _scrollType));
            float scaleValue;
            
            if (delta <= 0f)
            {
                scaleValue = MaxObjectSize;
            }
            else
            {
                scaleValue = Mathf.Clamp(MaxObjectSize - ((delta / _distanceBetweenObjects) * MinObjectSize),
                    MinObjectSize, MaxObjectSize);
            }

            return new Vector3(scaleValue, scaleValue, scaleValue);
        }
        
        protected virtual void UpdateScrollObjectsScale()
        {
            foreach (var child in _childObjects)
            {
                child.localScale = GetCurrentScaleByPosition(child.position);
            }
        }
    }
}