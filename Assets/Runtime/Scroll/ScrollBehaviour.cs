using System;
using CustomUtils.Scroll.Interfaces;
using CustomUtils.Utils.Scroll;
using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ScrollBehaviour : BaseScrollBehaviour, IPauseScrollBehaviour
    {
        protected IScrollModifier _scrollResizingModifier;
        
        protected Vector2 _contentBeginDragPosition = Vector2.zero;
        protected Vector2 _startPressPosition;
        protected Vector2 _pressPosition;

        protected ScrollSnappingModifier _scrollSnappingModifierRealize;

        protected override bool HasSnapModifier()
        {
            if (_scrollConfiguration.isScrollSnapping)
            {
                ConfigureSnappingScroll();
                return true;
            }
            else
            {
                DestroyModifier(ref _scrollSnappingModifier, () => _scrollSnappingModifierRealize = null);
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
        
        protected override bool HasResizeModifier()
        {
            if (_scrollConfiguration.isScrollShouldResizing)
            {
                ConfigureResizingScroll();
                return true;
            }
            else
            {
                DestroyModifier(ref _scrollResizingModifier, () =>
                {
                    for (int i = 0; i < _rootTransform.childCount; i++)
                    {
                        SetInitialScale(i, _scrollConfiguration.ObjectMaxSizeValue);
                    }
                });
                return false;
            }
        }
        
        protected override void ConfigureInfiniteScroll()
        {
            if(_scrollInfiniteModifier != null || !_scrollConfiguration.isScrollInfinite) return;
            
            // init infinite scroll
            _scrollInfiniteModifier = new ScrollInfiniteModifier();
            SetScrollBehaviour(_scrollInfiniteModifier);
        }
        
        protected override void ConfigureSnappingScroll()
        {
            if(_scrollSnappingModifier != null || !_scrollConfiguration.isScrollSnapping) return;
            
            // init snapping scroll
            _scrollSnappingModifier = new ScrollSnappingModifier();

            _scrollSnappingModifierRealize = (ScrollSnappingModifier) _scrollSnappingModifier;
            _scrollSnappingModifierRealize.SetSnappingModifierParameters(_rootTransform, 
                _scrollConfiguration.SnapAnimationDurationClampValue, _scrollConfiguration.AnimationCurveEase);
            
            SetScrollBehaviour(_scrollSnappingModifier);
        }
        
        protected override void ConfigureResizingScroll()
        {
            if(_scrollResizingModifier != null || !_scrollConfiguration.isScrollShouldResizing) return;
            
            _scrollResizingModifier = new ScrollResizingModifier();
            
            var scrollRealizeModifier = (ScrollResizingModifier) _scrollResizingModifier;
            scrollRealizeModifier.SetObjectSize(_scrollConfiguration.ObjectMinSizeValue, _scrollConfiguration.ObjectMaxSizeValue);

            SetScrollBehaviour(_scrollResizingModifier);
        }

        protected override void CreateBorder()
        {
            float rootTransformPos =
                _scrollDirection.GetCurrentVector(_rootTransform.position, _scrollConfiguration.ScrollType);
            
            _minBorder = _scrollDirection.GetObjectPosition((pos, val) => pos < val, 
                             _childObjects, _scrollConfiguration.ScrollType) - rootTransformPos;
            
            _maxBorder = _scrollDirection.GetObjectPosition((pos, val) => pos > val,
                _childObjects, _scrollConfiguration.ScrollType) - rootTransformPos;
        }
        
        protected override void SetInitialScale(int id, float value)
        {
            var scale = _childObjects[id].localScale;
            scale.Set(value, value, value);
            _childObjects[id].localScale = scale;
        }
        
        protected override void SetObjectStartPosition(Transform scrollObject, int multiplier)
        {
            Vector2 position = scrollObject.position;
            
            float x = multiplier * (_scrollConfiguration.ScrollType == ScrollType.Horizontal 
                ? _scrollConfiguration.DistanceObjectsValue : 0f);
            float y = multiplier * (_scrollConfiguration.ScrollType == ScrollType.Vertical 
                ? _scrollConfiguration.DistanceObjectsValue : 0f);
            
            position.Set(x,y);
            
            scrollObject.position = position;
        }
        
        public override void OnBeginScroll(Vector2 pressPosition)
        {
            HasInfiniteModifier();
            HasResizeModifier();
            
            _moveTween?.Kill();
            _startPressPosition = pressPosition;
            _contentBeginDragPosition = _rootTransform.position;

            _startDragTime = Time.time;

            for (int i = 0; i < _contentViews.Length; i++)
            {
                _contentViews[i].OnStartScroll();
            }

            if (HasSnapModifier())
            {
                _scrollSnappingModifierRealize.OnStartScroll();
            }
        }
        
        public override void OnContinueScroll(Vector2 pressPosition)
        {
            _pressPosition = pressPosition;

            var position = _contentBeginDragPosition + (_pressPosition - _startPressPosition) * _scrollConfiguration.ClampDragValue;

            MoveContent(position);
            OnUpdateContentPosition();
        }
        
        private void MoveContent(Vector2 changedPosition)
        {
            Vector2 currentPosition = _rootTransform.position;
            
            if (_scrollConfiguration.ScrollType == ScrollType.Horizontal)
            {
                currentPosition.Set(changedPosition.x, currentPosition.y);
            }
            else
            {
                currentPosition.Set(currentPosition.x, changedPosition.y);
            }

            SetContentPosition(currentPosition);
        }
        
        private void SetContentPosition(Vector2 position)
        {
            _rootTransform.position = HasInfiniteModifier() ? position : ClampPosition(position);
        }
        
        protected override void OnInertialScroll()
        {
            float currentDragTime = _endDragTime - _startDragTime; // calculate Time of scroll
            float currentDragDistance = _scrollDirection.GetCurrentVector
                (_pressPosition - _startPressPosition, _scrollConfiguration.ScrollType); // calculate Distance of scroll
            float speed = (currentDragDistance / currentDragTime); // and calculate Speed of Scroll
            float deltaTime = Time.unscaledDeltaTime;
            
            float inertia = (speed *_scrollConfiguration.ClampSpeedValue) * deltaTime;

            if (Mathf.Abs(inertia) > _scrollConfiguration.InertiaRestriction)
            {
                inertia = Mathf.Sign(inertia) * _scrollConfiguration.InertiaRestriction;
            }
            
            float delay = Mathf.Abs(inertia) * _scrollConfiguration.ClampInertiaDelayValue;

            _moveTween = DOTween.To(() => inertia, x => inertia = x, 0f, delay)
                .OnUpdate(() =>
                {
                    AddScrollSpeed(inertia);
                    OnUpdateContentPosition();
                }).SetEase(_scrollConfiguration.AnimationCurveEase).OnComplete(CompleteMove);
        }
        
        private void AddScrollSpeed(float speed)
        {
            Vector2 position = _rootTransform.position;

            if (_scrollConfiguration.ScrollType == ScrollType.Horizontal)
            {
                position.x += speed;
            }
            else
            {
                position.y += speed;
            }

            SetContentPosition(position);
        }
        
        protected virtual Vector2 ClampPosition(Vector2 position)
        {
            if (_scrollConfiguration.ScrollType == ScrollType.Horizontal)
            {
                position.Set(Mathf.Clamp(position.x, -_maxBorder, -_minBorder), position.y);
            }
            else
            {
                position.Set(position.x, Mathf.Clamp(position.y, -_maxBorder, -_minBorder));
            }

            return position;
        }
        
        protected override void OnCompleteMove()
        {
            if (HasSnapModifier())
            {
                _scrollSnappingModifierRealize.SnappingContent(CurrentIdOfObject, OnUpdateContentPosition);
            }
        }

        protected override void TurnOffNeedlessObjects() // turn off objects which out of showPosition
        {
            foreach (var currentObject in _childObjects)
            {
                float delta = _scrollDirection.GetCurrentVector(currentObject.position, _scrollConfiguration.ScrollType);
                currentObject.gameObject.SetActive(Mathf.Abs(delta) < _scrollConfiguration.HideObjectBorderPosition);
            }
        }
        
        protected override void UpdateCurrentId() // Update id of object which locate nearest in center
        {
            float nearestPosition = float.MaxValue;
            int id = CurrentIdOfObject;
            
            for (int i = 0; i < _childObjects.Length; i++)
            {
                float position = _scrollDirection.GetCurrentVector(_childObjects[i].position, _scrollConfiguration.ScrollType);
                
                if (!(Mathf.Abs(position) < nearestPosition)) continue;
                
                id = i;
                nearestPosition = Mathf.Abs(position);
            }

            CurrentIdOfObject = id;
        }
        
        public override void ScrollContentToTargetById(int id, float duration, Ease ease = Ease.InOutSine, Action complete = null)
        {
            var target = _rootTransform.position - _childObjects[id].position;
            ScrollContentToTarget(target, duration, ease, complete);
        }
        
        public override void ScrollContentToTarget(Vector3 target, float duration, Ease ease = Ease.Linear, Action complete = null)
        {
            _moveTween?.Kill();

            _moveTween = _rootTransform.DOMove(target, duration).OnStart(() =>
                {
                    int count = _contentViews.Length;
                    for (int i = 0; i < count; i++)
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
        
        public void OnPauseScroll(bool isPause)
        {
            _moveTween?.Kill();
            _contentBeginDragPosition = _rootTransform.position;

            if (!isPause)
            {
                CompleteMove();
            }
        }
    }
}