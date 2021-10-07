using UnityEngine;

public class Judge_Effector : MonoBehaviour
{
    private GameObject[] _effectors;
    private AudioSource se_Perfect;
    private AudioSource se_Good;
    private AudioSource se_Bad;
    private AudioSource se_;

    void Start()
    {
        _effectors = new GameObject[4];
        /* for (int i = 0; i < 4; i++)
          {
              _effectors[i] = transform.GetChild(i).gameObject;
              Debug.Log(_effectors[i].name);
          }*/
        foreach (Transform child in transform)
        {
            // �q�v�f�̖��O���
            Debug.Log(child.name);
        }

    }
    /// <summary>
    /// 1�^�b�v���ƂɑΉ�����^�b�v�m�[�c�i�����̉\������j�̔��茋�ʂƑΉ������G�t�F�N�g���m�[�c�̒��S���ɕ\������
    /// �^�b�v���̓m�[�c�܂��łȂ��^�C�~���O�ł܂Ƃ߂ď�������̂��ǂ�
    /// </summary>
    /// <param name="judgePositions">
    /// ���肪�s��ꂽ�m�[�c�̃��[���ԍ��z��
    /// </param>
    /// <param name="judgeType">
    /// ����̎��
    /// </param>

    public void TapJudgeEffector(int[] judgePositions, string judgeType)
    {
        bool missBool;
        foreach (int judgePos in judgePositions)
        {
            missBool = false;
            foreach (GameObject effecterObj in _effectors)
            {
                ParticleSystem effect1 = effecterObj.GetComponentsInChildren<ParticleSystem>()[0];
                if (effect1.isPlaying) continue;
                ParticleSystem effect2 = effecterObj.GetComponentsInChildren<ParticleSystem>()[1];
                effecterObj.transform.position = RhythmGamePresenter.LanePositions[judgePos];
                switch (judgeType)
                {
                    case "perfect":
                        effect1.startColor = new Color32(255, 255, 0, 255);
                        effect2.startColor = new Color32(255, 80, 50, 255); break;
                    case "good":
                        effect1.startColor = new Color32(0, 5, 255, 255);
                        effect2.startColor = new Color32(255, 255, 0, 255); break;
                    case "bad":
                        effect1.startColor = new Color32(0, 100, 255, 255);
                        effect2.startColor = new Color32(0, 5, 255, 255); break;
                    case "miss": missBool = true; break;
                }
                if (missBool)
                {
                    break;
                }
                effect1.Play();
                effect2.Play();
                break;
            }
            //�����Ń^�b�v������
        }
    }
}