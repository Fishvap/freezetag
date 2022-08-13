namespace Freezetag
{
    public partial class Freezetag
    {
        // Gameloop Variables
        [Net] public static GameStates GameState {get;set;} = GameStates.Preround;
        [Net] public static RealTimeUntil StateTimer {get;set;} = 0.0f;
        [Net] public static float RoundTimerLength {get;set;} = 30.0f;
        [Net] public static int FrozenPlayers {get;set;} = 0;
        public static float PreroundTimer {get;set;} = 10.0f;
        public int MaxPlayers = short.Parse(ConsoleSystem.GetValue( "maxplayers" ));
        public double TaggerNumber = Math.Ceiling(Client.All.Count / 4.0f);
        public enum GameStates {
            Preround,
            Round,
            Endround
        }
        
        public int NumberOfRunners = 0;
        
        /// <summary>
        /// A client has joined the server. Make them a pawn to play with
        /// </summary>

        public virtual void WaitingForPlayers()
        {

        }

        public virtual void StartRound()
        {
            TaggerNumber = Math.Ceiling(Client.All.Count / 4.0f);
            foreach ( var client in Client.All )
            {
                if ( client.Pawn is not FreezeBasePlayer pawn )
                continue;
                pawn.Respawn();
            }
        }

        public virtual void EndRound()
        {
            foreach ( var client in Client.All )
            {
                if ( client.Pawn is not FreezeBasePlayer pawn )
                continue;
                pawn.Tags.Clear();
                pawn.Tags.Add("spectator");
                Log.Info("Removed tags!");
            }
            FrozenPlayers = 0;
        }

        public async Task GameStateTimer()
        {
            do
            {
                await Task.DelayRealtimeSeconds(1.0f);
                if(GameState == GameStates.Round && FrozenPlayers == NumberOfRunners)
                {
                    StateTimer = 0.0f;
                    FrozenPlayers = 0;
                }
            } while(StateTimer > 0);
        }

        public async Task GameLoopAsync()
        {
            if(Client.All.Count > 1)
            {
                GameState = GameStates.Preround;
                StateTimer = PreroundTimer;
                await GameStateTimer();
                PreroundTimer = 3.0f;

                GameState = GameStates.Round;
                StartRound();
                var players = Client.All.Select( c => c.Pawn as FreezeBasePlayer );
                foreach ( var player in players )
                {
                    player.Respawn();
                }
                StateTimer = RoundTimerLength;
                await GameStateTimer();

                GameState = GameStates.Endround;
                StateTimer = 3.0f;
                EndRound();
                Log.Info("End Round");
                await GameStateTimer();
                await GameLoopAsync();
            }
        }
    }
}