# nullable enable

using UnityEngine;
using System.Collections.Generic;

namespace Reilas
{
    /// <summary>
    /// ボス曲のカメラの位置、回転を操作するScript
    /// 
    /// 初期位置は (0, 2.2, -2.8)
    /// </summary>
    public static class CameraPosCalculator
    {
        public static List<Vector3> CameraPosCalculatorService(float time, float rotation)
        {
            List<Vector3> pos = new List<Vector3>();
            if (time < 100) //ねじれて進む
            {
                float t = time - 93;
                float z = (-(1 / (t + 1)) + 1) * 5 - 2.8f;
                Vector3 position = new Vector3(0, 2.2f, z);
                Vector3 rota = new Vector3(0, 0, rotation + 5);
                pos.Add(position);
                pos.Add(rota);
            }
            else if (100 < time && time < 101.8f) // ねじれて進む
            {

            }
            else if (time < 103.2f) // 戻ってくる
            {

            }
            return pos;
        }
    }
}