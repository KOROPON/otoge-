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
                var t = time - 93;
                var z = (-(1 / (t + 1)) + 1) * 2 - 2.8f;
                pos.Add(new Vector3(0, 2.2f, z));
                pos.Add(new Vector3(0, 0, rotation - 2));
            }
            else if (time < 101.78f) // �˂���Đi�� //1.559
            {
                var t = time - 99.8f;
                var z = (t * t * t) * 180 - 1.928f;
                pos.Add(new Vector3(0, 2.2f, z));
                if (rotation % 360 != 0)
                {
                    pos.Add(new Vector3(0, 0, rotation - 2));
                }
                else
                {
                    pos.Add(new Vector3(0, 0, 0));
                }
            }
            else if (time < 103.04f) // �߂��Ă���
            {
                var z = Mathf.Lerp(-500, 2.8f, (time - 101.8f) / 1.4f);
                pos.Add(new Vector3(0, 2.2f, z));
                if (rotation % 360 != 0)
                {
                    pos.Add(new Vector3(0, 0, rotation - 2));
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