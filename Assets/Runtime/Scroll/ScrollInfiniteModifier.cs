using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ScrollInfiniteModifier : BaseScrollModifier
    {
        protected float _minBorder;
        protected float _maxBorder;
        
        protected override void OnInitChilds()
        {
            OnSetProperties();
        }

        protected virtual void OnSetProperties()
        {
            var maxObjectPosition = _scrollDirection.GetObjectPosition(
                (pos, val) => pos > val, _childObjects, _scrollType);
            
            _minBorder = -maxObjectPosition; // minimal object position value
            _maxBorder = maxObjectPosition; // maximal object position value
        }

        protected override void OnUpdateContentPosition()
        {
            ReplaceObject();
        }

        protected virtual void ReplaceObject()
        {
            /* get the first of object in root transform */
            float first = _scrollDirection.GetObjectPosition((pos, val) => pos > val, _childObjects, _scrollType);
            
            /* get the last of object in root transform */
            float last = _scrollDirection.GetObjectPosition((pos, val) => pos < val, _childObjects, _scrollType);

            for (int i = 0; i < _childObjects.Length; i++)
            {
                Vector2 position = _childObjects[i].position;

                if(_scrollType == ScrollType.Horizontal)
                {
                    position.Set(GetTargetByPosition(position.x, ref last, ref first), position.y);
                }
                else
                {
                    position.Set(position.x, GetTargetByPosition(position.y, ref last, ref first));
                }

                _childObjects[i].position = position;
            }
        }

        private float GetTargetByPosition(float currentPosition, ref float min, ref float max)
        {
            float position = currentPosition;
            
            if (currentPosition >= _maxBorder)
            {
                position = min - _distanceBetweenObjects;
                min = position;
            }
            else if (currentPosition <= _minBorder)
            {
                position = max + _distanceBetweenObjects;
                max = position;
            }
            
            // if does not require a change position
            return position;
        }
    }
}