using Sandbox;

namespace Sandbox;

class Commands
{
    [ConCmd.Server( "team_runner" )]
    public static void AssignRunnerServer()
    {
        ConsoleSystem.Caller.Pawn.Tags.Add("runner");
        ConsoleSystem.Caller.Pawn.Tags.Remove("tagger");
    }

    [ConCmd.Server( "team_tagger" )]
    public static void AssignTaggerServer()
    {
        ConsoleSystem.Caller.Pawn.Tags.Add("tagger");
        ConsoleSystem.Caller.Pawn.Tags.Remove("runner");
    }

    [ConCmd.Server( "roundlength" )]
    public static void SetRoundLength( string Time )
    {
        float RoundTime = Time.ToFloat();
        Freezetag.RoundTimerLength = RoundTime;
    }
}