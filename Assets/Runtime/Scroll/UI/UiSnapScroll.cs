using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomUtils.Scroll.UI
{
    [RequireComponent(typeof(UiScrollBase))]
    public class UiSnapScroll : MonoBehaviour
    {
        [SerializeField] protected Ease _typeEase;
        [SerializeField] protected float _minThresholdVelocity = 1000f;
        [SerializeField] protected float _maxThresholdVelocity = 5000f;
        [SerializeField] protected float _tweenDuration = 0.4f;
        [SerializeField] protected bool _autoCorrectOnInit;

        protected UiScrollBase _uiScrollBase;
        protected List<RectTransform> _items;
        protected IEnumerator _onEndDrag;
        protected Vector2 _inverseDistance;
        protected Vector2 _rectVelocity;
        protected Tween _tweenContent;
        protected ScrollRect _scrollRect;
        protected Transform _scrollContent;
        protected YieldInstruction _frameWaiter = new WaitForEndOfFrame();

        public void InitScroll()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _uiScrollBase = GetComponent<UiScrollBase>();
            _scrollContent = _scrollRect.content;
            _items = _uiScrollBase.Items;
            _uiScrollBase.BeginDrag += OnBeginDrag;
            _uiScrollBase.EndDrag += OnEndDrag;

            if (_autoCorrectOnInit)
            {
                DOVirtual.DelayedCall(0.2f, () => GoPage(0));
            }
        }

        protected virtual void OnBeginDrag(PointerEventData eventData)
        {
            _tweenContent?.Kill();
            StopOnEndDragCoroutine();
        }


        protected virtual void OnEndDrag(PointerEventData eventData)
        {
            var rectVelocity = _scrollRect.velocity;

            if (Mathf.Abs(rectVelocity.GetAxisProp(_uiScrollBase.ScrollType)) > _minThresholdVelocity &&
                Mathf.Abs(rectVelocity.GetAxisProp(_uiScrollBase.ScrollType)) < _maxThresholdVelocity)
            {
                Move(_uiScrollBase.GetId(-(int) Mathf.Sign(rectVelocity.GetAxisProp(_uiScrollBase.ScrollType))));
            }
            else
            {
                _onEndDrag = WaitLowVelocityCoroutine(); 
                StartCoroutine(_onEndDrag);
            }
        }

        protected IEnumerator WaitLowVelocityCoroutine()
        {
            do
            {
                _rectVelocity = _scrollRect.velocity;
                yield return _frameWaiter;
            }
            while (Mathf.Abs(_rectVelocity.GetAxisProp(_uiScrollBase.ScrollType)) > _minThresholdVelocity);
            
            Move(_uiScrollBase.CurrentId);
        }

        protected void StopOnEndDragCoroutine()
        {
            if (_onEndDrag != null)
            {
                StopCoroutine(_onEndDrag);
            }
        }

        protected void Move(int id)
        {
            _scrollRect.velocity = Vector2.zero;
            _inverseDistance = _scrollRect.transform.GetInverseDistance(_items[id].position, _scrollContent.position);

            _tweenContent?.Kill();
            _tweenContent = _uiScrollBase.ScrollRect.content.DOAnchorPos(_inverseDistance, _tweenDuration);
            _tweenContent.SetEase(_typeEase);
        }

        public void GoPage(int to)
        {
            StopOnEndDragCoroutine();
            Move(_uiScrollBase.GetId(to));
        }
    }
}