using CustomUtils.Scroll.Interfaces;

using UnityEngine;

namespace CustomUtils.Scroll
{
    public class ScrollableContent : IScrollableContent
    {
        private IScrollBehaviour _scrollBehaviour;
        private readonly IContentHolder _contentHolder;

        public ScrollableContent(IContentHolder contentHolder, IScrollBehaviour scrollBehaviour)
        {
            _scrollBehaviour = scrollBehaviour;
            _contentHolder = contentHolder;
        }

        public void InitContentHolder(Transform rootTransform)
        {
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                var contentView = rootTransform.GetChild(i).GetComponent<IContentView>();

                if (contentView == null) continue;

                _contentHolder.AddContent(contentView);
            }
        }

        public void AddContent(IContentView contentView)
        {
            _contentHolder.AddContent(contentView);

            _scrollBehaviour.UpdateContent();
        }

        public void RemoveContent(IContentView contentView)
        {
            _contentHolder.RemoveContent(contentView);

            _scrollBehaviour.UpdateContent();
        }

        public IContentView[] GetContentViews()
        {
            return _contentHolder.GetContentViews();
        }
    }
}