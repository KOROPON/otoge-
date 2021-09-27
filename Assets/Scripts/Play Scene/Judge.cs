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
/// ���茋��
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



/// <summary>
/// �������s���T�[�r�X
/// </summary>
public class JudgeService : MonoBehaviour
{
    public static List<JudgeResult> allJudgeType = new List<JudgeResult>();//�������`����
    public static List<JudgeResultInHold> judgedInHold = new List<JudgeResultInHold>(); //�������m�[�c�̓����������`����

    public static string aa = null!;
    static int per;
    static int good;
    static int bad;
    static int miss;

    
    bool TapJudgeSystem(float noteTime, float currentTime)
    {

        if (Mathf.Abs(noteTime - currentTime) <= 0.041f) // perfect
        {
            Debug.Log("perfect");
        }
        else if (Mathf.Abs(noteTime - currentTime) <= 0.058f) // good
        {
            Debug.Log("good");
        }
        else if (Mathf.Abs(noteTime - currentTime) <= 0.075f) // bad
        {
            Debug.Log("bad");
        }
        else
        {
            return true;
        }

        return false;
    }

    bool InternalJudgeSystem(float noteTime, float currentTime)
    {

        if (noteTime - currentTime <= 0.090f)
        {
            Debug.Log("perfect");
            judgedInHold.Add(new JudgeResultInHold
            {
                time = noteTime,
                perfect = true
            });
        }
        else
        {
            return true;
        }

        return false;
    }

    void RemoveNoteInList(List<int> delNums, List<List<float>> noteList)
    {
        for (int i = delNums.Count - 1; i >= 0; i--)
        {
            noteList.RemoveAt(delNums[i]);
        }
    }


    List<float> TapJudge(List<List<float>> tapList, float currentTime, List<LaneTapState> lanetapStates, NoteType type)
    {
        float laneNumMin;
        float laneNumMax;
        List<int> allDelNums = new List<int>();
        foreach (LaneTapState tapstate in lanetapStates)
        {
            allDelNums.Clear();
            int delNum = 0;
            foreach (List<float> list in tapList)
            {
                delNum++;

                ///<summary>
                /// レーン情報の取得
                ///</summary>
                if (type == NoteType.Tap) //下のレーン
                {
                    laneNumMin = list[1];
                    laneNumMax = list[1];
                }
                else    //上のレーン
                {
                    if (list[1] == 0)
                    {
                        laneNumMin = 4;
                    }
                    else
                    {
                        laneNumMin = list[1] + 3;
                    }
                    laneNumMax = list[1] + list[2] + 3;
                }

                if (laneNumMin <= tapstate.laneNumber && tapstate.laneNumber <= laneNumMax && tapstate.tapStarting)
                {
                    if (TapJudgeSystem(list[0], currentTime))
                    {
                        //判定なし
                        continue;
                    }

                    allDelNums.Add(delNum); // tapList , RhythmGamePresenter._***Notes  から delNum 番目の要素を削除
                }
            }

            RemoveNoteInList(allDelNums, tapList); // tapList からDelete

            if (type == NoteType.Tap)
            {
                for (int i = allDelNums.Count - 1; i >= 0; i--)
                {
                    RhythmGamePresenter._tapNotes.RemoveAt(allDelNums[i]);
                }
            }
            else if (type == NoteType.AboveTap)
            {
                for (int i = allDelNums.Count - 1; i >= 0; i--)
                {
                    RhythmGamePresenter._aboveTapNotes.RemoveAt(allDelNums[i]);
                }
            }
        }
    }

    private void InternalJudge(List<List<float>> internalList, float currentTime, List<LaneTapState> laneTapStates, NoteType type)
    {

        float laneNumMin;
        float laneNumMax;
        List<int> allDelNums = new List<int>();
        foreach (LaneTapState tapstate in laneTapStates)
        {
            allDelNums.Clear();
            int delNum = 0;
            foreach (List<float> list in internalList)
            {
                delNum++;

                ///<summary>
                /// レーン情報の取得
                ///</summary>
                if (type == NoteType.Hold) //下のレーン
                {
                    laneNumMin = list[1];
                    laneNumMax = list[1];
                }
                else    //上のレーン
                {
                    if (list[1] == 0)
                    {
                        laneNumMin = 4;
                    }
                    else
                    {
                        laneNumMin = list[1] + 3;
                    }
                    laneNumMax = list[1] + list[2] + 3;
                }

                if (laneNumMin <= tapstate.laneNumber && tapstate.laneNumber <= laneNumMax)
                {
                    if (InternalJudgeSystem(list[0], currentTime))
                    {
                        //判定なし
                        continue;
                    }

                    allDelNums.Add(delNum); // internalList , RhythmGamePresenter._***Notes  から delNum 番目の要素を削除
                }
            }
            
            RemoveNoteInList(allDelNums, internalList); // internalList からDelete
        }
    }

    void ChainJudge(List<List<float>> chainList, float currentTime, List<LaneTapState> laneTapStates, NoteType type)
    {
        float laneNumMin;
        float laneNumMax;
        List<int> allDelNums = new List<int>();
        foreach (LaneTapState tapstate in laneTapStates)
        {
            allDelNums.Clear();
            int delNum = 0;
            foreach (List<float> list in chainList)
            {
                delNum++;

                ///<summary>
                /// レーン情報の取得
                ///</summary>
                if (type == NoteType.Tap) //下のレーン
                {
                    laneNumMin = list[1];
                    laneNumMax = list[1];
                }
                else    //上のレーン
                {
                    if (list[1] == 0)
                    {
                        laneNumMin = 4;
                    }
                    else
                    {
                        laneNumMin = list[1] + 3;
                    }
                    laneNumMax = list[1] + list[2] + 3;
                }

                if (laneNumMin <= tapstate.laneNumber && tapstate.laneNumber <= laneNumMax && tapstate.tapStarting)
                {
                    if (currentTime - list[0] <= 0.025f)
                    {
                        Debug.Log("perfect");
                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Perfect
                        });
                    }
                    else
                    {
                        // 判定なし
                        continue;
                    }

                    allDelNums.Add(delNum); // tapList , RhythmGamePresenter._***Notes  から delNum 番目の要素を削除
                }
            }

            RemoveNoteInList(allDelNums, chainList); // tapList からDelete

            if (type == NoteType.Tap)
            {
                for (int i = allDelNums.Count - 1; i >= 0; i--)
                {
                    RhythmGamePresenter._aboveChainNotes.RemoveAt(allDelNums[i]);
                }
            }
        }
    }

    void MissTapJudge(float currentTime, List<List<float>> notesList, NoteType type)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach(List<float> note in notesList)
        {
            if(note[0] - currentTime > -0.075f)
            {
                return;
            }
            else
            {
                if(type == NoteType.Tap)
                {
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    RhythmGamePresenter._tapNotes[0].NoteDestroy();
                }
                else
                {
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
                }
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }

    void MissHoldJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach(List<float> note in notesList)
        {
            if(note[0] - currentTime > -0.075f)
            {
                return;
            }
            else
            {
                allJudgeType.Add(new JudgeResult
                {
                    ResultType = JudgeResultType.Miss
                });
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }

    void MissInternalJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach (List<float> note in notesList)
        {
            if (note[0] - currentTime > 0)
            {
                return;
            }
            else
            {
                judgedInHold.Add(new JudgeResultInHold
                {
                    time = note[0],
                    perfect = false
                });
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }

    void MissChainJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach (List<float> note in notesList)
        {
            if (note[0] - currentTime > -0.025f)
            {
                return;
            }
            else
            {
                allJudgeType.Add(new JudgeResult
                {
                    ResultType = JudgeResultType.Miss
                });
                RhythmGamePresenter._aboveChainNotes[0].NoteDestroy();
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }


    public void Judge(float currentTime, List<LaneTapState> tapStates)
    {
        MissTapJudge(currentTime, RhythmGamePresenter.notJudgedTapNotes, NoteType.Tap);
        MissTapJudge(currentTime, RhythmGamePresenter.notJudgedAboveTapNotes, NoteType.AboveTap);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudegedHoldNotes);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedAboveHoldNotes);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedAboveSlideNotes);
        MissInternalJudge(currentTime, RhythmGamePresenter.notJudgedInternalNotes);
        MissChainJudge(currentTime, RhythmGamePresenter.notJudegedAboveChainNotes);

        List<List<float>> _judgeTapNotes = new List<List<float>>();
        List<List<float>> _judgeInternalNotes = new List<List<float>>();
        List<List<float>> _judgeChainNotes = new List<List<float>>();
        foreach (LaneTapState tapstate in tapStates)
        {
            List<List<float>> _tapNotes = new List<List<float>>();
            if (tapstate.tapStarting)
            {
                TimeJudge(true, 0, notJudgedTapNotes, tapstate, currentTime ,_judgeTapNotes);
                TimeJudge(false, 1, notJudgedAboveTapNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(true, 2, notJudgedHoldNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(false, 3, notJudgedAboveHoldNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(false, 4, notJudgedAboveSlideNotes, tapstate, currentTime, _judgeTapNotes);
            }
            if (_tapNotes != null)
            {
                _tapNotes.OrderBy(tap => tap[0]);
                for (int a = 0; a < 6; a++)
                {
                    if (_tapNotes[0][0] == _tapNotes[a][0])
                    {
                        _judgeTapNotes.Add(_tapNotes[a]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            InternalJudge(true, notJudgedInternalNotes, tapstate, currentTime);
            InternalJudge(false, notJudgedAboveInternalNotes, tapstate, currentTime);

        }

        _judgeTapNotes.OrderByDescending(note => note[2]).Distinct;
        _judgeTapNotes.OrderByDescending(note => note[1]).Distinct;

        if (_judgeTapNotes != null)//タップ系判定
        {
            if (Mathf.Abs(_judgeTapNotes[0][0] - currentTime) <= 0.041f) // perfect
            {
                for (int a; a < _judgeTapNotes.Count(); a++)//同じタイミングのノーツ分繰り返す
                {
                    Debug.Log("perfect");
                }
            }
            else if (Mathf.Abs(_judgeTapNotes[0][0] - currentTime) <= 0.058f) // good
            {
                for (int a; a < _judgeTapNotes.Count(); a++)//同じタイミングのノーツ分繰り返す
                {
                    Debug.Log("good");
                }
            }
            else if (Mathf.Abs(_judgeTapNotes[0][0] - currentTime) <= 0.075f) // bad
            {
                for (int a; a < _judgeTapNotes.Count(); a++)//同じタイミングのノーツ分繰り返す
                {
                    Debug.Log("bad");
                }
            }
            
            foreach (List<float> _judgeTapNote in _judgeTapNotes)
            {
                switch (_judgeTapNote[1])
                {
                    case 0: notJudgedTapNotes.RemoveAt(_judgeTapNote[2]); RhythmGamePresenter._tapNotes[(int) _judgeTapNote[2]].NoteDestroy();break;
                    case 1: notJudgedAboveTapNotes.RemoveAt(_judgeTapNote[2]); RhythmGamePresenter._aboveTapNotes [(int)_judgeTapNote[2]].NoteDestroy();break;
                    case 2: notJudgedHoldNotes.RemoveAt(_judgeTapNote[2]); break;
                    case 3: notJudgedTapNotes.RemoveAt(_judgeTapNote[2]); break;
                    case 4: notJudgedTapNotes.RemoveAt(_judgeTapNote[2]); break;
                }
            }
        }
        
        for (int a = _judgeInternalNotes.Count() - 1; a >= 0; a--)//internal系判定
        {
            if (_judgeInternalNotes[a][0] == currentTime)
            {
                Debug.Log("perfect");
                _judgeInternalNotes.RemoveAt(a);
            }
         
        }
        
    }

    private void TimeJudge(bool isBelow, float typeNum, List<List<float>> tapType, LaneTapState tapstate, float currentTime, List<List<float>> _tapNotes)
    {
        bool isJudged = false;
        float judgeTime = 0;
        float orderNum = 0;
        foreach (List<float> tap in tapType)
        {
            if (tap[0] - currentTime <= 0.075f)
            {
                if (!isJudged)
                {
                    if (isBelow)
                    {
                        if (tap[1] == tapstate.laneNumber)
                        {
                            _tapNotes.Add(new List<float>()
                            {
                                tap[0],
                                typeNum,
                                orderNum
                            });
                            isJudged = true;
                            judgeTime = tap[0];
                        }
                    }
                    else
                    {
                        if (tap[1] == 0)
                        {
                            if (4 <= tapstate.laneNumber && tapstate.laneNumber <= tap[2] + 4)
                            {
                                _tapNotes.Add(new List<float>()
                                {
                                    tap[0],
                                    typeNum,
                                    orderNum
                                });
                                isJudged = true;
                                judgeTime = tap[0];
                            }
                        }
                        else
                        {
                            if (3 + tap[1] <= tapstate.laneNumber && tapstate.laneNumber <= 4 + tap[1] + tap[2])
                            {
                                _tapNotes.Add(new List<float>()
                                {
                                    tap[0],
                                    typeNum,
                                    orderNum
                                });
                                isJudged = true;
                                judgeTime = tap[0];
                            }
                        }
                    }
                }
                else
                {
                    if (tap[1] == judgeTime)
                    {
                        if (isBelow)
                        {
                            if (tap[1] == tapstate.laneNumber)
                            {
                                _tapNotes.Add(new List<float>()
                                {
                                    tap[0],
                                    typeNum,
                                    orderNum
                                });
                            }
                        }
                        else
                        {
                            if (tap[1] == 0)
                            {
                                if (4 <= tapstate.laneNumber && tapstate.laneNumber <= tap[2] + 4)
                                {
                                    _tapNotes.Add(new List<float>()
                                    {
                                    tap[0],
                                    typeNum,
                                    orderNum
                                    });
                                }
                            }
                            else
                            {
                                if (3 + tap[1] <= tapstate.laneNumber && tapstate.laneNumber <= 4 + tap[1] + tap[2])
                                {
                                    _tapNotes.Add(new List<float>()
                                    {
                                    tap[0],
                                    typeNum,
                                    orderNum
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                break;
            }
            orderNum++;
        }

        void ChainJudge(List<List<float>> tapType, LaneTapState tapstate, float currentTime)
        {
            foreach (List<float> tap in tapType)
            {
                if (0 <= currentTime - tap[0] && currentTime - tap[0] <= 0.025)
                {
                    if (tap[1] == 0)
                    {
                        if (4 <= tapstate.laneNumber && tapstate.laneNumber <= tap[2] + 4)
                        {
                            Debug.Log("perfect");


                        }
                    }
                    else
                    {
                        if (3 + tap[1] <= tapstate.laneNumber && tapstate.laneNumber <= 4 + tap[1] + tap[2])
                        {
                            Debug.Log("perfect");
                        }
                    }
                }
                else if (currentTime - tap[0] <= 0)
                {
                    break;
                }
            }

        }

        void InternalJudge(bool isBelow, List<List<float>> tapType, LaneTapState tapstate, float currentTime)
        {
            float orderNum;
            foreach (List<float> tap in tapType)
            {
                if (tap[0] - currentTime <= 0.090f)
                {
                    {
                        if (isBelow)
                        {
                            if (tap[1] == tapstate.laneNumber)
                            {
                                _judgeinternalNotes.Add(tap[0], orderNum);

                            }
                        }
                        else
                        {
                            if (tap[1] == 0)
                            {
                                if (4 <= tapstate.laneNumber && tapstate.laneNumber <= tap[2] + 4)
                                {
                                    _internalNotes.Add(tap[0], orderNum);
                                    tapType.Remove(tap);
                                }
                            }
                            else
                            {
                                if (3 + tap[1] <= tapstate.laneNumber && tapstate.laneNumber <= 4 + tap[1] + tap[2])
                                {
                                    _internalNotes.Add(tap[0], orderNum);
                                    tapType.Remove(tap);
                                }
                            }
                        }

                    }

                }
                else
                {
                    break;
                }
                orderNum++;
            }
        }
    }
}
