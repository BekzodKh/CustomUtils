using System;

using DG.Tweening;

using UnityEngine;

namespace CustomUtils.Scroll.Interfaces
{
    public interface IContentView
    {
        void SetIdOfObject(int id);

        void OnStartScroll();
        
        void OnEndScroll();
        
        void OnCenterPosition();
        
        void SubscribeToTap(Action<int, float, Ease, Action> method);

        Transform GetTransform();
    }
}