using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUndo
{
    public void Undo();
    public void Redo();
}

public class UndoAction : MonoBehaviour
{
    private Stack<IUndo> undo = new Stack<IUndo>();
    private Stack<IUndo> redo = new Stack<IUndo>();

    private bool yKeyDown = false;
    private bool zKeyDown = false;

    public void Undo()
    {
        Debug.Log(undo.Count);
        if (undo.Count > 0)
        {
            IUndo action = undo.Pop();
            action.Undo();
            redo.Push(action);
        }
    }

    public void Redo()
    {
        Debug.Log(undo.Count);
        if (redo.Count > 0)
        {
            IUndo action = redo.Pop();
            action.Redo();
            undo.Push(action);
        }
    }

    public void Add(IUndo action)
    {
        undo.Push(action);
        redo.Clear();
    }

    public void Update()
    {
        // For testing purposes!
        if (Input.GetKey("z"))
        {
          if (!zKeyDown)
          {
            zKeyDown = true;
            Undo();
          }
        }
        else
        {
          zKeyDown = false;
        }
        if (Input.GetKey("y") && !yKeyDown)
        {
          if (!yKeyDown)
          {
            yKeyDown = true;
            Redo();
          }
        }
        else
        {
          yKeyDown = false;
        }
    }
}
