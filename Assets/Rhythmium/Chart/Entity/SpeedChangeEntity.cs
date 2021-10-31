using System;
using UnityEngine;

namespace Rhythmium
{
    /// <summary>
    /// 速度変更情報
    /// </summary>
    [Serializable]
    public struct SpeedChangeEntity
    {
        [SerializeField] private float speed;
        [SerializeField] private float position;

        /// <summary>速度</summary>
        public float Speed => speed;

        /// <summary>小節位置</summary>
        public float Position => position;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="speedChangeJsonData">速度変更情報</param>
        public SpeedChangeEntity(SpeedChangeJsonData speedChangeJsonData)
        {
            position = speedChangeJsonData.measureIndex + speedChangeJsonData.measurePosition.To01();
            speed = speedChangeJsonData.speed;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="speedChangeJsonData">速度変更情報</param>
        public SpeedChangeEntity(OtherObjectJsonData speedChangeJsonData)
        {
            position = speedChangeJsonData.measureIndex + speedChangeJsonData.measurePosition.To01();
            speed = float.Parse(speedChangeJsonData.value);
        }
    }
}
