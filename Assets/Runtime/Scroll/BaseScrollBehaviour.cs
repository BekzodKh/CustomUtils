using System;
using CustomUtils.Scroll.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class BaseScrollBehaviour : IScrollBehaviour
    {
        public event Action<int> ChangedCurrentObjectId = delegate { };

        protected BaseScrollConfiguration _scrollConfiguration;
        protected readonly ScrollDirection _scrollDirection = new ScrollDirection();

        protected IScrollModifier _scrollInfiniteModifier;
        protected IScrollModifier _scrollSnappingModifier;

        protected IContentHolder _contentHolder;

        protected Tween _moveTween;
        protected Transform _rootTransform;
        protected Transform[] _childObjects;
        protected IContentView[] _contentViews;

        protected float _minBorder;
        protected float _maxBorder;

        protected float _startDragTime;
        protected float _endDragTime;

        private int _currentIdOfObject = 0;
        
        /* keeps in itself the current id of scroll object */
        public int CurrentIdOfObject
        {
            get => _currentIdOfObject;
            // A little math so that the Value does not exceed the length of objects;
            private protected set
            {
                int newId = (value + _childObjects.Length) % _childObjects.Length;

                if (_currentIdOfObject != newId)
                {
                    _currentIdOfObject = newId;
                    ChangedCurrentObjectId.Invoke(newId);
                }
            }
        }

        protected event Action UpdateContentPositionHandler = delegate {};

        public void SetScrollConfiguration(BaseScrollConfiguration scrollConfiguration)
        {
            _scrollConfiguration = scrollConfiguration;
        }

        public void SetCurrentIdOfObject(int id)
        {
            _currentIdOfObject = id;
        }

        public void SetContentRoot(Transform rootTransform, IContentHolder contentHolder)
        {
            _rootTransform = rootTransform;
            _contentHolder = contentHolder;
            
            InitScroll();
        }
        
        protected virtual bool HasSnapModifier()
        {
            return _scrollConfiguration.isScrollSnapping;
        }

        protected virtual bool HasInfiniteModifier()
        {
            return _scrollConfiguration.isScrollInfinite;
        }
        
        protected virtual bool HasResizeModifier()
        {
            return _scrollConfiguration.isScrollShouldResizing;
        }

        private void InitScroll()
        {
            InitChildObjects();
            ConfigureInfiniteScroll();
            ConfigureSnappingScroll();
            ConfigureResizingScroll();
            TurnOffNeedlessObjects();

            UnSubscribeToUpdateContent(TurnOffNeedlessObjects);
            SubscribeToUpdateContent(TurnOffNeedlessObjects);

            OnInitScroll();
        }
        
        protected virtual void OnInitScroll() {}
        
        private void InitChildObjects()
        {
            _contentViews = _contentHolder.GetContentViews();
            
            int countObjects = _contentViews.Length;
            
            _childObjects = null;
            _childObjects = new Transform[countObjects];

            int multiplierId = -_scrollConfiguration.CountObjectsCreatedBeforeCurrent;
            int id = (CurrentIdOfObject - _scrollConfiguration.CountObjectsCreatedBeforeCurrent + countObjects) % countObjects;

            for (int i = 0; i < countObjects; i++)
            {
                _contentViews[id].SetIdOfObject(id);
                _contentViews[id].SubscribeToTap(ScrollContentToTargetById);
                
                _childObjects[id] = _contentViews[id].GetTransform();

                SetObjectStartPosition(_childObjects[id], multiplierId);

                if (_scrollConfiguration.isScrollShouldResizing)
                {
                    SetInitialScale(id, id == CurrentIdOfObject ? 
                        _scrollConfiguration.ObjectMaxSizeValue : _scrollConfiguration.ObjectMinSizeValue);
                }

                id = (id + 1) % countObjects;
                multiplierId++;
            }

            _contentViews[CurrentIdOfObject].OnCenterPosition();
            
            CreateBorder();

            ChildsInitialized();
        }

        protected virtual void ChildsInitialized() { }

        protected virtual void CreateBorder() { }

        protected virtual void SetInitialScale(int id, float value) { }
        
        protected virtual void SetObjectStartPosition(Transform scrollObject, int multiplier) { }

        protected virtual void ConfigureInfiniteScroll() { }

        protected virtual void ConfigureSnappingScroll() { }

        protected virtual void ConfigureResizingScroll() { }

        protected virtual void SetScrollBehaviour(IScrollModifier baseScrollModifier)
        {
            baseScrollModifier.SetScrollDirection(_scrollDirection);
            baseScrollModifier.SetScrollType(_scrollConfiguration.ScrollType);
            baseScrollModifier.SetDistanceBetweenObjects(_scrollConfiguration.DistanceObjectsValue);
            baseScrollModifier.SetScrollingObjects(_childObjects);
            SubscribeToUpdateContent(baseScrollModifier.UpdateContentPosition);
        }

        protected virtual void DestroyModifier(ref IScrollModifier baseScrollModifier, Action onComplete)
        {
            if(baseScrollModifier == null) return;
            
            UnSubscribeToUpdateContent(baseScrollModifier.UpdateContentPosition);
            baseScrollModifier = null;
            onComplete?.Invoke();
        }

        public void UpdateContent()
        {
            OnUpdateContent();
            
            InitChildObjects();
            
            _scrollInfiniteModifier.SetScrollingObjects(_childObjects);
            _scrollSnappingModifier.SetScrollingObjects(_childObjects);
            
            TurnOffNeedlessObjects();
        }

        protected virtual void OnUpdateContent() { }

        public virtual void OnBeginScroll(Vector2 pressPosition) { }

        public virtual void OnContinueScroll(Vector2 pressPosition) { }
        
        public virtual void OnEndScroll(Vector2 pressPosition)
        {
            _endDragTime = Time.time;
            
            OnUpdateContentPosition();
            
            OnInertialScroll();
        }

        protected virtual void OnInertialScroll()
        {
            CompleteMove();
        }

        public void CompleteMove()
        {
            UpdateCurrentId();
            
            foreach (var content in _contentViews)
            {
                content.OnEndScroll();
            }
            
            _contentViews[CurrentIdOfObject].OnCenterPosition();

            OnCompleteMove();
        }
        
        protected virtual void OnCompleteMove() { }
        
        public virtual void ScrollContentToTargetById(int id, float duration, Ease ease = Ease.InOutSine, Action complete = null)
        { }
        
        public virtual void ScrollContentToTarget(Vector3 target, float duration, Ease ease = Ease.Linear, Action complete = null)
        { }

        protected virtual void TurnOffNeedlessObjects() // turn off objects which out of showPosition
        { }

        protected virtual void UpdateCurrentId() // Update id of object which locate nearest in center
        { }
        
        public void OnUpdateContentPosition()
        {
            UpdateContentPositionHandler();
        }

        public void UnSubscribeToUpdateContent(Action action)
        {
            UpdateContentPositionHandler -= action;
        }
        
        public void SubscribeToUpdateContent(Action action)
        {
            UpdateContentPositionHandler += action;
        }
    }
}