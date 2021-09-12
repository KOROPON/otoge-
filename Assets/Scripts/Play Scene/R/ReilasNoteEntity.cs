#nullable enable

using System.Collections.Generic;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class ReilasNoteEntity : NoteEntity
    {
        public NoteType Type { get; private set; }


        protected override int GetNoteType(NoteJsonData noteJsonData)
        {
            NoteType noteType = noteJsonData.type switch
            {
                "tap" => NoteType.Tap,
                "hold" => NoteType.Hold,
                "hold-judge-note" => NoteType.HoldInternal,
                "above-tap" => NoteType.AboveTap,
                "above-hold" => NoteType.AboveHold,
                "above-hold-judge-note" => NoteType.HoldInternal,
                "above-slide" => NoteType.AboveSlide,
                "above-slide-judge-note" => NoteType.AboveSlideInternal,
                "above-chain" => NoteType.AboveChain,
                _ => NoteType.None
            };

            if(noteType == NoteType.Hold || noteType == NoteType.AboveHold || noteType == NoteType.AboveSlide)
            {
                var guid = noteJsonData.guid;//このノーツの guid
                Rhythmium.NoteLineJsonData[] allguid = null!;
                allguid = TimelineJsonData.noteLines; //NoteLineJsonData を取得したい
                Debug.Log(Rhythmium.TimelineJsonData.noteLines);
                /*
                foreach(NoteLineJsonData noteline in allguid)
                {

                }
                */
                foreach (var noteLineJsonData in Rhythmium.TimelineJsonData.noteLines)
                {

                }
                //Guidのノーツの取得
            }

            return (int) noteType;
        }

        public override void Initialize(NoteJsonData note, float judgeTime)
        {
            base.Initialize(note, judgeTime);
            Type = (NoteType) IntType;

            // LanePosition = LanePosition * 9;
            // Size = Size * 9;
        }

        /// <summary>
        /// レーンのインデックスを反復する
        /// </summary>
        public IEnumerable<int> GetLaneIndices()
        {
            for (var i = 0; i < Size; i++)
            {
                yield return LanePosition + i;
            }
        }
    }
}