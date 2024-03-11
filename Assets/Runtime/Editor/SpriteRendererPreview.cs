#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace CustomUtils
{
    [CustomPreview(typeof(SpriteRenderer))]
    public class SpriteRendererPreview : ObjectPreview
    {
        private Texture _sprite = null;

        private bool HasSprite()
        {
            var spriteRenderer = (target as SpriteRenderer)?.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                _sprite = AssetPreview.GetAssetPreview(spriteRenderer.sprite);
                return true;
            }

            return false;
        }

        public override bool HasPreviewGUI()
        {
            return HasSprite();
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            EditorGUI.DrawTextureTransparent(rect, _sprite, ScaleMode.ScaleToFit);
        }
    }

}
#endif