[System.Serializable]
public sealed class SongDataBase
{
    public SongName[] songs;
}

[System.Serializable]
public sealed class SongName
{
    public string title;
    public Beat beat;
    public Custom[] custom;
    public Level level;
}

[System.Serializable]
public sealed class Beat
{
    public int numerator;
    public int denominator;
    public float bpm;
}

[System.Serializable]
public struct Custom
{
    public float time;
    public Beat changedBeat;
}

[System.Serializable]
public sealed class Level
{
    public int Easy;
    public int Hard;
    public int Extreme;
    public int KUJO;
}