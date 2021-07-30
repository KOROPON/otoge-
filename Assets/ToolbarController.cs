using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarController : MonoBehaviour
{
    public Button undoButton, redoButton;

    private UndoAction undo;

    void Start()
    {
        undo = FindObjectOfType<UndoAction>();
        undoButton.onClick.AddListener(undo.Undo);
        redoButton.onClick.AddListener(undo.Redo);
    }
}
