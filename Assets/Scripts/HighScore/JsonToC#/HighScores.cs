//ハイスコアを保存しておくための曲のクラス
[System.Serializable]
class HighScores
{
    Song song;
}

//曲の難易度ごとにハイスコアを保存する
[System.Serializable]
class Song
{
    int easy;
    int hard;
    int extreme;
    int kujo;
}
