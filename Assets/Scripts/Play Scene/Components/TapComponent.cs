using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapComponent : MonoBehaviour
{
    public int channel
    {
        get
        {
            return (int)Mathf.Round(transform.localPosition.x);
        }

        set
        {
            transform.localPosition = new Vector3(NoteSpawner.getChannelX(value), transform.localPosition.y, transform.localPosition.z);
        }
    }

    public float start
    {
        get
        {
            return transform.localPosition.z;
        }
    }
}
