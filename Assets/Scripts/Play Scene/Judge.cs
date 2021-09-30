#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Reilas;
using Rhythmium;

public enum JudgeResultType
{
    Perfect,
    Good,
    Bad,
    Miss,
    NotJudgedYet
}

public class JudgeResult
{
    public JudgeResultType resultType;
}

public class JudgeResultInHold
{
    public float time;
    public bool perfect;
}

public class JudgeService : MonoBehaviour
{
    
    private readonly Dictionary<string, float> _judgeSeconds = new Dictionary<string, float>()
    {
        {"Tap Perfect", 0.041f},
        {"Tap Good", 0.058f},
        {"Tap Bad", 0.075f},
        {"Internal", 0.090f},
        {"Chain", 0.025f}
    };

    private static float CalculateDifference(float currentTime, float judgeTime, string noteType)
    {
        return noteType switch
        {
            "Tap" => Math.Abs(currentTime - judgeTime),
            "Internal" => judgeTime - currentTime,
            "Chain" => currentTime - judgeTime,
            _ => throw new Exception()
        };
    }

    private bool TimeCheck(float currentTime, float judgeTime, string flag, string noteType)
    {
        var difference = CalculateDifference(currentTime, judgeTime, noteType);
        return difference >= 0 && difference <= _judgeSeconds[flag] || currentTime < judgeTime;
    }

    
    private JudgeResultType Tap(float currentTime, ReilasNoteEntity note, bool time, bool place)
    {
        var difference = CalculateDifference(currentTime, note.JudgeTime, "Tap");
        var timeCheck = TimeCheck(currentTime, note.JudgeTime, "Tap Bad", "Tap");
        if (time && place)
        {
            return difference switch
            {
                var dif when dif <= _judgeSeconds["Tap Perfect"] => JudgeResultType.Perfect,
                var dif when dif <= _judgeSeconds["Tap Good"] => JudgeResultType.Good,
                var dif when dif <= _judgeSeconds["Tap Bad"] => JudgeResultType.Bad,
                _ => timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss
            };
        }
        return timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss;
    }

    private JudgeResultType InternalOrChain(float currentTime, NoteEntity note, bool time, bool place, string internalOrChain)
    {
        var timeCheck = TimeCheck(currentTime, note.JudgeTime, internalOrChain, internalOrChain);
        if (time && place)
        {
            return timeCheck ? JudgeResultType.Perfect : JudgeResultType.Miss;
        }

        return timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss;
    }


    private void TapJudge(float currentTime, IEnumerable<ReilasNoteEntity> notes, bool time, bool place)
    {
        var reilasNoteEntities = notes as ReilasNoteEntity[] ?? notes.ToArray();
        for (int i = 0; i < reilasNoteEntities.Length; i++)
        {
            if (!_tapJudged[i])
            {
                Tap(currentTime, reilasNoteEntities[i], time, place);
                _tapJudged[i] = true;
            }
        }
        
    }

    private void HoldJudge(float currentTime, IEnumerable<ReilasNoteEntity> noteLines, bool time, bool place)
    {
        var reilasNoteEntities = noteLines as ReilasNoteEntity[] ?? noteLines.ToArray();
        for
    }
    /*
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

    }

     
    public static readonly List<JudgeResult> AllJudgeType = new List<JudgeResult>();//�������`����
    public static readonly List<JudgeResultInHold> JudgedInHold = new List<JudgeResultInHold>(); //�������m�[�c�̓����������`����


    private void RemoveNoteInList(List<int> delNums, List<List<float>> noteList)
    {
        for (int i = delNums.Count - 1; i >= 0; i--)
        {
            noteList.RemoveAt(delNums[i]);
        }
    }




    private void MissTapJudge(float currentTime, List<List<float>> notesList, bool above)
    {
        int limit = notesList.Count() - 1;

        for(int i = 0; i <= limit; i++)
        {
            if(notesList[i][0] - currentTime > -0.075f)
            {
                return;
            }
            else
            {
                Debug.Log(RhythmGamePresenter._tapNotes.Count());
                if(!above)
                {
                    AllJudgeType.Add(new JudgeResult
                    {
                        resultType = JudgeResultType.Miss
                    });
                    RhythmGamePresenter._tapNotes[0].NoteDestroy();
                    RhythmGamePresenter.notJudgedTapNotes.RemoveAt(0);
                    i--;
                    limit--;
                }
                else
                {
                    AllJudgeType.Add(new JudgeResult
                    {
                        resultType = JudgeResultType.Miss
                    });
                    RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
                    RhythmGamePresenter.notJudgedAboveTapNotes.RemoveAt(0);
                    i--;
                    limit--;
                }
            }
        }
    }

    void MissHoldJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach (List<float> note in notesList)
        {
            if (note[0] - currentTime > -0.075f)
            {
                return;
            }
            else
            {
                AllJudgeType.Add(new JudgeResult
                {
                    resultType = JudgeResultType.Miss
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
                JudgedInHold.Add(new JudgeResultInHold
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
        int limit = notesList.Count() - 1;
     
        for (int i = 0; i <= limit; i++)
        {
            if (notesList[i][0] - currentTime > -0.025f)
            {
                return;
            }
            else
            {
                AllJudgeType.Add(new JudgeResult
                {
                    resultType = JudgeResultType.Miss
                });
                RhythmGamePresenter._aboveChainNotes[0].NoteDestroy();
                RhythmGamePresenter.notJudgedAboveChainNotes.RemoveAt(0);
                i--;
                limit--;
            }
        }
    }
    void ChainJudge(List<List<float>> tapType, LaneTapState tapstate, float currentTime)
    {
        foreach (List<float> tap in tapType)
        {
            float orderNum = 0;
            List<float> _judgedIndex = new List<float>();
            if (tap[0] - currentTime >= -0.025f)
            {
                if (tap[0] - currentTime < 0f)
                {
                    if (tap[1] == 0)
                    {
                        if (4 <= tapstate.laneNumber && tapstate.laneNumber <= tap[2] + 4)
                        {
                            Debug.Log("Chainperfect");
                            _judgedIndex.Add(orderNum);
                        }
                    }
                    else
                    {
                        if (3 + tap[1] <= tapstate.laneNumber && tapstate.laneNumber <= 4 + tap[1] + tap[2])
                        {
                            Debug.Log("Chainperfect");
                            _judgedIndex.Add(orderNum);
                        }
                    }
                }
                else
                {
                    _judgedIndex.OrderByDescending(note => note);
                    foreach (float x in _judgedIndex)
                    {
                        tapType.RemoveAt((int)x);
                        RhythmGamePresenter._aboveChainNotes[(int) x].NoteDestroy();
                    }
                    break;
                }
            }
            orderNum++;
        }
    }

    void InternalJudge(bool isBelow, List<List<float>> tapType, LaneTapState tapstate, float currentTime, List<float> _judgeInternalNotes)
    {
        List<float> _judgedIndex = new List<float>();
        float orderNum = 0;
        foreach (List<float> tap in tapType)
        {
            if (tap[0] - currentTime <= 0.090f)
            {
                if (isBelow)
                {
                    if (tap[1] == tapstate.laneNumber)
                    {
                        _judgeInternalNotes.Add(tap[0]);
                        _judgedIndex.Add(orderNum);
                    }
                }
                else
                {
                    if (tap[1] == 0)
                    {
                        if (4 <= tapstate.laneNumber && tapstate.laneNumber <= tap[2] + 4)
                        {
                            _judgeInternalNotes.Add(tap[0]);
                            _judgedIndex.Add(orderNum);
                        }
                    }
                    else
                    {
                        if (3 + tap[1] <= tapstate.laneNumber && tapstate.laneNumber <= 4 + tap[1] + tap[2])
                        {
                            _judgeInternalNotes.Add(tap[0]);
                            _judgedIndex.Add(orderNum);
                        }
                    }
                }
            }
            else
            {
                _judgedIndex.OrderByDescending(note => note);
                foreach (float x in _judgedIndex)
                {
                    tapType.RemoveAt((int) x);
                }
                break;
            }
            orderNum++;
        }
    }



    List<float> _judgeInternalNotes = new List<float>();
    public void Judge(float currentTime, List<LaneTapState> tapStates)
    {
        MissTapJudge(currentTime, RhythmGamePresenter.notJudgedTapNotes, false);
        MissTapJudge(currentTime, RhythmGamePresenter.notJudgedAboveTapNotes, true);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedHoldNotes);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedAboveHoldNotes);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedAboveSlideNotes);
        MissInternalJudge(currentTime, RhythmGamePresenter.notJudgedInternalNotes);
        MissChainJudge(currentTime, RhythmGamePresenter.notJudgedAboveChainNotes);

        List<List<float>> _judgeTapNotes = new List<List<float>>();
       
        List<List<float>> _judgeChainNotes = new List<List<float>>();
        foreach (LaneTapState tapstate in tapStates)
        {
            List<List<float>> _tapNotes = new List<List<float>>();
            if (tapstate.tapStarting)
            {
                TimeJudge(true, 0, RhythmGamePresenter.notJudgedTapNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(false, 1, RhythmGamePresenter.notJudgedAboveTapNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(true, 2, RhythmGamePresenter.notJudgedHoldNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(false, 3, RhythmGamePresenter.notJudgedAboveHoldNotes, tapstate, currentTime, _judgeTapNotes);
                TimeJudge(false, 4, RhythmGamePresenter.notJudgedAboveSlideNotes, tapstate, currentTime, _judgeTapNotes);
            }
            if (_tapNotes.Count() != 0)
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

            ChainJudge(RhythmGamePresenter.notJudgedAboveChainNotes, tapstate, currentTime);

            InternalJudge(true, RhythmGamePresenter.notJudgedInternalNotes, tapstate, currentTime, _judgeInternalNotes);
            InternalJudge(false, RhythmGamePresenter.notJudgedAboveInternalNotes, tapstate, currentTime, _judgeInternalNotes);

        }

        _judgeTapNotes.OrderByDescending(note => note[2]).Distinct();
        _judgeInternalNotes.OrderByDescending(note => note).Distinct();

        if (_judgeTapNotes.Count() != 0)//タップ系判定
        {
            if (Mathf.Abs(_judgeTapNotes[0][0] - currentTime) <= 0.041f) // perfect
            {
                for (int a = 0; a < _judgeTapNotes.Count(); a++)//同じタイミングのノーツ分繰り返す
                {
                    AllJudgeType.Add(new JudgeResult
                    {
                        resultType = JudgeResultType.Perfect
                    });
                    Debug.Log("perfect");
                }
            }
            else if (Mathf.Abs(_judgeTapNotes[0][0] - currentTime) <= 0.058f) // good
            {
                for (int a = 0; a < _judgeTapNotes.Count(); a++)//同じタイミングのノーツ分繰り返す
                {
                    AllJudgeType.Add(new JudgeResult
                    {
                        resultType = JudgeResultType.Good
                    });
                    Debug.Log("good");
                }
            }
            else if (Mathf.Abs(_judgeTapNotes[0][0] - currentTime) <= 0.075f) // bad
            {
                for (int a = 0; a < _judgeTapNotes.Count(); a++)//同じタイミングのノーツ分繰り返す
                {
                    AllJudgeType.Add(new JudgeResult
                    {
                        resultType = JudgeResultType.Bad
                    });
                    Debug.Log("bad");
                }
            }

            foreach (List<float> _judgeTapNote in _judgeTapNotes)
            {
                switch (_judgeTapNote[1])
                {
                    case 0: RhythmGamePresenter.notJudgedTapNotes.RemoveAt((int)_judgeTapNote[2]); RhythmGamePresenter._tapNotes[(int)_judgeTapNote[2]].NoteDestroy(); break;
                    case 1: RhythmGamePresenter.notJudgedAboveTapNotes.RemoveAt((int)_judgeTapNote[2]); RhythmGamePresenter._aboveTapNotes[(int)_judgeTapNote[2]].NoteDestroy(); break;
                    case 2: RhythmGamePresenter.notJudgedHoldNotes.RemoveAt((int)_judgeTapNote[2]); break;
                    case 3: RhythmGamePresenter.notJudgedTapNotes.RemoveAt((int)_judgeTapNote[2]); break;
                    case 4: RhythmGamePresenter.notJudgedTapNotes.RemoveAt((int)_judgeTapNote[2]); break;
                }
            }
        }

        for (int a = _judgeInternalNotes.Count() - 1; a >= 0; a--)//internal系判定
        {
            if (_judgeInternalNotes[a] == currentTime)
            {
                Debug.Log("Internalperfect");
                _judgeInternalNotes.RemoveAt(a);
            }

        }

    }*/

}
