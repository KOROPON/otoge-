using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapDrag : MonoBehaviour
{
    private TapComponent tap;

    private Vector3 screenPoint;
    private Vector3 offset;

    void Start()
    {
        tap = GetComponentInParent<TapComponent>();
    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(
          new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        Debug.Log(cursorPosition.x);
        tap.channel = NoteSpawner.getChannelFromX(cursorPosition.x);
        Debug.Log(tap.channel);
    }

    void Update()
    {

    }
}
