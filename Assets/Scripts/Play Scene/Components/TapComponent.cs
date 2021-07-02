using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapComponent : MonoBehaviour
{
  public int channel {
    get {
      return (int)Mathf.Round(transform.localPosition.x);
    }
  }

  public float start {
    get {
      return transform.localPosition.z;
    }
  }
}
