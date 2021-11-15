#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
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

public class AllJudgeService : MonoBehaviour
{
    public int[] tapJudgeStartIndex = new int[36];
    public int internalJudgeStartIndex;
    public int chainJudgeStartIndex;
    private RhythmGamePresenter? _gamePresenter;
    private JudgeRankEffector _judgeRankEffector;
    private JudgeEffector _judgeEffector;

    public bool _alreadyChangeKujo = false;

    public static readonly List<JudgeResultType> AllJudge = new List<JudgeResultType>();

    public void JudgeStart()
    {
        _judgeRankEffector = GameObject.Find("JudgeRank").GetComponent<JudgeRankEffector>();
        _judgeEffector = GameObject.Find("Effectors").GetComponent<JudgeEffector>();
        Debug.Log("awake");
        _gamePresenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }

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

    private bool TimeCheck(float currentTime, float judgeTime, string noteType)
    {
        var difference = CalculateDifference(currentTime, judgeTime, noteType);
        return noteType switch
        {
            "Tap" => difference <= _judgeSeconds[noteType + " Bad"],
            "Internal" => currentTime <= judgeTime && difference <= _judgeSeconds[noteType],
            "Chain" => currentTime >= judgeTime && difference <= _judgeSeconds[noteType],
            _ => false
        };
    }

    private JudgeResultType InternalOrChain(float currentTime, NoteEntity note, bool tapState, string internalOrChain)
    {
        var timeCheck = TimeCheck(currentTime, note.JudgeTime, internalOrChain);
        if (tapState) return timeCheck ? JudgeResultType.Perfect : JudgeResultType.Miss;
        return timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss;
    }

    private static bool CheckType(ReilasNoteEntity note, string noteType)
    {
        return RhythmGamePresenter.CheckType(note, noteType);
    }


    private static int GetLane(ReilasNoteEntity note)
    {
        return RhythmGamePresenter.GetLane(note);
    }

    private static bool CheckIfTapped(ReilasNoteEntity note)
    {
        var tapState = RhythmGamePresenter.LaneTapStates;
        var noteLanePosition = GetLane(note);
        switch (noteLanePosition)
        {
            case var lane when lane < 4:
                {
                    if (tapState[lane, 0]) return true;
                    break;
                }
            case 4:
                {
                    for (var i = noteLanePosition; i < noteLanePosition + note.Size && i < 36; i++) if (tapState[i, 0]) return true;
                    break;
                }
            default:
                {
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.Size && i < 36; i++)
                    {
                        if (tapState[i, 0]) return true;
                    }

                    break;
                }
        }

        return false;
    }

    private static List<int> GetTapState(ReilasNoteEntity note)
    {
        var tapState = RhythmGamePresenter.LaneTapStates;
        var noteLanePosition = GetLane(note);
        if (noteLanePosition < 4) return tapState[noteLanePosition, 0] && tapState[noteLanePosition, 1] ? new List<int> { note.LanePosition } : new List<int>();

        var laneList = new List<int>();
        switch (noteLanePosition)
        {
            case 4:
                {
                    for (var i = noteLanePosition; i < noteLanePosition + note.Size && i < 36; i++) if (tapState[i, 0] && tapState[i, 1]) laneList.Add(i);

                    return laneList;
                }
            default:
                {
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.Size && i < 36; i++) if (tapState[i, 0] && tapState[i, 1]) laneList.Add(i);

                    return laneList;
                }
        }
    }

    public void Judge(float currentTime)
    {
        if (_gamePresenter == null) return;

        var tapNotes = RhythmGamePresenter.TapNoteLanes;
        if (_gamePresenter.alreadyChangeKujo && _gamePresenter.jumpToKujo)
        {
            tapNotes = RhythmGamePresenter.TapKujoNoteLanes;
        }

        Debug.Log(tapNotes.Length);
        for (var i = 0; i < tapNotes.Length; i++)
        {
            //Debug.Log("i:" + i + "  tapJudgeStartIndex" + tapJudgeStartIndex.Length + "tapNotes" + tapNotes.Length);
            var notJudgedYet = true;
            //Debug.Log(tapJudgeStartIndex[0]);
            //Debug.Log(tapNotes[0]);////// null
            if (tapJudgeStartIndex == null) continue;
            if (tapNotes == null)
            {
                continue;
            }
            var a = tapNotes[i];
            var b = tapJudgeStartIndex[i];
            if (tapNotes[i] == null) Debug.Log(tapNotes.Length + "  " + i + "�Ԗ�");
            for (var j = tapJudgeStartIndex[i]; j < tapNotes[i].Count; j++)
            {
                var note = tapNotes[i][j];
                if (note.hasBeenTapped) continue;
                JudgeResultType judgeResult;
                var reilasNoteEntity = note.note;
                var timeDifference = reilasNoteEntity.JudgeTime - currentTime;
                if (timeDifference > _judgeSeconds["Tap Bad"])
                {
                    break;
                }
                var difference = CalculateDifference(currentTime, reilasNoteEntity.JudgeTime, "Tap");
                var timeCheck = TimeCheck(currentTime, reilasNoteEntity.JudgeTime, "Tap");

                if (GetTapState(reilasNoteEntity).Contains(i) && notJudgedYet)
                {
                    var nextNoteIndex = j + 1;
                    if (nextNoteIndex != tapNotes[i].Count &&
                        timeDifference < currentTime - tapNotes[i][nextNoteIndex].note.JudgeTime)
                    {
                        judgeResult = JudgeResultType.Miss;
                        _judgeRankEffector.JudgeRankDisplay("miss");
                    }
                    else
                    {
                        int lanePos = tapNotes[i][j].note.LanePosition + (int)Mathf.Floor(tapNotes[i][j].note.Size / 2);
                        switch (difference)
                        {
                            case var dif when dif <= _judgeSeconds["Tap Perfect"]:
                                {
                                    judgeResult = JudgeResultType.Perfect;
                                    _judgeEffector.TapJudgeEffector(lanePos, "Perfect");
                                    _judgeRankEffector.JudgeRankDisplay("perfect");
                                    break;
                                }
                            case var dif when dif <= _judgeSeconds["Tap Good"]:
                                {
                                    judgeResult = JudgeResultType.Good;
                                    _judgeEffector.TapJudgeEffector(lanePos, "Good");
                                    _judgeRankEffector.JudgeRankDisplay("good");
                                    break;
                                }
                            case var dif when dif <= _judgeSeconds["Tap Bad"]:
                                {
                                    judgeResult = JudgeResultType.Bad;
                                    _judgeEffector.TapJudgeEffector(lanePos, "Bad");
                                    _judgeRankEffector.JudgeRankDisplay("bad");
                                    break;
                                }
                            default:
                                {
                                    if (timeCheck)
                                    {
                                        judgeResult = JudgeResultType.NotJudgedYet;
                                    }
                                    else
                                    {
                                        _judgeRankEffector.JudgeRankDisplay("miss");
                                        judgeResult = JudgeResultType.Miss;
                                    }
                                    break;
                                }
                        }
                    }

                    notJudgedYet = false;
                }
                else
                {
                    if (timeCheck)
                    {
                        judgeResult = JudgeResultType.NotJudgedYet;
                    }
                    else
                    {
                        _judgeRankEffector.JudgeRankDisplay("miss");
                        judgeResult = JudgeResultType.Miss;
                    }
                }
                if (judgeResult == JudgeResultType.NotJudgedYet) continue;
                AllJudge.Add(judgeResult);
                note.hasBeenTapped = true;
                if (CheckType(reilasNoteEntity, "AboveTap"))
                {
                    if (_alreadyChangeKujo) RhythmGamePresenter.AboveKujoTapNotes[0].NoteDestroy(true);
                    else RhythmGamePresenter.AboveTapNotes[0].NoteDestroy(false);
                }
                else
                {
                    if (_alreadyChangeKujo) RhythmGamePresenter.TapKujoNotes[0].NoteDestroy(true);
                    else RhythmGamePresenter.TapNotes[0].NoteDestroy(false);
                }
                tapJudgeStartIndex[i]++;
            }
        }

        var internalNotes = RhythmGamePresenter.internalNotes;
        if (_gamePresenter.alreadyChangeKujo && _gamePresenter.jumpToKujo)
        {
            internalNotes = RhythmGamePresenter.InternalKujoNotes;
        }

        for (var i = internalJudgeStartIndex; i < internalNotes.Count; i++)
        {
            if (RhythmGamePresenter.internalNoteJudge == null)
            {
                Debug.LogError("Can't Judge Internal");
                break;
            }

            if (RhythmGamePresenter.internalNoteJudge[i]) continue;
            var timeDifference = internalNotes[i].JudgeTime - currentTime;
            ////here
            //Debug.Log(timeDifference);
            if (timeDifference > _judgeSeconds["Internal"]) break;

            //Debug.Log(CheckIfTapped(internalNotes[i]));

            var judgeResult = InternalOrChain(currentTime, internalNotes[i], CheckIfTapped(internalNotes[i]), "Internal");
            if (judgeResult == JudgeResultType.NotJudgedYet) continue;
            AllJudge.Add(judgeResult);
            RhythmGamePresenter.internalNoteJudge[i] = true;
            internalJudgeStartIndex++;

            if (judgeResult == JudgeResultType.Miss)
            {
                _judgeRankEffector.JudgeRankDisplay("miss");
                Debug.Log("Miss...");
                continue;
            }

            _judgeRankEffector.JudgeRankDisplay("perfect");
        }

        var chainNotes = RhythmGamePresenter.chainNotes;
        if (_gamePresenter.alreadyChangeKujo && _gamePresenter.jumpToKujo)
        {
            chainNotes = RhythmGamePresenter.ChainKujoNotes;
        }

        for (var i = chainJudgeStartIndex; i < chainNotes.Count; i++)
        {
            if (RhythmGamePresenter.chainNoteJudge == null)
            {
                Debug.LogError("Can't Judge Chain");
                break;
            }
            if (RhythmGamePresenter.chainNoteJudge[i]) continue;
            var timeDifference = chainNotes[i].JudgeTime - currentTime;
            if (timeDifference > 0) break;
            var judgeResult = InternalOrChain(currentTime, chainNotes[i], CheckIfTapped(chainNotes[i]), "Chain");
            if (judgeResult == JudgeResultType.NotJudgedYet) continue;
            AllJudge.Add(judgeResult);
            if (judgeResult == JudgeResultType.Perfect)
            {
                Debug.Log("ChainPerfect");
                _judgeEffector.TapJudgeEffector(chainNotes[i].LanePosition, "Perfect");
                _judgeRankEffector.JudgeRankDisplay("perfect");
            }
            else
            {
                Debug.Log("ChainMiss");
                _judgeRankEffector.JudgeRankDisplay("miss");
            }
            if (RhythmGamePresenter.chainNoteJudge != null) RhythmGamePresenter.chainNoteJudge[i] = true;
            if (_alreadyChangeKujo) RhythmGamePresenter.AboveKujoChainNotes[0].NoteDestroy(true);
            else RhythmGamePresenter.AboveChainNotes[0].NoteDestroy(false);
            chainJudgeStartIndex++;
        }
    }
}
    