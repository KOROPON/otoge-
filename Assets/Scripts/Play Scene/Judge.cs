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
    public static List<float> judgedInHold = new List<float>(); //長押しノーツの内部判定を伝える


    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime,List<LaneTapState> aboveTapState) //Judge(ノーツ情報,再生時間,タップ情報){}
    {
        const float noJudgeTime = 0.090f; //一番近いノーツがこれより離れてると判定しない
        float tapTime = 0; //同じ時間にタップがある時、その時間

        bool firstTap = true;
        bool alreadyJudge = false;
        int laneNumMin;
        int laneNumMax;
        
        /*
        void RemoveNoteInList(List<int> noteNum)
        {

        }
        */

        foreach (var tapState in aboveTapState) //タップ情報で周回
        {

            foreach (var note in notJudgedNotes) //ノーツ情報の取得
            {
                if (note.JudgeTime - currentTime > noJudgeTime) // 次の判定するべきノーツが前に 0.090 秒 以上離れてたら returns
                {
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
                if(note.Type == NoteType.AboveChain || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlide || note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveTap)
                {
                    laneNumMin = note.LanePosition + 4;
                    laneNumMax = laneNumMin + note.Size -1;
                }
                else
                {
                    laneNumMin = note.LanePosition;
                    laneNumMax = note.LanePosition;
                }

                /*
                if (Mathf.Abs(note.JudgeTime - currentTime) <= 0.01)
                {
                    Debug.Log(note.LanePosition);
                }
                */

                if (firstTap || note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide)
                {
                    if (currentTime - note.JudgeTime > 0.075f)
                    {
                        notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));

                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Miss
                        });
                        break;
                    }
                    ///<summary>
                    /// 
                    /// </summary>
                    tapTime = note.JudgeTime;
                    firstTap = false;
                }
                

                if (laneNumMin <= tapState.laneNumber && tapState.laneNumber <= laneNumMax) //レーン番号が同じとき
                {


                    if (note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide || tapState.TapStating)//タップ、ホールドとスライドの始点は一回のタップで一度まで判定、同じ時間なら複数
                    {
                        if (!alreadyJudge && tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // 今押された瞬間だよ
                            //if ()
                            //{
                            if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                            {
                                // パーフェクト
                                notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));

                                allJudgeType.Add(new JudgeResult
                                {
                                    ResultType = JudgeResultType.Perfect
                                });

                                Debug.Log("Perfect");
                            }
                            else if (Mathf.Abs(note.JudgeTime - currentTime) < 0.058f)
                            {
                                // GOOD
                                notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));

                                allJudgeType.Add(new JudgeResult
                                {
                                    ResultType = JudgeResultType.Good
                                });

                                Debug.Log("Good");
                            }
                            else if (Mathf.Abs(note.JudgeTime - currentTime) < 0.075f)
                            {
                                // BAD
                                notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));

                                allJudgeType.Add(new JudgeResult
                                {
                                    ResultType = JudgeResultType.Bad
                                });

                                Debug.Log("Bad");
                            }
                            else if (currentTime - note.JudgeTime >= 0.075f)
                            {
                                // MISS
                                notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));

                                allJudgeType.Add(new JudgeResult
                                {
                                    ResultType = JudgeResultType.Miss
                                });

                                Debug.Log("Miss");
                            }
                            //}
                            //}
                        }
                    }
                    else if(note.Type == NoteType.HoldInternal || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlideInternal) //長押し系ノーツの内部判定は前に 90
                    {
                        judgedInHold.Add(note.JudgeTime);
                        notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));
                    }
                    else if(note.Type == NoteType.AboveChain) //チェインノーツは後ろに 25 の判定幅
                    {
                        if(currentTime - note.JudgeTime >= 0 || currentTime - note.JudgeTime <= 0.025f)
                        {
                            //Perfect
                            notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));

                            allJudgeType.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Perfect
                            });
                        }
                        else if(currentTime - note.JudgeTime > 0.025f)
                        {
                            notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));
                            allJudgeType.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Miss
                            });
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}

