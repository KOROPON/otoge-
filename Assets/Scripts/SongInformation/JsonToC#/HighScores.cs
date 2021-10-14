//ハイスコアを保存しておくための曲のクラス
[System.Serializable]
class HighScores
{
    public Song[] songs;
}

//曲の難易度ごとに曲の情報を保存する
[System.Serializable]
class Song
{
    public string title;
    public bool extremeLock;
    public Difficulty Easy;
    public Difficulty Hard;
    public Difficulty Extreme;
    public Difficulty KUJO;
}

[System.Serializable]
class Difficulty
{
    public string rank;
    public bool fullCombo;
    public bool allPerfect;
    public int highScore;
}
