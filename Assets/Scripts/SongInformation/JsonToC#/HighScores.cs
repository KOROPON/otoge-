//ハイスコアを保存しておくための曲のクラス
[System.Serializable]
class HighScores
{
    public Song[] songs;
}

//曲の難易度ごとに曲の情報を保存する
[System.Serializable]
internal class Song
{
    public string title;
    public bool extremeLock;
    public bool kujoLock;
    public Difficulty easy;
    public Difficulty hard;
    public Difficulty extreme;
    public Difficulty kujo;
}

[System.Serializable]
internal class Difficulty
{
    public string rank;
    public bool fullCombo;
    public bool allPerfect;
    public bool clear;
    public int highScore;
}
