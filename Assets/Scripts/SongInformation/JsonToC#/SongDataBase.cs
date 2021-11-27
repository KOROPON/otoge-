[System.Serializable]
public sealed class SongDataBase
{
    public SongName[] songs;
}

[System.Serializable]
public sealed class SongName
{
    public string title;
    public string composer;
    public Beat beat;
    public Custom[] custom;
    public Level level;
    public NoteDesiner noteDesiner;
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
    public int easy;
    public int hard;
    public int extreme;
    public int kujo;
}
[System.Serializable]
public sealed class NoteDesiner
{
    public string easyDesiner;
    public string hardDesiner;
    public string extremeDesiner;
    public string kujoDesiner;
}