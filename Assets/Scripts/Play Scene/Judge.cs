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

public class DelNote
{
    public float noteTime;
    public int noteNum;
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

    
    static List<DelNote> removeNoteNum = new List<DelNote>();
    static bool TapJudgeSystem(ReilasNoteEntity note, float time, List<ReilasNoteEntity> notJudgedNotes)
    {

        if (Mathf.Abs(note.JudgeTime - time) <= 0.041f) // perfect
        {
            Debug.Log("perfect");
            removeNoteNum.Add(new DelNote
            {
                noteTime = note.JudgeTime,
                noteNum = notJudgedNotes.IndexOf(note)
            });
            allJudgeType.Add(new JudgeResult
            {
                ResultType = JudgeResultType.Perfect
            });
        }
        else if (Mathf.Abs(note.JudgeTime - time) <= 0.058f) // good
        {
            Debug.Log("good");
            removeNoteNum.Add(new DelNote
            {
                noteTime = note.JudgeTime,
                noteNum = notJudgedNotes.IndexOf(note)
            });
            allJudgeType.Add(new JudgeResult
            {
                ResultType = JudgeResultType.Good
            });
        }
        else if (Mathf.Abs(note.JudgeTime - time) <= 0.075f) // bad
        {
            Debug.Log("bad");
            removeNoteNum.Add(new DelNote
            {
                noteTime = note.JudgeTime,
                noteNum = notJudgedNotes.IndexOf(note)
            });
            allJudgeType.Add(new JudgeResult
            {
                ResultType = JudgeResultType.Bad
            });
        }
        else
        {
            return true;
        }

        return false;
    }

    static void InternalJudgeSystem(ReilasNoteEntity note, float time, List<ReilasNoteEntity> notJudgedNotes)
    {

        if (time - note.JudgeTime <= 0.090f && time - note.JudgeTime >= 0f)
        {
            Debug.Log("perfect");
            judgedInHold.Add(new JudgeResultInHold
            {
                time = note.JudgeTime,
                perfect = true
            });
            removeNoteNum.Add(new DelNote
            {
                noteTime = note.JudgeTime,
                noteNum = notJudgedNotes.IndexOf(note)
            });
        }
    }

    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime, List<LaneTapState> aboveTapState) //Judge(�m�[�c����,�Đ�����,�^�b�v����){}
    {
        //Debug.Log(currentTime);
        removeNoteNum.Clear();

        const float noJudgeTime = 0.090f; //���ԋ߂��m�[�c���������藣���Ă��Ɣ��肵�Ȃ�
        float tapTime = 0; //�������ԂɃ^�b�v�����鎞�A���̎���

        int destroyNum;

        //bool firstTap = true;
        int laneNumMin;
        int laneNumMax;

        void RemoveNoteInList(List<DelNote> noteNum)
        {
            noteNum.OrderBy(note => note.noteTime);
            for (var i = noteNum.Count - 1; i >= 0; i--)
            {
                notJudgedNotes.RemoveAt(noteNum[i].noteNum);
            }
        }




        foreach (LaneTapState tapState in aboveTapState) //�^�b�v�����Ŏ���
        {
            // Debug.Log(tapState.TapStating);
            bool alreadyJudge = false;

            foreach (ReilasNoteEntity note in notJudgedNotes) //�m�[�c�����̎擾
            {
                Debug.Log("タップによるJudge");

                if (note.JudgeTime - currentTime > noJudgeTime) // ���̔��肷���ׂ��m�[�c���O�� 0.090 �b �ȏ㗣���Ă��� returns
                {
                    //Debug.Log("return");
                    aboveTapState.Clear();
                    return;
                }



                ///<summary>
                ///�m�[�c�̃��[���������擾
                ///
                /// ���̃m�[�c�� Json �̃��[���ԍ��� +4
                /// ���̃m�[�c�͂��̂܂�
                ///
                ///</summary>
                if (note.Type == NoteType.AboveChain || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlide || note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveTap)
                {
                    laneNumMin = note.LanePosition + 3;
                    laneNumMax = laneNumMin + note.Size;
                }
                else
                {
                    laneNumMin = note.LanePosition;
                    laneNumMax = note.LanePosition;
                }


                if (alreadyJudge && (note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide))
                {
                    tapTime = note.JudgeTime;
                }


                if (laneNumMin <= tapState.laneNumber && tapState.laneNumber <= laneNumMax) //���[���ԍ��������Ƃ�
                {

                    if (note.Type == NoteType.Tap)//�^�b�v�A�z�[���h�ƃX���C�h�̎n�_�͈����̃^�b�v�ň��x�܂Ŕ����A�������ԂȂ畡��
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if(TapJudgeSystem(note, currentTime,notJudgedNotes))
                                {
                                    continue;
                                }
                                
                                int indexNum = 0;
                                foreach (ReilasNoteEntity reilas in notJudgedNotes)
                                {
                                    if (reilas.Type == NoteType.Tap)
                                    {
                                        if (reilas == note)
                                        {
                                            break;
                                        }
                                        indexNum++;
                                    }
                                }
                                

                                //Debug.Log("TapDestroy  " + indexNum + "   " + RhythmGamePresenter._tapNotes.Count());
                                RhythmGamePresenter._tapNotes[indexNum].NoteDestroy();
                                //RhythmGamePresenter._tapNotes.RemoveAt(indexNum);
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if(note.Type == NoteType.AboveTap)
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (TapJudgeSystem(note, currentTime,notJudgedNotes))
                                {
                                    continue;
                                }

                                int indexNum = 0;
                                foreach (ReilasNoteEntity reilas in notJudgedNotes)
                                {
                                    if (reilas.Type == NoteType.AboveTap)
                                    {
                                        if (reilas == note)
                                        {
                                            Debug.Log("Break");
                                            break;
                                        }
                                        indexNum++;
                                    }
                                }
                                

                                //Debug.Log("AboveTapDestroy  " + indexNum);
                                RhythmGamePresenter._aboveTapNotes[indexNum].NoteDestroy();
                                //RhythmGamePresenter._aboveTapNotes.RemoveAt(indexNum);
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if (note.Type == NoteType.Hold)
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (TapJudgeSystem(note, currentTime,notJudgedNotes))
                                {
                                    continue;
                                }
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if (note.Type == NoteType.AboveSlide)
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (TapJudgeSystem(note, currentTime,notJudgedNotes))
                                {
                                    continue;
                                }
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if (note.Type == NoteType.AboveHold)
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (TapJudgeSystem(note, currentTime,notJudgedNotes))
                                {
                                    continue;
                                }
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if (note.Type == NoteType.HoldInternal) //�������n�m�[�c�̓��������͑O�� 90
                    {
                        InternalJudgeSystem(note, currentTime,notJudgedNotes);
                    }
                    else if (note.Type == NoteType.AboveHoldInternal)
                    {
                        InternalJudgeSystem(note, currentTime,notJudgedNotes);
                    }
                    else if (note.Type == NoteType.AboveSlideInternal)
                    {
                        InternalJudgeSystem(note, currentTime,notJudgedNotes);
                    }
                    else if (note.Type == NoteType.AboveChain) //�`�F�C���m�[�c�͌����� 25 �̔��蕝
                    {
                        if (currentTime - note.JudgeTime >= 0 && currentTime - note.JudgeTime <= 0.025f)
                        {
                            //Perfect
                            removeNoteNum.Add(new DelNote
                            {
                                noteTime = note.JudgeTime,
                                noteNum = notJudgedNotes.IndexOf(note)
                            });

                            allJudgeType.Add(new JudgeResult
                            {
                                ResultType = JudgeResultType.Perfect
                            });
                            per++;
                            Debug.Log("perfect");
                        }
                        else
                        {
                            continue;
                        }

                        
                        int indexNum = 0;
                        foreach (ReilasNoteEntity reilas in notJudgedNotes)
                        {
                            if (reilas.Type == NoteType.AboveChain)
                            {
                                if (reilas == note)
                                {
                                    break;
                                }
                                indexNum++;
                            }
                        }
                        

                        //Debug.Log("ChainDestroy  " + indexNum);
                        RhythmGamePresenter._aboveChainNotes[indexNum].NoteDestroy();
                        //RhythmGamePresenter._aboveChainNotes.RemoveAt(indexNum);
                    }
                }
            }

            RemoveNoteInList(removeNoteNum);
        }


        notJudgedNotes.OrderBy(notes => notes.JudgeTime);
        //Debug.Log(notJudgedNotes[0].JudgeTime.ToString() + "   " + notJudgedNotes.Count());
        int i = 0;
        foreach (ReilasNoteEntity note in notJudgedNotes) //通過したノーツに Miss 判定をする
        {
            i++;
            //Debug.Log(note.JudgeTime);
            if ((note.JudgeTime - currentTime) > 0f)
            {
                Debug.Log("Break  " + i);
                break;
            }

            //Debug.Log("judge  " + note.Type);

            if (note.Type == NoteType.AboveSlideInternal)
            {
                if (currentTime - note.JudgeTime <= 0.090f)
                {
                    continue;
                }
                else
                {
                    removeNoteNum.Add(new DelNote
                    {
                        noteTime = note.JudgeTime,
                        noteNum = notJudgedNotes.IndexOf(note)
                    });
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    miss++;
                    //Debug.Log("miss");
                }
            }
            else if (note.Type == NoteType.AboveHoldInternal)
            {

                if (currentTime - note.JudgeTime <= 0.090f)
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
                    //Debug.Log("miss");
                }
            }
            else if (note.Type == NoteType.HoldInternal)
            {

                if (currentTime - note.JudgeTime <= 0.090f)
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
                    //Debug.Log("miss");
                }
            }
            else if (note.Type == NoteType.Tap)
            {
                if (currentTime - note.JudgeTime > 0.075f)
                {
                    removeNoteNum.Add(new DelNote
                    {
                        noteTime = note.JudgeTime,
                        noteNum = notJudgedNotes.IndexOf(note)
                    });
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    miss++;
                    //Debug.Log("miss");
                    RhythmGamePresenter._tapNotes[0].NoteDestroy();
                    //RhythmGamePresenter._tapNotes.RemoveAt(0);
                }
                else
                {
                    //Debug.Log("notMiss");
                    continue;
                }
            }
            else if (note.Type == NoteType.Hold)
            {

                if (currentTime - note.JudgeTime > 0.075f)
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
                    //Debug.Log("miss");
                }
                else
                {
                    continue;
                }
            }
            else if (note.Type == NoteType.AboveTap)
            {

                if (currentTime - note.JudgeTime > 0.075f)
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
                    //Debug.Log("miss");
                    RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
                    //RhythmGamePresenter._aboveTapNotes.RemoveAt(0);
                }
                else
                {
                    continue;
                }
            }
            else if (note.Type == NoteType.AboveHold)
            {

                if (currentTime - note.JudgeTime > 0.075f)
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
                    //Debug.Log("miss");
                }
                else
                {
                    continue;
                }
            }
            else if (note.Type == NoteType.AboveSlide)
            {

                if (currentTime - note.JudgeTime > 0.075f)
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
                    //Debug.Log("miss");
                }
                else
                {
                    continue;
                }
            }
            else if (note.Type == NoteType.AboveChain)
            {
                if (currentTime - note.JudgeTime > 0.025f)
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
                    //Debug.Log("miss");
                    RhythmGamePresenter._aboveChainNotes[0].NoteDestroy();
                    //RhythmGamePresenter._aboveChainNotes.RemoveAt(0);
                }
                else
                {
                    continue;
                }
            }
        }
        RemoveNoteInList(removeNoteNum);

        aa = "Perfect:" + per.ToString() + "  Good:" + good.ToString() + "  Bad:" + bad.ToString() + "  Miss:" + miss.ToString();
        //Debug.Log(aa);

    }
}
