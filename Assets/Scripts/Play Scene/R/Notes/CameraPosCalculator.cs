# nullable enable

using UnityEngine;
using System.Collections.Generic;

namespace Reilas
{
    /// <summary>
    /// �{�X�Ȃ̃J�����̈ʒu�A��]�𑀍삷��Script
    /// 
    /// �����ʒu�� (0, 2.2, -2.8)
    /// </summary>
    public static class CameraPosCalculator
    {
        public static List<Vector3> CameraPosCalculatorService(float time, float rotation)
        {
            List<Vector3> pos = new List<Vector3>();
            if (time < 99.8) //�˂���Đi��
            {
                float t = time - 93;
                float z = (-(1 / (t + 1)) + 1) * 5 - 2.8f;
                pos.Add(new Vector3(0, 2.2f, z));
                pos.Add(new Vector3(0, 0, rotation - 5));
            }
            else if (time < 101.78f) // �˂���Đi�� //1.559
            {
                float t = time - 99.8f;
                float z = (t * t * t) * 200 + 1.559f;
                pos.Add(new Vector3(0, 2.2f, z));
                if (rotation % 360 != 0)
                {
                    pos.Add(new Vector3(0, 0, rotation - 5));
                }
                else
                {
                    pos.Add(new Vector3(0, 0, 0));
                }
            }
            else if (time < 103.04f) // �߂��Ă���
            {
                float z = Mathf.Lerp(-500, 2.8f, (time - 101.8f) / 1.4f);
                pos.Add(new Vector3(0, 2.2f, z));
                if (rotation % 360 != 0)
                {
                    pos.Add(new Vector3(0, 0, rotation - 5));
                }
                else
                {
                    pos.Add(new Vector3(0, 0, 0));
                }
            }
            else
            {
                pos.Add(new Vector3(0, 2.2f, -2.8f));
                pos.Add(new Vector3(0, 0, 0));
            }
            return pos;
        }
    }
}