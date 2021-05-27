using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayNote : MonoBehaviour {
    public float spd;
    
    void Start()
    {
      spd = Variable.speed;
    }

    void Update() {
    Vector3 pos = this.gameObject.transform.position;
    this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + spd);
  }
}
