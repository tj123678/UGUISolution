using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wepie.UILibrary
{
    public class UIDepth : MonoBehaviour
    {
        public int order;

        public OrderMode orderMode = OrderMode.Absolute;

        public bool isUI = true;

        public bool raycastTarget = true;

        private bool m_hasInited = false;

        private int m_realOrder;

        private int m_sortingLayerId;

        private bool m_hasUpdateRealOrder;

        private Canvas m_canvas;

        private GraphicRaycaster m_graphicRaycaster;

        private void Update()
        {
            var parentUIDepth = GetParentComponent<UIDepth>();
            // 只有根节点才会更新所有Render
            if (parentUIDepth != null)
            {
                return;
            }
            
            UpdateSortingOrder();
        }

        private T GetParentComponent<T>() where T : Component
        {
            var parentTransform = transform.parent;
            if (parentTransform == null)
            {
                return null;
            }

            var parentComponent = parentTransform.GetComponentInParent<T>();
            return parentComponent;
        }

        public void UpdateSortingOrder()
        {
            var allUIDepth = ListPool<UIDepth>.Get();
            GetComponentsInChildren<UIDepth>(allUIDepth);

            UpdateAllSortingLayer(allUIDepth);
            UpdateAllRealOrder(allUIDepth);

            UpdateRendererSortingOrder();

            foreach (var uiDepth in allUIDepth)
            {
                uiDepth.UpdateUISortingOrder();
            }

            ListPool<UIDepth>.Release(allUIDepth);
        }

        private void UpdateAllSortingLayer(List<UIDepth> allUIDepth)
        {
            var sortingLayerID = 0;
            var parentCanvas = GetParentComponent<Canvas>();
            if (parentCanvas == null)
            {
                sortingLayerID = SortingLayer.NameToID("Default");
            }
            else
            {
                sortingLayerID = parentCanvas.sortingLayerID;
            }

            foreach (var uiDepth in allUIDepth)
            {
                uiDepth.m_sortingLayerId = sortingLayerID;
            }
        }

        private void UpdateAllRealOrder(List<UIDepth> allUIDepth)
        {
            foreach (var uiDepth in allUIDepth)
            {
                uiDepth.m_hasUpdateRealOrder = false;
            }
            foreach (var uiDepth in allUIDepth)
            {
                uiDepth.UpdateRealOrder();
            }
        }

        private void UpdateRealOrder()
        {
            if (m_hasUpdateRealOrder)
            {
                return;
            }

            if (orderMode == OrderMode.Absolute)
            {
                m_realOrder = order;
            }
            else if (orderMode == OrderMode.Relative)
            {
                m_realOrder = order;
                var parentUIDepth = GetParentComponent<UIDepth>();
                if (parentUIDepth != null)
                {
                    parentUIDepth.UpdateRealOrder();
                    m_realOrder += parentUIDepth.m_realOrder;
                }
            }

            m_hasUpdateRealOrder = true;
        }

        private void UpdateUISortingOrder()
        {
            if (!isUI)
            {
                return;
            }
            
            EnsureCanvas();
            if (m_canvas.renderMode == RenderMode.WorldSpace)
            {
                m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
            if (m_canvas.overrideSorting != true)
            {
                m_canvas.overrideSorting = true;
            }
            if (m_canvas.sortingLayerID != m_sortingLayerId)
            {
                m_canvas.sortingLayerID = m_sortingLayerId;
            }
            if (m_canvas.sortingOrder != m_realOrder)
            {
                m_canvas.sortingOrder = m_realOrder;
            }
            if (raycastTarget)
            {
                EnsureGraphicRaycaster();
            }
        }

        private void EnsureCanvas()
        {
            if (m_canvas != null)
            {
                return;
            }

            m_canvas = GetComponent<Canvas>();
            if (m_canvas == null)
            {
                m_canvas = gameObject.AddComponent<Canvas>();
            }
        }

        private void EnsureGraphicRaycaster()
        {
            if (m_graphicRaycaster != null)
            {
                return;
            }

            m_graphicRaycaster = GetComponent<GraphicRaycaster>();
            if (m_graphicRaycaster == null)
            {
                m_graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        private void UpdateRendererSortingOrder()
        {
            var allRenderers = ListPool<Renderer>.Get();
            GetComponentsInChildren<Renderer>(allRenderers);

            foreach(Renderer render in allRenderers)
            {
                var belongUIDepth = render.GetComponentInParent<UIDepth>();
                render.sortingLayerID = belongUIDepth.m_sortingLayerId;
                render.sortingOrder = belongUIDepth.m_realOrder;
            }

            ListPool<Renderer>.Release(allRenderers);
        }
    }

    public enum OrderMode
    {
        Absolute = 0,

        Relative = 1,
    }
}