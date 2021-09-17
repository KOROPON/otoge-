[System.Serializable]
class Level
{
    public SongName[] songs;
}

[System.Serializable]
class SongName
{
    public string title;
    public int Easy;
    public int Hard;
    public int Extreme;
    public int KUJO;
}