global using Sandbox;
global using Sandbox.Component;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

namespace Sandbox;

public partial class FreezetagPlayer : Player
{
	// Defining Variables
	[Net,Predicted] public bool Frozen {get;set;} = false;
	public ClothingContainer Clothing = new();

	public FreezetagPlayer() {
	}

	protected override void UseFail() {
		PlaySound( "player_use_fail_new" );
		return;
	}

	public FreezetagPlayer( Client cl ):this() {
		Clothing.LoadFromClient( cl );
	}

	public override void Respawn() {
		Frozen = false;
		// DEFINE PLAYER
		Controller = new WalkController();
		CameraMode = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();

		UseAnimGraph = true;
		EnableAllCollisions = true;
		EnableHitboxes = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		// SET TAGS
		Tags.Add("player");
		SetModel( "models/citizen/citizen.vmdl" );
		Clothing.LoadFromClient( Client );
		Clothing.DressEntity( this );

		base.Respawn();
		SetTeam();
	}

	/// <summary>
	/// Called when the entity is first created 
	/// </summary>

	public override void Simulate( Client cl ) {
		//if(IsServer)
		//	Log.Info(Freezetag.StateTimer);

		if (Input.Pressed( InputButton.View))
		{
			if ( CameraMode is ThirdPersonCamera )
				CameraMode = new FirstPersonCamera();
			else
				CameraMode = new ThirdPersonCamera();
		}

		base.Simulate( cl );
		TickPlayerUse();

		var TagTrace = Trace.Ray(EyePosition, EyePosition + EyeRotation.Forward*50)
			.UseHitboxes()
			.Ignore(this)
			.Run();

		if(Input.Pressed(InputButton.PrimaryAttack))
		{
			if(TagTrace.Entity is FreezetagPlayer player)
				{
					var FreezeParticle = new Particles();
					if(Tags.Has("tagger") && player.Tags.Has("runner"))
					{
						if(player.Tags.Has("Frozen"))
							return;
						player.PlaySound("player_freeze");
						FreezeParticle = Particles.Create("particles/fire_extinguisher/extinguisher_destroy_fx_smoke.vpcf", player.EyePosition);
						player.Controller = null;
						player.Tags.Add("frozen");
					}
					if(Tags.Has("runner") && player.Tags.Has("runner"))
					{
						if(!player.Tags.Has("Frozen"))
							return;
						player.PlaySound("player_thaw");
						Particles.Create("particles/impact.glass.vpcf", player.EyePosition);
						FreezeParticle.Destroy(true);
						player.Controller = new WalkController();
						player.Tags.Remove("frozen");
					}
				}
		}
	}
	private void BecomeRagdollOnClient( Vector3 velocity )
	{
		var ent = new ModelEntity();
		ent.Tags.Add( "ragdoll", "solid", "debris" );
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.Scale = Scale;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.CopyBodyGroups( this );
		ent.CopyMaterialGroup( this );
		ent.CopyMaterialOverrides( this );
		ent.TakeDecalsFrom( this );
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;
		ent.RenderColor = RenderColor;
		ent.PhysicsGroup.Velocity = velocity;
		ent.PhysicsEnabled = true;

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			var model = e.GetModelName();

			var clothing = new ModelEntity();
			clothing.SetModel( model );
			clothing.SetParent( ent, true );
			clothing.RenderColor = e.RenderColor;
			clothing.CopyBodyGroups( e );
			clothing.CopyMaterialGroup( e );
		}
		ent.DeleteAsync( 10.0f );
	}

	public override void OnKilled()
	{
		base.OnKilled();
		BecomeRagdollOnClient( Velocity );
		PlaySound("player_death");

		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;

		CameraMode = new SpectateRagdollCamera();

		foreach ( var child in Children )
		{
			child.EnableDrawing = false;
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( Client cl ) {
		base.FrameSimulate( cl );
	}
}
