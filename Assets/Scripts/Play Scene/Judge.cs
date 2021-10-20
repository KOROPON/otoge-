#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using Reilas;

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
    private int _tapJudgeStartIndex;
    private int _internalJudgeStartIndex;
    private int _chainJudgeStartIndex;

    public static readonly List<JudgeResultType> AllJudge = new List<JudgeResultType>();

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
        return difference <= _judgeSeconds[flag] || currentTime < judgeTime;
    }

    private JudgeResultType InternalOrChain(float currentTime, ReilasNoteEntity note, bool tapState,
        string internalOrChain)
    {
        var timeCheck = TimeCheck(currentTime, note.JudgeTime, internalOrChain, internalOrChain);
        if (tapState)
        {
            return timeCheck ? JudgeResultType.Perfect : JudgeResultType.Miss;
        }

        return timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss;
    }

    private static bool GetTapState(ReilasNoteEntity note)
    {
        var tapState = RhythmGamePresenter.laneTapStates;
        if (note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold ||
            note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlide ||
            note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveChain)
        {
            int noteLanePosition = note.LanePosition + 4;
            switch (noteLanePosition)
            {
                case 4:
                {
                    for (var i = noteLanePosition; i < noteLanePosition + note.Size; i++)
                    {
                        if (tapState[i]) return true;
                    }

                    break;
                }
                case 35:
                {
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.Size - 1; i++)
                    {
                        if (tapState[i]) return true;
                    }

                    break;
                }
                default:
                {
                    for (var i = noteLanePosition - 1; i < noteLanePosition + note.Size; i++)
                    {
                        if (tapState[i]) return true;
                    }

                    break;
                }
            }
        }
        else
        {
            if (tapState[note.LanePosition]) return true;
        }

        return false;
    }

    public void Judge(float currentTime)
    {
        var tapNotes = RhythmGamePresenter.tapNotes;
        for (var i = _tapJudgeStartIndex; i < tapNotes.Count; i++)
        {
            if (RhythmGamePresenter.tapNoteJudge != null && RhythmGamePresenter.tapNoteJudge[i]) continue;
            JudgeResultType judgeResult;
            var timeDifference = tapNotes[i].JudgeTime - currentTime;
            if (timeDifference > _judgeSeconds["Tap Bad"]) break;
            var difference = CalculateDifference(currentTime, tapNotes[i].JudgeTime, "Tap");
            var timeCheck = TimeCheck(currentTime, tapNotes[i].JudgeTime, "Tap Bad", "Tap");
            if (GetTapState(tapNotes[i]))
            {
                judgeResult = difference switch
                {
                    var dif when dif <= _judgeSeconds["Tap Perfect"] => JudgeResultType.Perfect,
                    var dif when dif <= _judgeSeconds["Tap Good"] => JudgeResultType.Good,
                    var dif when dif <= _judgeSeconds["Tap Bad"] => JudgeResultType.Bad,
                    _ => timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss
                };
            }
            else
            {
                judgeResult = timeCheck ? JudgeResultType.NotJudgedYet : JudgeResultType.Miss;
            }

            if (judgeResult == JudgeResultType.NotJudgedYet) continue;
            AllJudge.Add(judgeResult);
            if (RhythmGamePresenter.tapNoteJudge != null) RhythmGamePresenter.tapNoteJudge[i] = true;
            if (tapNotes[i].Type == NoteType.Tap || tapNotes[i].Type == NoteType.Hold)
            {
                RhythmGamePresenter._tapNotes[0].NoteDestroy();
            }
            else if(tapNotes[i].Type == NoteType.AboveTap || tapNotes[i].Type == NoteType.AboveHold || tapNotes[i].Type == NoteType.AboveSlide)
            {
                RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
            }
            
            _tapJudgeStartIndex++;
        }

        var internalNotes = RhythmGamePresenter.internalNotes;
        for (var i = _internalJudgeStartIndex; i < internalNotes.Count; i++)
        {
            if (RhythmGamePresenter.internalNoteJudge != null && RhythmGamePresenter.internalNoteJudge[i]) continue;
            var timeDifference = internalNotes[i].JudgeTime - currentTime;
            if (timeDifference > _judgeSeconds["Internal"]) break;
            var judgeResult = InternalOrChain(currentTime, internalNotes[i], GetTapState(internalNotes[i]), "Internal");
            if (judgeResult == JudgeResultType.NotJudgedYet) continue;
            AllJudge.Add(judgeResult);
            if (RhythmGamePresenter.internalNoteJudge != null) RhythmGamePresenter.internalNoteJudge[i] = true;
            _internalJudgeStartIndex++;
        }

        var chainNotes = RhythmGamePresenter.chainNotes;
        for (var i = _chainJudgeStartIndex; i < chainNotes.Count; i++)
        {
            if (RhythmGamePresenter.chainNoteJudge != null && RhythmGamePresenter.chainNoteJudge[i]) continue;
            var timeDifference = chainNotes[i].JudgeTime - currentTime;
            if (timeDifference > 0) break;
            var judgeResult = InternalOrChain(currentTime, chainNotes[i], GetTapState(chainNotes[i]), "Chain");
            if (judgeResult == JudgeResultType.NotJudgedYet) continue;
            AllJudge.Add(judgeResult);
            if (RhythmGamePresenter.chainNoteJudge != null) RhythmGamePresenter.chainNoteJudge[i] = true;
            RhythmGamePresenter._aboveChainNotes[0].NoteDestroy();
            _chainJudgeStartIndex++;

        }
    }
}
    