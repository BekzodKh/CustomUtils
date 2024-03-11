using System;

using UnityEngine;

namespace CustomUtils.Scroll.Interfaces
{
    public interface IScrollBehaviour
    {
        event Action<int> ChangedCurrentObjectId;

        void SetCurrentIdOfObject(int id);

        void SetContentRoot(Transform rootTransform, IContentHolder contentHolder);

        void SetScrollConfiguration(BaseScrollConfiguration scrollConfiguration);

        void OnBeginScroll(Vector2 pressPosition);

        void OnContinueScroll(Vector2 pressPosition);

        void OnEndScroll(Vector2 pressPosition);

        void UpdateContent();
    }
}