using System.Collections.Generic;
using CustomUtils.Scroll.Interfaces;
using UnityEngine;

namespace CustomUtils.Scroll
{
    /* This Class will be container for root transform childs */
    public class ContentHolder : MonoBehaviour, IContentHolder
    {
        [SerializeField] private Transform[] _contents;
        
        private readonly List<IContentView> _contentViewList = new List<IContentView>();

        private void Awake()
        {
            GetContentViewsByTransform();
        }

        private void GetContentViewsByTransform()
        {
            if (_contents == null || _contentViewList.Count >= _contents.Length) return;
            
            for (int i = 0; i < _contents.Length; i++)
            {
                _contentViewList.Add(_contents[i].GetComponent<IContentView>());
            }
        }
        
        public void AddContent(IContentView contentView)
        {
            _contentViewList.Add(contentView);
        }

        public void RemoveContent(IContentView contentView)
        {
            _contentViewList.Remove(contentView);
        }

        public IContentView[] GetContentViews()
        {
            GetContentViewsByTransform();
            
            return _contentViewList.ToArray();
        }
    }
}