#nullable enable

using Cysharp.Threading.Tasks;
using Rhythmium;
using UnityEngine;

namespace Reilas
{
    public sealed class Test : MonoBehaviour
    {
        private void Awake()
        {
            AwakeAsync().Forget();
        }

        private async UniTask AwakeAsync()
        {
            var chartTextAsset = await Resources.LoadAsync<TextAsset>("Charts/I") as TextAsset;

            if (chartTextAsset == null)
            {
                Debug.LogError("譜面データが見つかりませんでした");
                return;
            }

            var chartJsonData = JsonUtility.FromJson<ChartJsonData>(chartTextAsset.text);

            var chartEntity = new ReilasChartConverter().Convert(chartJsonData);

            Debug.Log(chartEntity);
        }
    }
}