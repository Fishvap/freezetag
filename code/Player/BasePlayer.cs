global using Sandbox;
global using System;
global using System.Linq;

namespace Freezetag
{
	public partial class BasePlayer : Player
	{
		public enum TagTeamEnum {
			Spectator,
			Runner,
			Tagger
		}
		public TagTeamEnum CurrentTeam = TagTeamEnum.Runner;
		public ClothingContainer Clothing = new();

		public BasePlayer()
		{
			
		}

		public BasePlayer( Client cl ):this()
		{
			Clothing.LoadFromClient( cl );
		}

		public void SpawnAtPoint()
		{
			// Get all of the spawnpoints
			var spawnpoints = Entity.All.OfType<SpawnPoint>().OrderBy(x => Guid.NewGuid()).FirstOrDefault();

			// if it exists, place the pawn there
			if (spawnpoints != null)
			{
				var tx = spawnpoints.Transform;
				tx.Position += Vector3.Up * 50.0f; // raise it up
				Transform = tx;
			}
		}

		/// <summary>
		/// Called when the entity is first created 
		/// </summary>
		public override void Respawn()
		{
			base.Respawn();
			// Checks if their current team is not Spectator,
			// else they are by default a Spectator
			if( CurrentTeam != TagTeamEnum.Spectator )
			{
				// TODO: Custom playermodels?
				SetModel( "models/citizen/citizen.vmdl" );
				
				Clothing.LoadFromClient( Client );
				Clothing.DressEntity( this );

				Controller = new WalkController();
				CameraMode = new FirstPersonCamera();
				Animator = new StandardPlayerAnimator();
				
				EnableHitboxes = true;
				EnableDrawing = true;
				EnableHideInFirstPerson = true;
				EnableShadowInFirstPerson = true;
			}
			else
			{
				Controller = new NoclipController();
				CameraMode = new FirstPersonCamera();
				Animator = new StandardPlayerAnimator();
				
				EnableAllCollisions = false;
				EnableDrawing = false;
			}
		}
	}
}	


