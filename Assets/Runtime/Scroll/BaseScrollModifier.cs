using CustomUtils.Scroll.Interfaces;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class BaseScrollModifier : IScrollModifier
    {
        protected ScrollType _scrollType = ScrollType.Horizontal;
        protected Transform[] _childObjects;
        protected float _distanceBetweenObjects;
        protected ScrollDirection _scrollDirection;

        public void SetScrollType(ScrollType currentScrollType)
        {
            _scrollType = currentScrollType;
        }

        public void UpdateContentPosition()
        {
            OnUpdateContentPosition();
        }
        
        protected virtual void OnUpdateContentPosition() {}

        public void SetScrollingObjects(Transform[] currentChildObjects) // Set the Scrolling Objects (Contents view)
        {
            _childObjects = currentChildObjects;
            
            OnInitChilds();
        }
        
        public void SetDistanceBetweenObjects(float distance)
        {
            _distanceBetweenObjects = distance;
        }

        public void SetScrollDirection(ScrollDirection scrollDirection)
        {
            _scrollDirection = scrollDirection;
        }

        protected virtual void OnInitChilds(){}
    }
}