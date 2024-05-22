using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomImage : Image,IPointerClickHandler
{
    private PolygonCollider2D _polygon;

    private PolygonCollider2D Polygon
    {
        get
        {
            if (_polygon == null)
                _polygon = GetComponent<PolygonCollider2D>();

            return _polygon;
        }   
    }


    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector3 point;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out point);
        return Polygon.OverlapPoint(point);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("点击了");
    }
}
