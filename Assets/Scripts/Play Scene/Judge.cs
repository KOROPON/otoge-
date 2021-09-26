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

    
    bool TapJudgeSystem(float noteTime, float currentTime)
    {

        if (Mathf.Abs(noteTime - currentTime) <= 0.041f) // perfect
        {
            Debug.Log("perfect");
        }
        else if (Mathf.Abs(noteTime - currentTime) <= 0.058f) // good
        {
            Debug.Log("good");
        }
        else if (Mathf.Abs(noteTime - currentTime) <= 0.075f) // bad
        {
            Debug.Log("bad");
        }
        else
        {
            return true;
        }

        return false;
    }

    bool InternalJudgeSystem(float noteTime, float currentTime)
    {

        if (noteTime - currentTime <= 0.090f)
        {
            Debug.Log("perfect");
            judgedInHold.Add(new JudgeResultInHold
            {
                time = noteTime,
                perfect = true
            });
        }
        else
        {
            return true;
        }

        return false;
    }

    void RemoveNoteInList(List<int> delNums, List<List<float>> noteList)
    {
        for (int i = delNums.Count - 1; i >= 0; i--)
        {
            noteList.RemoveAt(delNums[i]);
        }
    }


    List<float> TapJudge(List<List<float>> tapList, float currentTime, List<LaneTapState> lanetapStates, NoteType type)
    {
        float laneNumMin;
        float laneNumMax;
        List<int> allDelNums = new List<int>();
        foreach (LaneTapState tapstate in lanetapStates)
        {
            allDelNums.Clear();
            int delNum = 0;
            foreach (List<float> list in tapList)
            {
                delNum++;

                ///<summary>
                /// レーン情報の取得
                ///</summary>
                if (type == NoteType.Tap) //下のレーン
                {
                    laneNumMin = list[1];
                    laneNumMax = list[1];
                }
                else    //上のレーン
                {
                    if (list[1] == 0)
                    {
                        laneNumMin = 4;
                    }
                    else
                    {
                        laneNumMin = list[1] + 3;
                    }
                    laneNumMax = list[1] + list[2] + 3;
                }

                if (laneNumMin <= tapstate.laneNumber && tapstate.laneNumber <= laneNumMax && tapstate.TapStating)
                {
                    if (TapJudgeSystem(list[0], currentTime))
                    {
                        //判定なし
                        continue;
                    }

                    allDelNums.Add(delNum); // tapList , RhythmGamePresenter._***Notes  から delNum 番目の要素を削除
                }
            }

            RemoveNoteInList(allDelNums, tapList); // tapList からDelete

            if (type == NoteType.Tap)
            {
                for (int i = allDelNums.Count - 1; i >= 0; i--)
                {
                    RhythmGamePresenter._tapNotes.RemoveAt(allDelNums[i]);
                }
            }
            else if (type == NoteType.AboveTap)
            {
                for (int i = allDelNums.Count - 1; i >= 0; i--)
                {
                    RhythmGamePresenter._aboveTapNotes.RemoveAt(allDelNums[i]);
                }
            }
        }
    }

    private void InternalJudge(List<List<float>> internalList, float currentTime, List<LaneTapState> laneTapStates, NoteType type)
    {

        float laneNumMin;
        float laneNumMax;
        List<int> allDelNums = new List<int>();
        foreach (LaneTapState tapstate in laneTapStates)
        {
            allDelNums.Clear();
            int delNum = 0;
            foreach (List<float> list in internalList)
            {
                delNum++;

                ///<summary>
                /// レーン情報の取得
                ///</summary>
                if (type == NoteType.Hold) //下のレーン
                {
                    laneNumMin = list[1];
                    laneNumMax = list[1];
                }
                else    //上のレーン
                {
                    if (list[1] == 0)
                    {
                        laneNumMin = 4;
                    }
                    else
                    {
                        laneNumMin = list[1] + 3;
                    }
                    laneNumMax = list[1] + list[2] + 3;
                }

                if (laneNumMin <= tapstate.laneNumber && tapstate.laneNumber <= laneNumMax)
                {
                    if (InternalJudgeSystem(list[0], currentTime))
                    {
                        //判定なし
                        continue;
                    }

                    allDelNums.Add(delNum); // internalList , RhythmGamePresenter._***Notes  から delNum 番目の要素を削除
                }
            }
            
            RemoveNoteInList(allDelNums, internalList); // internalList からDelete
        }
    }

    void ChainJudge(List<List<float>> chainList, float currentTime, List<LaneTapState> laneTapStates, NoteType type)
    {
        float laneNumMin;
        float laneNumMax;
        List<int> allDelNums = new List<int>();
        foreach (LaneTapState tapstate in laneTapStates)
        {
            allDelNums.Clear();
            int delNum = 0;
            foreach (List<float> list in chainList)
            {
                delNum++;

                ///<summary>
                /// レーン情報の取得
                ///</summary>
                if (type == NoteType.Tap) //下のレーン
                {
                    laneNumMin = list[1];
                    laneNumMax = list[1];
                }
                else    //上のレーン
                {
                    if (list[1] == 0)
                    {
                        laneNumMin = 4;
                    }
                    else
                    {
                        laneNumMin = list[1] + 3;
                    }
                    laneNumMax = list[1] + list[2] + 3;
                }

                if (laneNumMin <= tapstate.laneNumber && tapstate.laneNumber <= laneNumMax && tapstate.TapStating)
                {
                    if (currentTime - list[0] <= 0.025f)
                    {
                        Debug.Log("perfect");
                        allJudgeType.Add(new JudgeResult
                        {
                            ResultType = JudgeResultType.Perfect
                        });
                    }
                    else
                    {
                        // 判定なし
                        continue;
                    }

                    allDelNums.Add(delNum); // tapList , RhythmGamePresenter._***Notes  から delNum 番目の要素を削除
                }
            }

            RemoveNoteInList(allDelNums, chainList); // tapList からDelete

            if (type == NoteType.Tap)
            {
                for (int i = allDelNums.Count - 1; i >= 0; i--)
                {
                    RhythmGamePresenter._aboveChainNotes.RemoveAt(allDelNums[i]);
                }
            }
        }
    }

    void MissTapJudge(float currentTime, List<List<float>> notesList, NoteType type)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach(List<float> note in notesList)
        {
            if(note[0] - currentTime > -0.075f)
            {
                return;
            }
            else
            {
                if(type == NoteType.Tap)
                {
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    RhythmGamePresenter._tapNotes[0].NoteDestroy();
                }
                else
                {
                    allJudgeType.Add(new JudgeResult
                    {
                        ResultType = JudgeResultType.Miss
                    });
                    RhythmGamePresenter._aboveTapNotes[0].NoteDestroy();
                }
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }

    void MissHoldJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach(List<float> note in notesList)
        {
            if(note[0] - currentTime > -0.075f)
            {
                return;
            }
            else
            {
                allJudgeType.Add(new JudgeResult
                {
                    ResultType = JudgeResultType.Miss
                });
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }

    void MissInternalJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach (List<float> note in notesList)
        {
            if (note[0] - currentTime > 0)
            {
                return;
            }
            else
            {
                judgedInHold.Add(new JudgeResultInHold
                {
                    time = note[0],
                    perfect = false
                });
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }

    void MissChainJudge(float currentTime, List<List<float>> notesList)
    {
        List<int> delNum = new List<int>();
        int del = 0;
        foreach (List<float> note in notesList)
        {
            if (note[0] - currentTime > -0.025f)
            {
                return;
            }
            else
            {
                allJudgeType.Add(new JudgeResult
                {
                    ResultType = JudgeResultType.Miss
                });
                RhythmGamePresenter._aboveChainNotes[0].NoteDestroy();
                delNum.Add(del);
            }
            del++;
        }
        RemoveNoteInList(delNum, notesList);
    }


    public void Judge(float currentTime, List<LaneTapState> tapStates)
    {
        MissTapJudge(currentTime, RhythmGamePresenter.notJudgedTapNotes, NoteType.Tap);
        MissTapJudge(currentTime, RhythmGamePresenter.notJudgedAboveTapNotes, NoteType.AboveTap);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudegedHoldNotes);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedAboveHoldNotes);
        MissHoldJudge(currentTime, RhythmGamePresenter.notJudgedAboveSlideNotes);
        MissInternalJudge(currentTime, RhythmGamePresenter.notJudgedInternalNotes);
        MissChainJudge(currentTime, RhythmGamePresenter.notJudegedAboveChainNotes);

        TapJudge();
        TapJudge();
        TapJudge();
        TapJudge();
        TapJudge();        
        InternalJudge();
        InternalJudge();
        InternalJudge();        
        ChainJudge();
    }


    public void aaaaaaaaaaaaaJudge(List<ReilasNoteEntity> notJudgedNotes, float currentTime, List<LaneTapState> aboveTapState) //Judge(�m�[�c����,�Đ�����,�^�b�v����){}
    {
        //Debug.Log(currentTime);
        removeNoteNum.Clear();

        const float noJudgeTime = 0.090f; //���ԋ߂��m�[�c���������藣���Ă��Ɣ��肵�Ȃ�
        float tapTime = 0; //�������ԂɃ^�b�v�����鎞�A���̎���

        int destroyNum;

        //bool firstTap = true;
        int laneNumMin;
        int laneNumMax;





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
