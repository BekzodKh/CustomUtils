using System;
using CustomUtils.Scroll.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ContentView : MonoBehaviour, IContentView
    {
        private int _objectIdentifier;
        
        public void SetIdOfObject(int id)
        {
            _objectIdentifier = id;
            Debug.Log("Set scroll object id " + id);
        }

        public void OnStartScroll()
        {
            Debug.Log("Start Scroll Move");
        }

        public void OnEndScroll()
        {
            Debug.Log("End Scroll Move");
        }

        public void OnCenterPosition()
        {
            Debug.Log($"ContentView with id {_objectIdentifier}, on center of scroll");
        }

        public void SubscribeToTap(Action<int, float, Ease, Action> method)
        {
            Debug.Log($"On Tap");
        }
        
        public Transform GetTransform()
        {
            return transform;
        }
    }
}