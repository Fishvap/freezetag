namespace Sandbox;

public partial class Freezetag : Sandbox.Game
{
    [Net] public string SongChoice {get;set;}

    public virtual void PlayMusic()
    {
        var SongList5mins = new List<string> {
            "sounds/plok_beach.mp3",
            "sounds/sonic_windmill_isle.mp3"
        };
        Random RNG = new();
        int SongNumber = RNG.Next(SongList5mins.Count);
		SongChoice = SongList5mins[SongNumber];
    }
}