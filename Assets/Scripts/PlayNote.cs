using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayNote : MonoBehaviour {
    public float spd;
    public bool a = false;
    NoteSpawner sc;
    void Start()
    {
      spd = Variable.speed;
      sc = GameObject.Find("Note").GetComponent<NoteSpawner>();
      StartCoroutine("MoveNote");
    }

    void Update()
    {
        if (a)
        {
            Vector3 pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + spd);
        }
    }

    IEnumerator MoveNote()
    {
        yield return new WaitForSeconds(5);
        sc.wait = true;
        a = true;
    }
}
