using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayNote : MonoBehaviour {
    public float spd;
    void Start()
    {
        spd = GameObject.Find("Run Programs").GetComponent<Variable>().speed;
    }
    //void Main() {
    //var timer = new Timer(30);
    // ^C}[Ì
    //timer.Elapsed += (sender, e) => {
    //Debug.Log("");
    //Vector3 pos = this.gameObject.transform.position;
    //this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + spd / 10);
    //};
    //timer.Start();
    //}
    void Update() {
    Vector3 pos = this.gameObject.transform.position;
    this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + spd);
  }
}
