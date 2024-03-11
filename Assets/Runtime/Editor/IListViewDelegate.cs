#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace CustomUtils.ListView
{
    public interface IListViewDelegate<in T> where T : TreeViewItem
    {
        MultiColumnHeader Header { get; }
        List<TreeViewItem> GetData();
        List<TreeViewItem> GetSortedData(int columnIndex, bool isAscending);
        void Draw(Rect rect, int columnIndex, T data, bool selected);
        void OnItemClick(int id);
        void OnContextClick();
    }
}
#endif
