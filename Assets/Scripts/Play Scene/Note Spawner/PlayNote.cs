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

    void FixedUpdate()
    {
        if (a && togglePlayNote)
        {
            //ノーツの速度の改変のためノーツを速度ではなく位置で管理する。位置は z=-spd*t^2/2+spd*t(tは曲が始まってからの時間
            //ー判定する時間)で計算する
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
