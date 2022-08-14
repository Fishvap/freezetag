global using Sandbox;
global using Sandbox.UI.Construct;
global using System;
global using System.Linq;
global using System.Threading.Tasks;

using Freezetag.Util;

namespace Freezetag
{
	public partial class Freezetag : Game
	{
		public Freezetag()
		{
			Log.Debug("Game created, round started");
			if(IsServer)
				_ = new HUD();

				_ = GameLoopAsync();
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			// Create a pawn for this client to play with
			var pawn = new FreezeBasePlayer( client );
			client.Pawn = pawn;

			// Get all of the spawnpoints
			var spawnpoints = Entity.All.OfType<SpawnPoint>();
 
			// chose a random one
			var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

			// if it exists, place the pawn there
			if ( randomSpawnPoint != null )
			{
				var tx = randomSpawnPoint.Transform;
				tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
				pawn.Transform = tx;
			}
			
			pawn.Respawn();
		}
	}
}