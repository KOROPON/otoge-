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
/// ���茋��
/// </summary>
public class JudgeResult
{
    public JudgeResultType ResultType;
}

/// <summary>
/// ������s���T�[�r�X
/// </summary>
public class JudgeService
{
    public static List<JudgeResult> allJudgeType = new List<JudgeResult>();//�����`����
    public static List<float> judgedInHold = new List<float>(); //�������m�[�c�̓��������`����


    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime,List<LaneTapState> aboveTapState) //Judge(�m�[�c���,�Đ�����,�^�b�v���){}
    {
        const float noJudgeTime = 0.090f; //��ԋ߂��m�[�c�������藣��Ă�Ɣ��肵�Ȃ�
        float tapTime = 0; //�������ԂɃ^�b�v�����鎞�A���̎���

        bool firstTap = true;
        bool alreadyJudge = false;
        int laneNumMin;
        int laneNumMax;
        
        /*
        void RemoveNoteInList(List<int> noteNum)
        {

        }
        */

        foreach (var tapState in aboveTapState) //�^�b�v���Ŏ���
        {

            foreach (var note in notJudgedNotes) //�m�[�c���̎擾
            {
                if (note.JudgeTime - currentTime > noJudgeTime) // ���̔��肷��ׂ��m�[�c���O�� 0.090 �b �ȏ㗣��Ă��� returns
                {
                    aboveTapState.Clear();
                    return;
                }


                ///<summary>
                ///�m�[�c�̃��[�������擾
                ///
                /// ��̃m�[�c�� Json �̃��[���ԍ��� +4
                /// ���̃m�[�c�͂��̂܂�
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
                

                if (laneNumMin <= tapState.laneNumber && tapState.laneNumber <= laneNumMax) //���[���ԍ��������Ƃ�
                {


                    if (note.Type == NoteType.Tap || note.Type == NoteType.Hold || note.Type == NoteType.AboveTap || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveSlide || tapState.TapStating)//�^�b�v�A�z�[���h�ƃX���C�h�̎n�_�͈��̃^�b�v�ň�x�܂Ŕ���A�������ԂȂ畡��
                    {
                        if (!alreadyJudge && tapTime == note.JudgeTime)
                        {
                            //for (var i = 0; i < note.Size; i++)
                            //{

                            // �������ꂽ�u�Ԃ���
                            //if ()
                            //{
                            if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                            {
                                // �p�[�t�F�N�g
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
                    else if(note.Type == NoteType.HoldInternal || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlideInternal) //�������n�m�[�c�̓�������͑O�� 90
                    {
                        judgedInHold.Add(note.JudgeTime);
                        notJudgedNotes.RemoveAt(notJudgedNotes.IndexOf(note));
                    }
                    else if(note.Type == NoteType.AboveChain) //�`�F�C���m�[�c�͌��� 25 �̔��蕝
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

