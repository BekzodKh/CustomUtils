using UnityEngine;

namespace CustomUtils.Scroll.Interfaces
{
    public interface IScrollModifier
    {
        void SetScrollType(ScrollType currentScrollType);

        void UpdateContentPosition();

        void SetScrollingObjects(Transform[] currentChildObjects);

        void SetDistanceBetweenObjects(float distance);

        void SetScrollDirection(ScrollDirection scrollDirection);
    }
}