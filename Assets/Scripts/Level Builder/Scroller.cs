using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour {
  public float scrollSpeed = 1f;

  void Update() {
    transform.position += new Vector3(0f, 0f, Input.mouseScrollDelta.y * scrollSpeed);
  }
}
