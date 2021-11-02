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

public class JudgeService : MonoBehaviour
{
    public int[] tapJudgeStartIndex = new int[36];
    public int internalJudgeStartIndex;
    public int chainJudgeStartIndex;

    public static readonly List<JudgeResultType> AllJudge = new List<JudgeResultType>();

    private readonly Dictionary<string, float> _judgeSeconds = new Dictionary<string, float>()
    {
        {"Tap Perfect", 0.041f},
        {"Tap Good", 0.058f},
        {"Tap Bad", 0.5f},
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
        if (noteLanePosition < 4) return tapState[noteLanePosition, 0] && tapState[noteLanePosition, 1] ? new List<int> {note.LanePosition} : new List<int>();
        
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
        var tapNotes = RhythmGamePresenter.TapNoteLanes;
        for (var i = 0; i < tapNotes.Length; i++)
        {
            var notJudgedYet = true;
            for (var j = tapJudgeStartIndex[i]; j < tapNotes[i].Count; j++)
            {
                var note = tapNotes[i][j];
                if (note.hasBeenTapped) continue;
                JudgeResultType judgeResult;
                var reilasNoteEntity = note.note;
                var timeDifference = reilasNoteEntity.JudgeTime - currentTime;
                if (timeDifference > _judgeSeconds["Tap Bad"]) break;
                var difference = CalculateDifference(currentTime, reilasNoteEntity.JudgeTime, "Tap");
                var timeCheck = TimeCheck(currentTime, reilasNoteEntity.JudgeTime, "Tap");

                if (GetTapState(reilasNoteEntity).Contains(i) && notJudgedYet)
                {
                    var nextNoteIndex = j + 1;
                    if (nextNoteIndex != tapNotes[i].Count &&
                        timeDifference < currentTime - tapNotes[i][nextNoteIndex].note.JudgeTime)
                        judgeResult = JudgeResultType.Miss;
                    else
                    {
                        judgeResult = difference switch
                        {
                            var dif when dif <= _judgeSeconds["Tap Perfect"] => JudgeResultType.Perfect,
                            var dif when dif <= _judgeSeconds["Tap Good"] => JudgeResultType.Good,
                            var dif when dif <= _judgeSeconds["Tap Bad"] => JudgeResultType.Bad,
                            _ => timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss
                        };
                    }

                    notJudgedYet = false;
                }
                else judgeResult = timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss;
                if (judgeResult == JudgeResultType.NotJudgedYet) continue;
                AllJudge.Add(judgeResult);
                note.hasBeenTapped = true;
                if (CheckType(reilasNoteEntity, "AboveTap")) RhythmGamePresenter.AboveTapNotes[0].NoteDestroy(false);
                else RhythmGamePresenter.TapNotes[0].NoteDestroy(false);
                tapJudgeStartIndex[i]++;
            }
        }
        
        var internalNotes = RhythmGamePresenter.internalNotes;

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

            if (judgeResult == JudgeResultType.Miss) Debug.Log("Miss...");
        }

        var chainNotes = RhythmGamePresenter.chainNotes;
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
            if (RhythmGamePresenter.chainNoteJudge != null) RhythmGamePresenter.chainNoteJudge[i] = true;
            RhythmGamePresenter.AboveChainNotes[0].NoteDestroy(false);
            chainJudgeStartIndex++;
        }
    }
}
    