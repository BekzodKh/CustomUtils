#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace CustomUtils.ListView
{
    public class ListView<T> : TreeView where T : TreeViewItem
    {
        private const string SortedColumnIndexStateKey = "ListView_sortedColumnIndex";
        private readonly IListViewDelegate<T> _viewDelegate;
        
        public ListView(IListViewDelegate<T> listViewViewDelegate) : this(new TreeViewState(), listViewViewDelegate.Header)
        {
            _viewDelegate = listViewViewDelegate;
        }

        private ListView(TreeViewState state, MultiColumnHeader header) : base(state, header)
        {
            rowHeight = 20;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            header.sortingChanged += SortingChanged;

            header.ResizeToFit();
            Reload();

            header.sortedColumnIndex = SessionState.GetInt(SortedColumnIndexStateKey, 1);
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem {depth = -1};
            root.children = new List<TreeViewItem>();
            return root;
        }

        public void Refresh()
        {
            if (_viewDelegate == null) return;
            rootItem.children = _viewDelegate.GetData();
            BuildRows(rootItem);
            Repaint();
        }

        private void SortingChanged(MultiColumnHeader header)
        {
            SessionState.SetInt(SortedColumnIndexStateKey, multiColumnHeader.sortedColumnIndex);

            if (_viewDelegate == null)
            {
                rootItem.children = new List<TreeViewItem>();
                BuildRows(rootItem);
                return;
            }

            var index = multiColumnHeader.sortedColumnIndex;
            var ascending = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);

            rootItem.children = _viewDelegate.GetSortedData(index, ascending);
            BuildRows(rootItem);
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void ContextClicked()
        {
            _viewDelegate?.OnContextClick();
            base.ContextClicked();
        }

        protected override void SingleClickedItem(int id)
        {
            _viewDelegate?.OnItemClick(id);
            base.SingleClickedItem(id);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as T;

            for (var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++)
            {
                var rect = args.GetCellRect(visibleColumnIndex);
                var columnIndex = args.GetColumn(visibleColumnIndex);

                _viewDelegate.Draw(rect, columnIndex, item, args.selected);
            }
        }
    }
}
#endif
