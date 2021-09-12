#nullable enable

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
public class JudgeResultInHold
{
    public float time;
    public bool perfect;
}

public class DelNote
{
    public float noteTime;
    public int noteNum;
}


/// <summary>
/// 判定を行うサービス
/// </summary>
public class JudgeService : MonoBehaviour
{
    public static List<JudgeResult> allJudgeType = new List<JudgeResult>();//判定を伝える
    public static List<JudgeResultInHold> judgedInHold = new List<JudgeResultInHold>(); //長押しノーツの内部判定を伝える

    public static string aa = null!;
    public static int per;
    public static int good;
    public static int bad;
    public static int miss;


    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime, List<LaneTapState> aboveTapState) //Judge(ノーツ情報,再生時間,タップ情報){}
    {
        Debug.Log("Judge");
        const float noJudgeTime = 0.090f; //一番近いノーツがこれより離れてると判定しない
        float tapTime = 0; //同じ時間にタップがある時、その時間

        bool firstTap = true;
        bool alreadyJudge = false;
        int laneNumMin;
        int laneNumMax;
        List<DelNote> removeNoteNum = new List<DelNote>();

        void RemoveNoteInList(List<DelNote> noteNum)
        {
            noteNum.OrderBy(note => note.noteTime);
            for (var i = noteNum.Count - 1; i >= 0; i--)
            {
                notJudgedNotes.RemoveAt(noteNum[i].noteNum);
            }
        }



        foreach (LaneTapState tapState in aboveTapState) //タップ情報で周回
        {
            Debug.Log(tapState.TapStating);

            foreach (ReilasNoteEntity note in notJudgedNotes) //ノーツ情報の取得
            {
                //Debug.Log(note.Type);

                if (note.JudgeTime - currentTime > noJudgeTime) // 次の判定するべきノーツが前に 0.090 秒 以上離れてたら returns
                {
                    Debug.Log("return");
                    aboveTapState.Clear();
                    return;
                }




                ///<summary>
                ///ノーツのレーン情報を取得
                ///
                /// 上のノーツは Json のレーン番号に +4
                /// 下のノーツはそのまま
                ///
                ///</summary>
                if (note.Type == NoteType.AboveChain || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlide || note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveTap)
                {
                    laneNumMin = note.LanePosition + 4;
                    laneNumMax = laneNumMin + note.Size - 1;
                }
                else
                {
                    laneNumMin = note.LanePosition;
                    laneNumMax = note.LanePosition;
                }


                if (firstTap && (note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide))
                {
                    tapTime = note.JudgeTime;
                    firstTap = false;
                }


                if (laneNumMin <= tapState.laneNumber && tapState.laneNumber <= laneNumMax) //レーン番号が同じとき
                {

                    if (note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide)//タップ、ホールドとスライドの始点は一回のタップで一度まで判定、同じ時間なら複数
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // 今押された瞬間だよ
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                {
                                    // パーフェクト
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    }); ;
                                    per++;
                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Perfect
                                    });

                                    //Debug.Log("Perfect");
                                }
                                else if (Mathf.Abs(note.JudgeTime - currentTime) < 0.058f)
                                {
                                    // GOOD
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    }); ;
                                    good++;
                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Good
                                    });

                                    //Debug.Log("Good");
                                }
                                else if (Mathf.Abs(note.JudgeTime - currentTime) < 0.075f)
                                {
                                    // BAD
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    }); ;
                                    bad++;
                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Bad
                                    });

                                    //Debug.Log("Bad");
                                }
                                else if (currentTime - note.JudgeTime >= 0.075f)
                                {
                                    // MISS
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    }); ;
                                    miss++;
                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Miss
                                    });

                                    //Debug.Log("Miss");
                                }
                                //}
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if (note.Type == NoteType.HoldInternal || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlideInternal) //長押し系ノーツの内部判定は前に 90
                    {
                        if (note.JudgeTime - currentTime < 0.090f || note.JudgeTime - currentTime > 0)
                        {
                            judgedInHold.Add(new JudgeResultInHold
                            {
                                time = note.JudgeTime,
                                perfect = true
                            });
                            per++;
                            removeNoteNum.Add(new DelNote
                            {
                                noteTime = note.JudgeTime,
                                noteNum = notJudgedNotes.IndexOf(note)
                            }); ;
                        }
                        else if (note.JudgeTime - currentTime < 0)
                        {
                            judgedInHold.Add(new JudgeResultInHold
                            {
                                time = note.JudgeTime,
                                perfect = false
                            });
                            miss++;
                        }
                    }
                    else if (note.Type == NoteType.AboveChain) //チェインノーツは後ろに 25 の判定幅
                    {
                        if (currentTime - note.JudgeTime >= 0 || currentTime - note.JudgeTime <= 0.025f)
                        {
                            //Perfect
                            removeNoteNum.Add(new DelNote
                            {
                                noteTime = note.JudgeTime,
                                noteNum = notJudgedNotes.IndexOf(note)
                            }); ;

                            allJudgeType.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Perfect
                            });
                            per++;
                        }
                        else if (currentTime - note.JudgeTime > 0.025f)
                        {
                            removeNoteNum.Add(new DelNote
                            {
                                noteTime = note.JudgeTime,
                                noteNum = notJudgedNotes.IndexOf(note)
                            }); ;
                            allJudgeType.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Miss
                            });
                            miss++;
                        }
                    }
                }
            }

            RemoveNoteInList(removeNoteNum);
        }



        foreach (ReilasNoteEntity note in notJudgedNotes) //タップされていない場合に使用される Miss 判定
        {
            if(note.JudgeTime - currentTime > 0.075f)
            {
                break;
            }

            if(note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.HoldInternal)
            {
                if(note.JudgeTime - currentTime >= 0)
                {
                    continue;
                }
                else
                {
                    removeNoteNum.Add(new DelNote
                    {
                        noteTime = note.JudgeTime,
                        noteNum = notJudgedNotes.IndexOf(note)
                    }); ;
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    miss++;
                }
            }
            else if(note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide)
            {
                if(currentTime - note.JudgeTime > 0.075f)
                {
                    removeNoteNum.Add(new DelNote
                    {
                        noteTime = note.JudgeTime,
                        noteNum = notJudgedNotes.IndexOf(note)
                    }); ;
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    miss++;
                }
                else
                {
                    continue;
                }
            }
            else if(note.Type == NoteType.AboveChain)
            {
                if(currentTime - note.JudgeTime > 0.025f)
                {
                    removeNoteNum.Add(new DelNote
                    {
                        noteTime = note.JudgeTime,
                        noteNum = notJudgedNotes.IndexOf(note)
                    }); ;
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    miss++;
                }
                else
                {
                    continue;
                }
            }
        }
        RemoveNoteInList(removeNoteNum);

        aa = ("Perfect:" + per.ToString() + "  Good:" + good.ToString() + "  Bad:" + bad.ToString() + "  Miss:" + miss.ToString());
        
    }
}

