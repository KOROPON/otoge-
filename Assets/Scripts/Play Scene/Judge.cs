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

    private readonly InputService _inputService = new InputService();

    //private List<JudgeResult> _result = new List<JudgeResult>(10);

    public static void Judge(List<ReilasNoteEntity> notJudgedNotes, float currentTime,List<LaneTapState> aboveTapState) //Judge(�m�[�c���,�Đ�����,�^�b�v���){}
    {
        const float noJudgeTime = 1f; //��ԋ߂��m�[�c�������藣��Ă�Ɣ��肵�Ȃ�

        bool alreadyJudge = false;
        int laneNum;
        //var inputService = _inputService;
        //int judgeNoteCount = 0;

        foreach (var note in notJudgedNotes) //�m�[�c���̎擾
        {

            if (note.JudgeTime - currentTime >= noJudgeTime) // ���̔��肷��ׂ��m�[�c��1�b�ȏ㗣��Ă���Break
            {
                aboveTapState.Clear();
                break;
            }

            foreach (var tapState in aboveTapState) //�^�b�v���Ŏ���
            {
               // Debug.Log(note.LanePosition + "Lane");
                if(note.Type == NoteType.AboveChain || note.Type == NoteType.AboveHold || note.Type == NoteType.AboveHoldInternal || note.Type == NoteType.AboveSlide || note.Type == NoteType.AboveSlideInternal || note.Type == NoteType.AboveTap)
                {
                    laneNum = note.LanePosition + 4;
                }
                else
                {
                    laneNum = note.LanePosition;
                }

                if (laneNum == tapState.laneNumber) //���[���ԍ��������Ƃ�
                {

                    // ���胉�C�����߂��� 0.75 �b�o������~�X�ɂ���
                    if (currentTime - note.JudgeTime > 0.075f)
                    {
                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Miss
                        });

                        continue;
                    }



                    if (!alreadyJudge)//�^�b�v�A�z�[���h�ƃX���C�h�̎n�_�͈��̃^�b�v�ň�x�܂Ŕ���
                    {
                        if (note.Type == NoteType.Tap)
                        {
                            for (var i = 0; i < note.Size; i++)
                            {

                                // �������ꂽ�u�Ԃ���
                                if (tapState.TapStating)
                                {
                                    if (Mathf.Abs(note.JudgeTime - currentTime) < 0.041f)
                                    {
                                        // �p�[�t�F�N�g
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Perfect
                                        });

                                        Debug.Log("Perfect");

                                        // ���C���̃N���X�ɔ��茋�ʂ�`���܂�
                                        //GameObject.FindObjectOfType<RhythmGamePresenter>().HandleJudgeResult(JudgeResultType.Perfect);
                                    }

                                    if (Mathf.Abs(note.JudgeTime - currentTime) < 0.058f)
                                    {
                                        // GOOD
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Good
                                        });

                                        Debug.Log("Good");
                                    }
                                    if (Mathf.Abs(note.JudgeTime - currentTime) < 0.075f)
                                    {
                                        // BAD
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Bad
                                        });

                                        Debug.Log("Bad");
                                    }
                                    if (Mathf.Abs(note.JudgeTime - currentTime) >= 0.075f)
                                    {
                                        // MISS
                                        notJudgedNotes.RemoveAt(0);

                                        allJudgeType.Add(new JudgeResult
                                        {
                                            ResultType = JudgeResultType.Miss
                                        });

                                        Debug.Log("Miss");
                                    }
                                }
                            }
                        }
                        if(note.Type == NoteType.AboveTap)
                        {

                        }
                    }
                }
            }
        }
    }
}

