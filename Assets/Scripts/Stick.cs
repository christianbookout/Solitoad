using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        GetComponentInChildren<SpriteRenderer>().enabled = true;
    }
}
