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
    public static int per;
    public static int good;
    public static int bad;
    public static int miss;


    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime, List<LaneTapState> aboveTapState) //Judge(�m�[�c����,�Đ�����,�^�b�v����){}
    {
        Debug.Log("judge");

        const float noJudgeTime = 0.090f; //���ԋ߂��m�[�c���������藣���Ă��Ɣ��肵�Ȃ�
        float tapTime = 0; //�������ԂɃ^�b�v�����鎞�A���̎���

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



        foreach (LaneTapState tapState in aboveTapState) //�^�b�v�����Ŏ���
        {
            // Debug.Log(tapState.TapStating);

            foreach (ReilasNoteEntity note in notJudgedNotes) //�m�[�c�����̎擾
            {
                //Debug.Log(note.Type);

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


                if (laneNumMin <= tapState.laneNumber && tapState.laneNumber <= laneNumMax) //���[���ԍ��������Ƃ�
                {

                    if (note.Type == NoteType.Tap)//�^�b�v�A�z�[���h�ƃX���C�h�̎n�_�͈����̃^�b�v�ň��x�܂Ŕ����A�������ԂȂ畡��
                    {
                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                {
                                    // �p�[�t�F�N�g
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    });
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
                                RhythmGamePresenter._tapNotes[0].NoteDestroy();
                                RhythmGamePresenter._tapNotes.RemoveAt(0);
                                //}
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if(note.Type == NoteType.AboveTap)
                    {

                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                {
                                    // �p�[�t�F�N�g
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    });
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
                                RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
                                RhythmGamePresenter._aboveTapNotes.RemoveAt(0);
                                //}
                            }
                            alreadyJudge = true;
                        }
                    }
                    else if(note.Type == NoteType.Hold)
                    {

                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                {
                                    // �p�[�t�F�N�g
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    });
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
                    else if(note.Type == NoteType.AboveSlide)
                    {

                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                {
                                    // �p�[�t�F�N�g
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    });
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
                    else if(note.Type == NoteType.AboveHold)
                    {

                        if (!alreadyJudge || tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                {
                                    // �p�[�t�F�N�g
                                    removeNoteNum.Add(new DelNote
                                    {
                                        noteTime = note.JudgeTime,
                                        noteNum = notJudgedNotes.IndexOf(note)
                                    });
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
                    else if (note.Type == NoteType.HoldInternal) //�������n�m�[�c�̓��������͑O�� 90
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
                    else if(note.Type == NoteType.AboveHoldInternal)
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
                    else if(note.Type == NoteType.AboveSlideInternal)
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
                    else if (note.Type == NoteType.AboveChain) //�`�F�C���m�[�c�͌����� 25 �̔��蕝
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
                        RhythmGamePresenter._aboveChainNotes[0].NoteDeestroy();
                        RhythmGamePresenter._aboveChainNotes.RemoveAt(0);
                    }
                }
            }

            RemoveNoteInList(removeNoteNum);
        }



        foreach (ReilasNoteEntity note in notJudgedNotes) //�^�b�v�����Ă��Ȃ��ꍇ�Ɏg�p������ Miss ����
        {
            Debug.Log("missJudge");
            if(note.JudgeTime - currentTime > 0.075f)
            {
                break;
            }

            if(note.Type == NoteType.AboveSlideInternal)
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
            else if (note.Type == NoteType.AboveHoldInternal)
            {

                if (note.JudgeTime - currentTime >= 0)
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
            else if(note.Type == NoteType.HoldInternal)
            {

                if (note.JudgeTime - currentTime >= 0)
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
            else if(note.Type == NoteType.Tap)
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
                    RhythmGamePresenter._tapNotes[0].NoteDestroy();
                    Debug.Log("Destroy Tap");
                    RhythmGamePresenter._tapNotes.RemoveAt(0);
                }
                else
                {
                    continue;
                }
            }
            else if(note.Type == NoteType.Hold)
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
                }
                else
                {
                    continue;
                }
            }
            else if(note.Type == NoteType.AboveTap)
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
                    RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
                    RhythmGamePresenter._aboveTapNotes.RemoveAt(0);
                }
                else
                {
                    continue;
                }
            }
            else if(note.Type == NoteType.AboveHold)
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
                }
                else
                {
                    continue;
                }
            }
            else if(note.Type == NoteType.AboveSlide)
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
                    RhythmGamePresenter._aboveChainNotes[0].NoteDeestroy();
                    RhythmGamePresenter._aboveChainNotes.RemoveAt(0);
                }
                else
                {
                    continue;
                }
            }
        }
        RemoveNoteInList(removeNoteNum);

        aa = ("Perfect:" + per.ToString() + "  Good:" + good.ToString() + "  Bad:" + bad.ToString() + "  Miss:" + miss.ToString());
        Debug.Log(aa);

    }
}
