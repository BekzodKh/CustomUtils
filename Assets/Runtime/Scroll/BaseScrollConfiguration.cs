using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class BaseScrollConfiguration : MonoBehaviour
    {
        [SerializeField] private ScrollType _scrollType; // set the type of scroll (Horizontal or Vertical)

        [Header("Main Parameters")]
        [SerializeField, Range(0.0f, 4.0f)] private float _clampSpeedValue; // multiplies the inertia value after scroll
        [SerializeField, Range(0.0f, 4.0f)] private float _clampDragValue; // multiplies the scroll distance
        
        [Header("Inertia Parameters")]
        [SerializeField, Range(0.0f, 4.0f)] private float _clampInertiaDelayValue; // multiplies the scroll inertia delay
        [SerializeField, Range(0.0f, 4.0f)] private float _inertiaRestriction;
        
        /// <summary>
        /// Count of Objects visible from left and right, counting from the current object
        /// </summary>
        [Space] 
        [SerializeField] private int _countObjectsCreatedBeforeCurrent = 3;
        [SerializeField] private float _hideObjectBorderPosition = 10f;
        [SerializeField] private float _distanceObjectsValue = 5f;

        [Space]
        public bool isScrollInfinite; // if true init Infinitive scroll
        
        [Space]
        public bool isScrollSnapping; // if true init Snapping scroll
        [SerializeField] private Ease _animationCurveEase;
        [SerializeField, Range(0.0f, 1.0f)] private float _snapAnimationDurationClampValue;
        
        [Space]
        public bool isScrollShouldResizing; // if true init Resizing scroll
        [SerializeField] private float _objectMinSizeValue;
        [SerializeField] private float _objectMaxSizeValue;

        [SerializeField]
        private bool _selfInitialize = true;

        public float ClampSpeedValue => _clampSpeedValue;
        
        public float ClampDragValue => _clampDragValue;
        
        public float ClampInertiaDelayValue => _clampInertiaDelayValue;

        public int CountObjectsCreatedBeforeCurrent => _countObjectsCreatedBeforeCurrent;

        public float HideObjectBorderPosition => _hideObjectBorderPosition;

        public float InertiaRestriction => _inertiaRestriction;
        
        public ScrollType ScrollType => _scrollType;

        public float DistanceObjectsValue
        {
            get => _distanceObjectsValue;
            set
            {
                if (value > 0)
                {
                    _distanceObjectsValue = value;
                }
            }
        }

        public Ease AnimationCurveEase => _animationCurveEase;

        public float SnapAnimationDurationClampValue => _snapAnimationDurationClampValue;

        public float ObjectMinSizeValue => _objectMinSizeValue;

        public float ObjectMaxSizeValue => _objectMaxSizeValue;
        public bool SelfInitialize => _selfInitialize;
    }
}