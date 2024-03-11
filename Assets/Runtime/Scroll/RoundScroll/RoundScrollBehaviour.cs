using System;
using CustomUtils.Utils.Scroll;
using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll.RoundScroll
{
    public class RoundScrollBehaviour : BaseScrollBehaviour, IPauseScrollBehaviour
    {
        protected float _contentBeginDragRotateValue = 0f;
        protected float _startPressPositionValue;
        protected float _pressPositionValue;

        protected RoundScrollSnappingModifier _roundScrollSnappingModifier;

        protected override bool HasSnapModifier()
        {
            if (_scrollConfiguration.isScrollSnapping)
            {
                ConfigureSnappingScroll();
                return true;
            }
            else
            {
                DestroyModifier(ref _scrollSnappingModifier, () => _roundScrollSnappingModifier = null);
                return false;
            }
        }
        
        protected override bool HasInfiniteModifier()
        {
            if (_scrollConfiguration.isScrollInfinite)
            {
                ConfigureInfiniteScroll();
                return true;
            }
            else
            {
                DestroyModifier(ref _scrollInfiniteModifier, CreateBorder);
                return false;
            }
        }

        protected override void ConfigureInfiniteScroll()
        {
            if(_scrollInfiniteModifier != null || !_scrollConfiguration.isScrollInfinite) return;
            
            // init infinite scroll
            _scrollInfiniteModifier = new RoundScrollInfiniteModifier();
            SetScrollBehaviour(_scrollInfiniteModifier);
        }

        protected override void ConfigureSnappingScroll()
        {
            if(_scrollSnappingModifier != null || !_scrollConfiguration.isScrollSnapping) return;
            
            // init snapping scroll
            _scrollSnappingModifier = new RoundScrollSnappingModifier();
            _roundScrollSnappingModifier = (RoundScrollSnappingModifier)_scrollSnappingModifier;
            _roundScrollSnappingModifier.SetSnappingModifierParameters(_rootTransform, 
                _scrollConfiguration.SnapAnimationDurationClampValue, _scrollConfiguration.AnimationCurveEase);
            
            SetScrollBehaviour(_scrollSnappingModifier);
        }

        protected override void CreateBorder()
        {
            float rootTransformRotation = _scrollDirection.ClampAngle(_rootTransform.eulerAngles.z);
            
            _minBorder = _scrollDirection.GetObjectRotation((pos, val) => pos < val, 
                _childObjects) - rootTransformRotation;
            
            _maxBorder = _scrollDirection.GetObjectRotation((pos, val) => pos > val,
                _childObjects) - rootTransformRotation;
        }

        protected override void SetObjectStartPosition(Transform scrollObject, int multiplier)
        {
            float rotate = multiplier * _scrollConfiguration.DistanceObjectsValue;

            var rotation = scrollObject.eulerAngles;
            rotation.Set(0f,0f, rotate);

            scrollObject.eulerAngles = rotation;
        }

        public override void OnBeginScroll(Vector2 pressPosition)
        {
            HasInfiniteModifier();
            
            _moveTween?.Kill();
            _startPressPositionValue = pressPosition.x;
            _contentBeginDragRotateValue = _rootTransform.eulerAngles.z;
            
            _startDragTime = Time.time;
            
            for (int i = 0; i < _contentViews.Length; i++)
            {
                _contentViews[i].OnStartScroll();
            }
            
            if (HasSnapModifier())
            {
                _roundScrollSnappingModifier.OnStartScroll();
            }
        }

        public override void OnContinueScroll(Vector2 pressPosition)
        {
            _pressPositionValue = pressPosition.x;

            float rotation = _contentBeginDragRotateValue - (_pressPositionValue - _startPressPositionValue) * _scrollConfiguration.ClampDragValue;

            MoveContent(rotation);
            OnUpdateContentPosition();
        }

        private void MoveContent(float changedValue)
        {
            var currentRotation = _rootTransform.eulerAngles;
            
            currentRotation.Set(currentRotation.x, currentRotation.y, changedValue);

            SetContentRotation(currentRotation);
        }
        
        private void SetContentRotation(Vector3 rotation)
        {
            _rootTransform.eulerAngles = HasInfiniteModifier() ? rotation : ClampRotation(rotation);
        }

        protected override void OnInertialScroll()
        {
            float currentDragTime = _endDragTime - _startDragTime; // calculate Time of scroll
            float currentDragDistance = _pressPositionValue - _startPressPositionValue; // calculate Distance of scroll
            float speed = (-currentDragDistance / currentDragTime); // and calculate Speed of Scroll
            
            speed *= Time.unscaledDeltaTime;
            
            speed *= _scrollConfiguration.ClampSpeedValue;
            
            if (Mathf.Abs(speed) > _scrollConfiguration.InertiaRestriction)
            {
                speed = Mathf.Sign(speed) * _scrollConfiguration.InertiaRestriction;
            }
            
            float delay = Mathf.Abs(speed) * _scrollConfiguration.ClampInertiaDelayValue;

            _moveTween = DOTween.To(() => speed, x => speed = x, 0f, delay)
                .OnUpdate(() =>
                {
                    AddScrollSpeed(speed);
                    OnUpdateContentPosition();
                }).OnComplete(CompleteMove).SetEase(_scrollConfiguration.AnimationCurveEase);
        }

        protected void AddScrollSpeed(float speed)
        {
            var rotation = _rootTransform.eulerAngles;

            rotation.Set(rotation.x, rotation.y, rotation.z + speed);

            SetContentRotation(rotation);
        }

        private Vector3 ClampRotation(Vector3 rotation)
        {
            float zDelta = _scrollDirection.ClampAngle(rotation.z);
            
            rotation.Set(rotation.x, rotation.y, Mathf.Clamp(zDelta, -_maxBorder, -_minBorder));

            return rotation;
        }

        protected override void OnCompleteMove()
        {
            if (HasSnapModifier())
            {
                _roundScrollSnappingModifier.SnappingContent(CurrentIdOfObject, OnUpdateContentPosition);
            }
        }

        public override void ScrollContentToTargetById(int id, float duration, Ease ease = Ease.InOutSine, Action complete = null)
        {
            var rotation = _rootTransform.eulerAngles - _childObjects[id].eulerAngles;
            ScrollContentToTarget(rotation, duration, ease, complete);
        }

        public virtual void ScrollContentToTarget(float target, float duration, Ease ease = Ease.Linear, Action complete = null)
        {
            var rotation = new Vector3(0f,0f,target);
            ScrollContentToTarget(rotation, duration, ease, complete);
        }
        
        public override void ScrollContentToTarget(Vector3 target, float duration, Ease ease = Ease.Linear, Action complete = null)
        {
            _moveTween?.Kill();

            _moveTween = _rootTransform.DORotate(target, duration).OnStart(() =>
                {
                    for (int i = 0; i < _contentViews.Length; i++)
                    {
                        _contentViews[i].OnStartScroll();
                    }
                }).OnUpdate(OnUpdateContentPosition).SetEase(ease)
                .OnComplete(() =>
                {
                    CompleteMove();
                    complete?.Invoke();
                });
        }

        protected override void TurnOffNeedlessObjects() // turn off objects which out of showPosition
        {
            foreach (var currentObject in _childObjects)
            {
                float delta = _scrollDirection.ClampAngle(currentObject.eulerAngles.z);
                currentObject.gameObject.SetActive(Mathf.Abs(delta) < _scrollConfiguration.HideObjectBorderPosition);
            }
        }

        protected override void UpdateCurrentId() // Update id of object which locate nearest in center
        {
            float nearestPosition = float.MaxValue;
            int id = CurrentIdOfObject;

            for (int i = 0; i < _childObjects.Length; i++)
            {
                float delta = Mathf.Abs(_scrollDirection.ClampAngle(_childObjects[i].eulerAngles.z));
                
                if (delta > nearestPosition) continue;

                id = i;
                nearestPosition = delta;
            }

            CurrentIdOfObject = id;
        }

        public void OnPauseScroll(bool isPause)
        {
            _moveTween?.Kill();
            
            _contentBeginDragRotateValue = _rootTransform.eulerAngles.z;

            if (!isPause)
            {
                CompleteMove();
            }
        }
    }
}