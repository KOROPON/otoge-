using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TapDragAction : IUndo
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Transform transform;

    public TapDragAction(Vector3 startPos, Vector3 endPos, Transform trans)
    {
        startPosition = startPos;
        endPosition = endPos;
        transform = trans;
    }

    public void Undo()
    {
        transform.localPosition = startPosition;
    }

    public void Redo()
    {
        transform.localPosition = endPosition;
    }
}

public class TapDrag : MonoBehaviour
{
    private TapComponent tap;

    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 startPosition;

    private UndoAction undo;

    void Start()
    {
        tap = GetComponentInParent<TapComponent>();
        undo = FindObjectOfType<UndoAction>();
    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        startPosition = transform.localPosition;
    }

    void OnMouseDrag()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        Debug.Log(cursorPosition.x);
        tap.channel = NoteSpawner.getChannelFromX(cursorPosition.x);
        Debug.Log(tap.channel);
    }

    void OnMouseUp()
    {
        undo.Add(new TapDragAction(startPosition, transform.localPosition, transform));
    }

    void Update()
    {

    }
}
