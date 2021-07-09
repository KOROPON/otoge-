using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapComponent : MonoBehaviour
{
  public int channel {
    get {
      return (int)Mathf.Round(transform.localPosition.x);
    }

    set {
      transform.position = new Vector3(NoteSpawner.getChannelX(value), transform.position.y, transform.position.z);
    }
  }

  public float start {
    get {
      return transform.localPosition.z;
    }
  }
}
