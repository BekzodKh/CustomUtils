namespace CustomUtils.Scroll.RoundScroll
{
    public class RoundScrollInfiniteModifier : ScrollInfiniteModifier
    {
        protected override void OnSetProperties()
        {
            var maxObjectRotation = _scrollDirection.GetObjectRotation((pos, val) => pos > val, _childObjects);
            
            _scrollDirection.ClampAngle(maxObjectRotation);
            
            _minBorder = -maxObjectRotation; // minimal object rotation value
            _maxBorder = maxObjectRotation; // maximal object rotation value
        }

        protected override void ReplaceObject()
        {
            /* get the first of object in root transform */
            float first = _scrollDirection.GetObjectRotation((pos, val) => pos > val, _childObjects);
            
            /* get the last of object in root transform */
            float last = _scrollDirection.GetObjectRotation((pos, val) => pos < val, _childObjects);

            for (int i = 0; i < _childObjects.Length; i++)
            {
                var rotation = _childObjects[i].eulerAngles;

                float zDelta = _scrollDirection.ClampAngle(rotation.z);

                rotation.Set(rotation.x, rotation.y, GetTargetByRotation(zDelta, ref last, ref first));

                _childObjects[i].eulerAngles = rotation;
            }
        }

        private float GetTargetByRotation(float currentRotation, ref float min, ref float max)
        {
            float rotate = _scrollDirection.ClampAngle(currentRotation);
            
            if (rotate <= _minBorder)
            {
                rotate = _scrollDirection.ClampAngle(max + _distanceBetweenObjects);
                max = rotate;
            }
            else if (rotate >= _maxBorder)
            {
                rotate = _scrollDirection.ClampAngle(min - _distanceBetweenObjects);
                min = rotate;
            }
            
            return rotate;
        }
    }
}