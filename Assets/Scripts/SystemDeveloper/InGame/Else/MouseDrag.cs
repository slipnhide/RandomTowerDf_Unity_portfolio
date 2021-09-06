using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDrag : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(gameObject);
    }
}
