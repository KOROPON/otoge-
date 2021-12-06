#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Reilas;
using Rhythmium;
using System.Linq;

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
    
    private RhythmGamePresenter _gamePresenter = null!;
    private JudgeRankEffector _judgeRankEffector = null!;
    private JudgeEffector _judgeEffector = null!;

    public bool alreadyChangeKujo;

    public static readonly List<JudgeResultType> AllJudge = new List<JudgeResultType>();
    
    private readonly Dictionary<string, float> _judgeSeconds = new Dictionary<string, float>()
    {
        {"Tap Perfect", 0.060f},
        {"Tap Good", 0.105f},
        {"Tap Bad", 0.150f},
        {"Internal", 0.090f},
        {"Chain", 0.060f}
    };

    public void JudgeStart()
    {
        _judgeRankEffector = GameObject.Find("JudgeRank").GetComponent<JudgeRankEffector>();
        _judgeEffector = GameObject.Find("Effectors").GetComponent<JudgeEffector>();
        _gamePresenter = GameObject.Find("Main").GetComponent<RhythmGamePresenter>();
    }
    
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
                    for (var i = noteLanePosition; i < noteLanePosition + note.Size && i < 36; i++)
                        if (tapState[i, 0])
                            return true;
                    
                    break;
                }
            default:
                {
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.Size && i < 36; i++)
                        if (tapState[i, 0])
                            return true;

                    break;
                }
        }

        return false;
    }

    private static List<int> GetTapState(ReilasNoteEntity note)
    {
        var tapState = RhythmGamePresenter.LaneTapStates;
        var noteLanePosition = GetLane(note);

        if (noteLanePosition < 4)
            return tapState[noteLanePosition, 0] && tapState[noteLanePosition, 1]
                ? new List<int> {note.LanePosition}
                : new List<int>();

        var laneList = new List<int>();
        
        switch (noteLanePosition)
        {
            case 4:
                {
                    for (var i = noteLanePosition; i < noteLanePosition + note.Size && i < 36; i++)
                        if (tapState[i, 0] && tapState[i, 1])
                            laneList.Add(i);

                    return laneList;
                }
            default:
                {
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.Size && i < 36; i++)
                        if (tapState[i, 0] && tapState[i, 1])
                            laneList.Add(i);

                    return laneList;
                }
        }
    }

    public void Judge(float currentTime)
    {
        if (_gamePresenter == null) return;

        var tapNotes = RhythmGamePresenter.TapNoteLanes;

        if (_gamePresenter.alreadyChangeKujo && RhythmGamePresenter.jumpToKujo)
            tapNotes = RhythmGamePresenter.TapKujoNoteLanes;
                
        for (var i = 0; i < 36; i++)
        {
            var notJudgedYet = true;

            if (tapJudgeStartIndex == null || tapNotes == null) continue;

            for (var j = tapJudgeStartIndex[i]; j < tapNotes[i].Count; j++)
            {
                var note = tapNotes[i][j];
                
                if (note.hasBeenTapped) continue;
                
                var reilasNoteEntity = note.note;
                var timeDifference = reilasNoteEntity.JudgeTime - currentTime;
                
                if (timeDifference > _judgeSeconds["Tap Bad"]) break;
                
                var difference = CalculateDifference(currentTime, reilasNoteEntity.JudgeTime, "Tap");
                var timeCheck = TimeCheck(currentTime, reilasNoteEntity.JudgeTime, "Tap");

                JudgeResultType judgeResult = JudgeResultType.NotJudgedYet;
                
                if (GetTapState(reilasNoteEntity).Contains(i) && notJudgedYet)
                {
                    var nextNoteIndex = j + 1;
                    
                    if (nextNoteIndex != tapNotes[i].Count &&
                        timeDifference < currentTime - tapNotes[i][nextNoteIndex].note.JudgeTime)
                    {
                        judgeResult = JudgeResultType.Miss;
                        if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("miss");
                    }
                    else
                    {
                        var noteEntity = tapNotes[i][j].note;
                        
                        if (noteEntity != null)
                        {
                            // ReSharper disable once PossibleLossOfFraction
                            var lanePos = noteEntity.LanePosition + (int)Mathf.Floor(noteEntity.Size / 2);
                            
                            switch (difference)
                            {
                                case var dif when dif <= _judgeSeconds["Tap Perfect"]:
                                {
                                    judgeResult = JudgeResultType.Perfect;
                                    
                                    if (_judgeEffector != null) _judgeEffector.TapJudgeEffector(lanePos, "Perfect");
                                    
                                    if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("perfect");
                                    
                                    break;
                                }
                                case var dif when dif <= _judgeSeconds["Tap Good"]:
                                {
                                    judgeResult = JudgeResultType.Good;

                                    if (_judgeEffector != null) _judgeEffector.TapJudgeEffector(lanePos, "Good");

                                    if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("good");
                                    
                                    break;
                                }
                                case var dif when dif <= _judgeSeconds["Tap Bad"]:
                                {
                                    judgeResult = JudgeResultType.Bad;

                                    if (_judgeEffector != null) _judgeEffector.TapJudgeEffector(lanePos, "Bad");

                                    if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("bad");
                                    
                                    break;
                                }
                                default:
                                {
                                    if (timeCheck) judgeResult = JudgeResultType.NotJudgedYet;
                                    else
                                    {
                                        if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("miss");
                                        
                                        judgeResult = JudgeResultType.Miss;
                                    }
                                    
                                    break;
                                }
                            }
                        }
                    }

                    notJudgedYet = false;
                }
                else
                {
                    if (timeCheck) judgeResult = JudgeResultType.NotJudgedYet;
                    else
                    {
                        if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("miss");
                        
                        judgeResult = JudgeResultType.Miss;
                    }
                }
                
                if (judgeResult == JudgeResultType.NotJudgedYet) continue;
                
                AllJudge.Add(judgeResult);
                
                note.hasBeenTapped = true;
                
                if (CheckType(reilasNoteEntity, "AboveTap"))
                {
                    if (alreadyChangeKujo) RhythmGamePresenter.AboveKujoTapNotes[0].NoteDestroy(true);
                    else RhythmGamePresenter.AboveTapNotes[0].NoteDestroy(false);
                }
                else
                {
                    if (alreadyChangeKujo) RhythmGamePresenter.TapKujoNotes[0].NoteDestroy(true);
                    else RhythmGamePresenter.TapNotes[0].NoteDestroy(false);
                }
                
                tapJudgeStartIndex[i]++;
            }
        }

        var internalNotes = RhythmGamePresenter.internalNotes.OrderBy(note => note.JudgeTime).ToList();

        if (_gamePresenter.alreadyChangeKujo && RhythmGamePresenter.jumpToKujo)
            internalNotes = new List<ReilasNoteEntity>(RhythmGamePresenter.InternalKujoNotes);

        for (var i = internalJudgeStartIndex; i < internalNotes.Count; i++)
        {
            var timeDifference = internalNotes[i].JudgeTime - currentTime;
            
            //here
            if (timeDifference > _judgeSeconds["Internal"]) break;

            var judgeResult =
                InternalOrChain(currentTime, internalNotes[i], CheckIfTapped(internalNotes[i]), "Internal");

            if (judgeResult == JudgeResultType.NotJudgedYet) continue;
            
            AllJudge.Add(judgeResult);
            internalJudgeStartIndex++;

            if (judgeResult == JudgeResultType.Miss)
            {
                if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("miss");
                continue;
            }

            if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("perfect");
        }

        var chainNotes = RhythmGamePresenter.chainNotes;

        if (_gamePresenter.alreadyChangeKujo && RhythmGamePresenter.jumpToKujo)
            chainNotes = RhythmGamePresenter.ChainKujoNotes;

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
                if (_judgeEffector != null)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    _judgeEffector.TapJudgeEffector(
                        chainNotes[i].LanePosition + (int) Mathf.Floor(chainNotes[i].Size / 2), "Perfect");
                }
                
                if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("perfect");
            }
            else if (_judgeRankEffector != null) _judgeRankEffector.JudgeRankDisplay("miss");

            RhythmGamePresenter.chainNoteJudge[i] = true;
            
            if (alreadyChangeKujo) RhythmGamePresenter.AboveKujoChainNotes[0].NoteDestroy(true);
            else RhythmGamePresenter.AboveChainNotes[0].NoteDestroy(false);
            
            chainJudgeStartIndex++; 
        }
    }
}
    