#nullable enable

using System.Collections.Generic;
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

        bool alreadyJudge = false;
        int laneNum;
        //var inputService = _inputService;
        //int judgeNoteCount = 0;

        foreach (var note in notJudgedNotes) //ノーツ情報の取得
        {

            if (note.JudgeTime - currentTime >= noJudgeTime) // 次の判定するべきノーツが1秒以上離れてたらBreak
            {
                aboveTapState.Clear();
                break;
            }

            foreach (var tapState in aboveTapState) //タップ情報で周回
            {
               // Debug.Log(note.LanePosition + "Lane");
                if(note.Type == NoteType.AboveChain || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlide || note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveTap)
                {
                    laneNum = note.LanePosition + 4;
                }
                else
                {
                    laneNum = note.LanePosition;
                }

                if (laneNum == tapState.laneNumber) //レーン番号が同じとき
                {

                    // 判定ラインを過ぎて 0.75 秒経ったらミスにする
                    if (currentTime - note.JudgeTime > 0.075f)
                    {
                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Miss
                        });

                        continue;
                    }



                    if (!alreadyJudge)//タップ、ホールドとスライドの始点は一回のタップで一度まで判定
                    {
                        if (note.Type == NoteType.Tap)
                        {
                            for (var i = 0; i < note.Size; i++)
                            {

                                // 今押された瞬間だよ
                                if (tapState.TapStating)
                                {
                                    if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                    {
                                        // パーフェクト
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Perfect
                                        });

                                        Debug.Log("Perfect");

                                        // メインのクラスに判定結果を伝えます
                                        //GameObject.FindObjectOfType<RhythmGamePresenter>().HandleJudgeResult(JudgeResultType.Perfect);
                                    }

                                    if (Mathf.Abs(note.JudgeTime - currentTime) < 0.058f)
                                    {
                                        // GOOD
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Good
                                        });

                                        Debug.Log("Good");
                                    }
                                    if (Mathf.Abs(note.JudgeTime - currentTime) < 0.075f)
                                    {
                                        // BAD
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Bad
                                        });

                                        Debug.Log("Bad");
                                    }
                                    if (Mathf.Abs(note.JudgeTime - currentTime) >= 0.075f)
                                    {
                                        // MISS
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Miss
                                        });

                                        Debug.Log("Miss");
                                    }
                                }
                            }
                        }
                        if(note.Type == NoteType.AboveTap)
                        {

                        }
                    }
                }
            }
        }
    }
}

