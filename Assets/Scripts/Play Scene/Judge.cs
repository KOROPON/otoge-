#nullable enable

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Reilas;

public enum JudgeResultType
{
    Perfect,
    Good,
    Bad,
    Miss
}

/// <summary>
/// 判定結果
/// </summary>
public class JudgeResult
{
    public JudgeResultType ResultType;
}

/// <summary>
/// 判定を行うサービス
/// </summary>
public class JudgeService
{
    public static List<JudgeResult> allJudgeType = new List<JudgeResult>();//判定を伝える

    private readonly InputService _inputService = new InputService();

    //private List<JudgeResult> _result = new List<JudgeResult>(10);

    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime,List<LaneTapState> aboveTapState) //Judge(ノーツ情報,再生時間,タップ情報){}
    {
        const float noJudgeTime = 1f; //一番近いノーツがこれより離れてると判定しない

        //var inputService = _inputService;
        //int judgeNoteCount = 0;

        foreach (var note in notJudgedNotes) //ノーツ情報の取得
        {
            /*
            judgeNoteCount++;
            
            if(judgeNoteCount > 4) // 1フレームに判定するのは 4 つまで (リミット)
            {
                break;
            }
            */

            foreach (var tapState in aboveTapState) //タップ情報で周回
            {

                if (note.LanePosition == tapState.laneNumber) //レーン番号が同じとき
                {

                    // 判定ラインを過ぎて 0.75 秒経ったらミスにする
                    if (currentTime - note.JudgeTime > 0.75f)
                    {
                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Miss
                        });

                        continue;
                    }


                    if (note.JudgeTime - currentTime >= noJudgeTime)
                    {
                        break;
                    }

                    if (note.Type == NoteType.Tap)
                    {
                        for (var i = 0; i < note.Size; i++)
                        {

                            // 今押された瞬間だよ
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.2f)
                                {
                                    // パーフェクト
                                    notJudgedNotes.RemoveAt(0);

                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Perfect
                                    });

                                    // メインのクラスに判定結果を伝えます
                                    GameObject.FindObjectOfType<RhythmGamePresenter>().HandleJudgeResult(JudgeResultType.Perfect);
                                }

                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.4f)
                                {
                                    // GOOD
                                    notJudgedNotes.RemoveAt(0);

                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Good
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        //notJudgedNotes.RemoveAt(0);
    }
}

