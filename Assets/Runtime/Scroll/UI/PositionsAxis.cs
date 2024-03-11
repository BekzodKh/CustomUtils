using UnityEngine;

namespace CustomUtils.Scroll.UI
{
    public static class PositionsAxis
    {
        public static ref float GetAxisProp(this ref Vector2 position, ScrollType type)
        {
            return ref type == ScrollType.Horizontal ? ref position.x : ref position.y;
        }
        
        public static ref float GetAxisProp(this ref Vector3 position, ScrollType type)
        {
            return ref type == ScrollType.Horizontal ? ref position.x : ref position.y;
        }
        
        public static Vector2 GetInverseDistance(this Transform parentContent, Vector2 target, Vector2 content)
        {
            return parentContent.InverseTransformPoint(content) - parentContent.InverseTransformPoint(target);
        }
        
    }
}