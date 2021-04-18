using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNote : MonoBehaviour {
  public int spd = 10;

  void Update() {
      Vector3 pos = gameObject.transform.position;
      if(pos.z > -5000) {
        gameObject.transform.position = new Vector3(pos.x,pos.y,pos.z -spd);
    }
  }

  public void Plus() {
    spd += 5;
  }

  public void Minus() {
    spd -= 5;
  }
}
