using System;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ScrollDirection
    {
        /* Get current vector of vector2 by Scroll type */
        public float GetCurrentVector(Vector2 position, ScrollType scrollType)
        {
            return scrollType == ScrollType.Horizontal ? position.x : position.y;
        }
        
        public float GetObjectPosition(Func<float, float, bool> comparer, Transform[] childObjects, ScrollType scrollType)
        {
            float value = GetCurrentVector(childObjects[0].position, scrollType); // default value
            
            for (int i = 0; i < childObjects.Length; i++)
            {
                float position = GetCurrentVector(childObjects[i].position, scrollType);
                
                if (comparer(position, value))
                {
                    value = position;
                }
            }
            
            return value;
        }
        
        public float GetObjectRotation(Func<float, float, bool> comparer, Transform[] childObjects)
        {
            float value = ClampAngle(childObjects[0].eulerAngles.z);;
            
            for (int i = 0; i < childObjects.Length; i++)
            {
                float rotation = ClampAngle(childObjects[i].eulerAngles.z);
                if (comparer(rotation, value))
                {
                    value = rotation;
                }
            }
            
            return value;
        }
        
        /* Calculate negative angle which show clamped by 360 degrees to current negative value */
        public float ClampAngle(float angle)
        {
            return angle > 180 ? angle - 360 : angle;
        }
    }
}