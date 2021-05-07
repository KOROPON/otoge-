using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNote : MonoBehaviour {
  public int spd = 1;

  void Update() {
      Vector3 pos = gameObject.transform.position;
      gameObject.transform.position = new Vector3(pos.x,pos.y,pos.z + spd);
  }

  public void Plus() {
    spd += 5;
  }

  public void Minus() {
    spd -= 5;
  }
}
