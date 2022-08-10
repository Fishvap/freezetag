using Sandbox;

namespace Freezetag
{
<<<<<<< Updated upstream
    class Commands
    {
        [ConCmd.Server( "setteam" )]
        public static void AssignRunnerServer( string teamname )
        {
            var client = ConsoleSystem.Caller;
            client.Pawn.Delete();

            switch(teamname)
            {
                case "Spectator":
                {
                    var pawn = new Spectator();
                    client.Pawn = pawn;
                    pawn.Respawn();
                    break;
                }
                case "Tagger":
                {
                    var pawn = new Tagger();
                    client.Pawn = pawn;
                    pawn.Respawn();
                    break;
                }
                case "Runner":
                {
                    var pawn = new Runner();
                    client.Pawn = pawn;
                    pawn.Respawn();
                    break;
                }
                default:
                {
                    Log.Info( "setteam failed: " + teamname + " doesn't exist." );
                    break;
                }
            }
        Log.Info( "Your team was set to: " + teamname );
=======
    public partial class Freezetag
    {
        [ConCmd.Server("team")]
        public static void SetTeam( string arg )
        {
            if ( ConsoleSystem.Caller.Pawn is not BasePlayer player )
                return;

            arg = arg.ToUpper().Trim();

            switch(arg)
            {
                case "RUNNER":
                    player.CurrentTeam = BasePlayer.TagTeamEnum.Runner;
                    break;
                case "TAGGER":
                    player.CurrentTeam = BasePlayer.TagTeamEnum.Tagger;
                    break;
                case "SPECTATOR" :
                    player.CurrentTeam = BasePlayer.TagTeamEnum.Spectator;
                    break;
                default:
                    Log.Info( $"Failed to set your team to {arg}" );
                    return;
            }
            Log.Info( $"Your team was set to {arg}" );
            player.Respawn();
        }

        [ConCmd.Admin("debugmode")]
        public static void DebugCMD()
        {
            Debug = !Debug;
>>>>>>> Stashed changes
        }
    }
}