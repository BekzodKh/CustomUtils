using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomUtils.Scroll.UI
{
    [RequireComponent(typeof(ScrollRect))]
    public class UiScrollBase : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public Action<PointerEventData> BeginDrag = delegate(PointerEventData data) { };
        public Action<PointerEventData> EndDrag = delegate(PointerEventData data) { };

        public int CurrentId => _currentId;
        public float DisableEdge => _disableEdge;
        public float DistanceEdge => _distanceEdge;

        public ScrollType ScrollType => _scrollType;
        public List<RectTransform> Items => _items;
        public ScrollRect ScrollRect => _scrollRect;


        [SerializeField] private bool _initByUser = false;

        private int _currentId = 0;
        private bool _hasConfigurated = false;
        private bool _hasInfinite = false;
        private float _offset = 0;
        private float _disableEdge = 0;
        private float _distanceEdge = 0;
    
        private ScrollType _scrollType;
        private Vector3 _inversePoint;
        private ScrollRect _scrollRect;
        private RectTransform _content;
        private UiSnapScroll _scrollSnap;
        private UiInfiniteScroll _scrollInfinite;
        private ContentSizeFitter _contentSizeFitter;
        private VerticalLayoutGroup _verticalLayoutGroup;
        private HorizontalLayoutGroup _horizontalLayoutGroup;
        private List<RectTransform> _items = new List<RectTransform>();

        public float Offset
        {
            get => _offset;
            set
            {
                if (value > 0f)
                {
                    _offset = value;
                }
            }
        }

        private void Awake()
        {
            if (!_initByUser)
            {
                Init();
            }
            
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }

        public void Init()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollSnap = GetComponent<UiSnapScroll>();
            _scrollInfinite = GetComponent<UiInfiniteScroll>();
        
            _scrollType = _scrollRect.vertical ? ScrollType.Vertical : ScrollType.Horizontal;
            _scrollRect.onValueChanged.AddListener(OnScroll);

            _content = _scrollRect.content;

            for (int i = 0; i < _content.childCount; i++)
            {
                _items.Add(_content.GetChild(i).GetComponent<RectTransform>());
            }

            GetFittersComponent();

            if (_scrollInfinite != null)
            {
                _hasInfinite = true;
                _scrollInfinite.Init();
            }

            if (_scrollSnap != null)
            {
                _scrollSnap.InitScroll();
            }
        }

        private void GetFittersComponent()
        {
            _verticalLayoutGroup = _content.GetComponent<VerticalLayoutGroup>();
            _horizontalLayoutGroup = _content.GetComponent<HorizontalLayoutGroup>();
            _contentSizeFitter = _content.GetComponent<ContentSizeFitter>();
        }

        public void Configure()
        {
            if (_hasConfigurated) return;
        
            _hasConfigurated = true;
        
            _distanceEdge = _offset * _items.Count;
            _disableEdge = _distanceEdge / 2;
        
            if (!_hasInfinite) return;

            if (_verticalLayoutGroup != null)
            {
                _verticalLayoutGroup.enabled = false;
            }

            if (_horizontalLayoutGroup != null)
            {
                _horizontalLayoutGroup.enabled = false;
            }

            if (_contentSizeFitter != null)
            {
                _contentSizeFitter.enabled = false;
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            BeginDrag.Invoke(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            EndDrag.Invoke(eventData);
        }

        public int GetId(int count = 0)
        {
            if (!_hasInfinite)
            {
                return Mathf.Clamp(count, 0, _items.Count - 1);
            }

            return (count + _items.Count) % _items.Count;
        }

        private void OnScroll(Vector2 pos = default)
        {
            Configure();
        
            for (int i = 0; i < _items.Count; i++)
            {
                _inversePoint = _scrollRect.transform.InverseTransformPoint(_items[i].position);

                if (Mathf.Abs(_inversePoint.GetAxisProp(_scrollType)) < _disableEdge / _items.Count)
                {
                    SetCurrentId(i);
                }
            }
        }

        public void SetCurrentId(int id)
        {
            _currentId = GetId(id);
        }
    }
}