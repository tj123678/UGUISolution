namespace UnityEngine.UI.Extensions
{
    [AddComponentMenu("Layout/Extensions/NonDrawingGraphic")]
    public class NonDrawingGraphic : MaskableGraphic
    {
        public override void SetMaterialDirty() { return; }
        public override void SetVerticesDirty() { return; }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            return;
        }
    }
}