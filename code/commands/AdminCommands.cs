using Sandbox;
using System;
using System.Linq;

namespace Freezetag
{
    public class AdminCommands
    {
        [ConCmd.Server( "team" )]
        public static void Team( string Str )
        {
            var client = ConsoleSystem.Caller;
            if( client.Pawn is not FreezeBasePlayer player )
                return;
            client.Pawn.DeleteAsync(25.0f);

            Str = Str.ToUpper();
            try {
                switch( Str )
                {
                    case "SPECTATOR":
                        player.SetupSpectator();
                        break;
                    case "RUNNER":
                        player.SetupRunner();
                        break;
                    case "TAGGER":
                        player.SetupTagger();
                        break;
                    default:
                        Log.Info( "Your team was not set because it was an invalid team." );
                        return;
                }
            }
            catch {
                Log.Info( $"There was an error when trying to run the team command, likely due to an internal error." ); 
            }
            Log.Info( $"Your team was set to [{Str}]" );
        }
    }
}