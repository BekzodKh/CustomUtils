using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomUtils.Scroll
{
    public class ScrollInput : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Camera _camera;

        public event Action<Vector2> BeginScrollHandler = delegate { };
        public event Action<Vector2> ContinueScrollHandler = delegate {};
        public event Action<Vector2> EndScrollHandler = delegate {};
        
        private void Start()
        {
            _camera = Camera.main;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector2 pressPosition = _camera.ScreenToWorldPoint(eventData.position);
            BeginScrollHandler.Invoke(pressPosition);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pressPosition = _camera.ScreenToWorldPoint(eventData.position);
            ContinueScrollHandler.Invoke(pressPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 pressPosition = _camera.ScreenToWorldPoint(eventData.position);
            EndScrollHandler.Invoke(pressPosition);
        }
    }
}