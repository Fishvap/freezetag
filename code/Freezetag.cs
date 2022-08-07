global using Sandbox;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Sandbox;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class Freezetag : Sandbox.Game
{
	// Gameloop Variables
    [Net] public static GameStates GameState {get;set;} = GameStates.Preround;
    [Net] public static RealTimeUntil StateTimer {get;set;} = 0.0f;
    [Net] public static float RoundTimerLength {get;set;} = 10.0f;
	public static float PreroundTimer {get;set;} = 10.0f;
	public int MaxPlayers = short.Parse(ConsoleSystem.GetValue( "maxplayers" ));
	public int Playercount = 0;
	public float PlayerTagging = 0.0f;
	public double TaggerNumber = Math.Ceiling(Client.All.Count / 4.0f);
	public int HasBeenTagged = 0;
	public int SkipTag = 0;

	public Freezetag()
	{
		if(IsServer)
		{
			_ = new HUD();

			_ = GameLoopAsync();
		}
	}
	
	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new FreezetagPlayer( client );
		client.Pawn = pawn;

		// Get all of the spawnpoints
		var spawnpoints = All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}

		var player = new FreezetagPlayer();
		pawn.Respawn();
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );
	}

	public virtual void StartRound()
	{
		SkipTag = HasBeenTagged;
		foreach ( var client in Client.All )
		{
			
			if ( client.Pawn is not FreezetagPlayer pawn )
        	continue;
			if(HasBeenTagged == 0)
				if(TaggerNumber > 0)
				{
					TaggerNumber -= 1;
					pawn.Tags.Add("tagger");
					Log.Info("Assigned a tagger!");
				}
			else
			{
				if(HasBeenTagged > 0)
					HasBeenTagged = HasBeenTagged - 1;
				pawn.Tags.Add("runner");
				Log.Info("Assigned a runner!");
			}
			pawn.Respawn();
			pawn.SetTeam();
		}
		HasBeenTagged = SkipTag + 1;
		TaggerNumber = Math.Ceiling(Client.All.Count / 4.0f);
		SkipTag = HasBeenTagged;
		if(SkipTag == Client.All.Count)
			SkipTag = 0;
		
	}

	public virtual void EndRound()
	{
		foreach ( var client in Client.All )
		{
			if ( client.Pawn is not FreezetagPlayer pawn )
        	continue;
			pawn.Tags.Remove("runner");
			pawn.Tags.Remove("tagger");
			Log.Info("Removed tags!");
		}
	}

    public async Task GameStateTimer()
    {
        do
        {
            await Task.DelayRealtimeSeconds(1.0f);
        } while(StateTimer > 0);
    }

    public async Task GameLoopAsync()
    {
        GameState = GameStates.Preround;
        StateTimer = PreroundTimer;
        await GameStateTimer();
		PreroundTimer = 3.0f;

        GameState = GameStates.Round;
		StartRound();
		Log.Info("Start Round");
		var players = Client.All.Select( c => c.Pawn as FreezetagPlayer );
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

    public enum GameStates {
        Preround,
        Round,
        Endround
    }
}

