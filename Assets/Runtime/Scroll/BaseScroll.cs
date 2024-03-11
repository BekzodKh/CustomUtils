using CustomUtils.Scroll.Interfaces;
using CustomUtils.Utils.Scroll;

using UnityEngine;
using UnityEngine.Assertions;

namespace CustomUtils.Scroll
{
    public class BaseScroll : MonoBehaviour
    {
        [SerializeField] private BaseScrollConfiguration _scrollConfiguration;
        [SerializeField] private Transform _rootTransform;
        [SerializeField] private ScrollInput _scrollInput;

        public BaseScrollConfiguration ScrollConfiguration => _scrollConfiguration;
        public Transform RootTransform => _rootTransform;
        public ScrollInput ScrollInput => _scrollInput;

        public IContentHolder ContentHolder { get; private set; }

        public IScrollBehaviour ScrollBehaviour { get; protected set; }

        public IScrollableContent ScrollableContent { get; private set; }

        public IPauseScrollBehaviour PauseScrollBehaviour { get; private set; }

        private void Awake()
        {
            OnAwake();

            Assert.IsNotNull(_scrollConfiguration, "[Scroll] You have not appointed scroll configuration to Scroll component!");
            Assert.IsNotNull(_rootTransform, "[Scroll] You have not appointed root transform to Scroll component!");
            Assert.IsNotNull(_scrollInput, "[Scroll] You have not appointed scroll input to Scroll component!");

            if (_scrollConfiguration.SelfInitialize)
                Initialize();
        }

        private void OnDestroy()
        {
            RemoveSubscriptions();
            Destroyed();
        }

        protected virtual void OnAwake() { }
        protected virtual void Destroyed() { }

        public void Initialize()
        {
            BaseComponentsInit();
            InitScrollBehaviour();
        }

        private void BaseComponentsInit()
        {
            ContentHolder = GetComponent<IContentHolder>() ?? gameObject.AddComponent<ContentHolder>();
            PauseScrollBehaviour = (IPauseScrollBehaviour)ScrollBehaviour;
            InitScrollableContent(ContentHolder.GetContentViews().Length == 0);
        }

        private void InitScrollableContent(bool withInitContents = false)
        {
            ScrollableContent = new ScrollableContent(ContentHolder, ScrollBehaviour);

            if (withInitContents)
            {
                ((ScrollableContent)ScrollableContent).InitContentHolder(_rootTransform);
            }
        }

        private void InitScrollBehaviour()
        {
            ScrollBehaviour.SetScrollConfiguration(ScrollConfiguration);

            ScrollInput.BeginScrollHandler += ScrollBehaviour.OnBeginScroll;
            ScrollInput.ContinueScrollHandler += ScrollBehaviour.OnContinueScroll;
            ScrollInput.EndScrollHandler += ScrollBehaviour.OnEndScroll;

            OnInitScrollBehaviour();
        }

        protected virtual void OnInitScrollBehaviour() { }

        private void AddContent(Transform contentViewTransform)
        {
            // Add child to the content
            contentViewTransform.parent = _rootTransform;

            ScrollableContent.AddContent(contentViewTransform.gameObject.GetComponent<ContentView>() ??
                                          contentViewTransform.gameObject.AddComponent<ContentView>());
        }

        private void RemoveContent(Transform contentViewTransform)
        {
            // Remove child of the content
            contentViewTransform.parent = null;

            ScrollableContent.RemoveContent(contentViewTransform.GetComponent<ContentView>());
            Destroy(contentViewTransform.gameObject);
        }

        /// <summary>
        /// Unsubscribe Scroll Behaviour methods from Scroll Input actions
        /// </summary>
        protected void RemoveSubscriptions()
        {
            _scrollInput.BeginScrollHandler -= ScrollBehaviour.OnBeginScroll;
            _scrollInput.ContinueScrollHandler -= ScrollBehaviour.OnContinueScroll;
            _scrollInput.EndScrollHandler -= ScrollBehaviour.OnEndScroll;
        }
    }
}