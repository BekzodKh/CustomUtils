using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Scroll.UI
{
    [RequireComponent(typeof(UiScrollBase))]
    public class UiInfiniteScroll : MonoBehaviour
    {
        private UiScrollBase _uiScrollBase;
        private ScrollRect _scrollRect;
        private List<RectTransform> _items = new List<RectTransform>();
        private Vector2 _newPosition = Vector2.zero;
        private Vector3 _inversePoint;
        
        public void Init()
        {
            _uiScrollBase = GetComponent<UiScrollBase>();
            _scrollRect = _uiScrollBase.ScrollRect;
            _scrollRect.onValueChanged.AddListener(OnScroll);
            _scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
            _items = _uiScrollBase.Items;
        }

        private void OnScroll(Vector2 pos = default)
        {
            UpdateChildren();
        }

        private void UpdateChildren()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _inversePoint = _scrollRect.transform.InverseTransformPoint(_items[i].position);

                float positionObject = _inversePoint.GetAxisProp(_uiScrollBase.ScrollType);

                if (Mathf.Abs(positionObject) > _uiScrollBase.DisableEdge)
                {
                    UpdateChild(i, (int)Mathf.Sign(positionObject) > 0);
                }
            }
        }

        private void UpdateChild(int id, bool positiveDirection)
        {
            _newPosition = _items[id].anchoredPosition;
            _newPosition.GetAxisProp(_uiScrollBase.ScrollType) += positiveDirection ? -_uiScrollBase.DistanceEdge : _uiScrollBase.DistanceEdge;

            _items[id].anchoredPosition = _newPosition;
        }
    }
}