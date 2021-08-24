#nullable enable

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

        //var inputService = _inputService;
        //int judgeNoteCount = 0;

        foreach (var note in notJudgedNotes) //�m�[�c���̎擾
        {
            /*
            judgeNoteCount++;
            
            if(judgeNoteCount > 4) // 1�t���[���ɔ��肷��̂� 4 �܂� (���~�b�g)
            {
                break;
            }
            */

            foreach (var tapState in aboveTapState) //�^�b�v���Ŏ���
            {

                if (note.LanePosition == tapState.laneNumber) //���[���ԍ��������Ƃ�
                {

                    // ���胉�C�����߂��� 0.75 �b�o������~�X�ɂ���
                    if (currentTime - note.JudgeTime > 0.75f)
                    {
                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Miss
                        });

                        continue;
                    }


                    if (note.JudgeTime - currentTime >= noJudgeTime)
                    {
                        break;
                    }

                    if (note.Type == NoteType.Tap)
                    {
                        for (var i = 0; i < note.Size; i++)
                        {

                            // �������ꂽ�u�Ԃ���
                            if (tapState.TapStating)
                            {
                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.2f)
                                {
                                    // �p�[�t�F�N�g
                                    notJudgedNotes.RemoveAt(0);

                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Perfect
                                    });

                                    // ���C���̃N���X�ɔ��茋�ʂ�`���܂�
                                    GameObject.FindObjectOfType<RhythmGamePresenter>().HandleJudgeResult(JudgeResultType.Perfect);
                                }

                                if (Mathf.Abs(note.JudgeTime - currentTime) < 0.4f)
                                {
                                    // GOOD
                                    notJudgedNotes.RemoveAt(0);

                                    allJudgeType.Add(new JudgeResult
                                    {
                                        ResultType = JudgeResultType.Good
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
        //notJudgedNotes.RemoveAt(0);
    }
}

