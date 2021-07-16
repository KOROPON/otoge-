using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldComponents : MonoBehaviour
{
    public float zScale
    {
        get
        {
            return GetComponentsInParent<NoteSpawner>()[0].zScale;
        }
    }

    public int channel
    {
        get
        {
            return (int)Mathf.Round(transform.localPosition.x);
        }
    }

    public float start
    {
        get
        {
            return (transform.localPosition.z + transform.localScale.z / 2) / zScale;
        }
    }

    public float end
    {
        get
        {
            return transform.localScale.z / zScale + start;
        }
    }
}
