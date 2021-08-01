using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayNote : MonoBehaviour
{
    public float spd;
    public bool a = false;

    [Tooltip("NoteSpawnerから譜面作成用にPlayNoteをオフにする")]
    public bool togglePlayNote = true;

    void Start()
    {
        spd = Variable.speed;
        StartCoroutine("MoveNote");
    }

    void Update()
    {
        if (a && togglePlayNote)
        {
            Vector3 pos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + spd * Time.deltaTime * 60);
        }
    }

    IEnumerator MoveNote()
    {
        yield return new WaitForSeconds(3);
        a = true;
    }
}