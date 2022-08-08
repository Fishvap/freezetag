using Sandbox;

namespace Freezetag
{
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
        }
    }
}