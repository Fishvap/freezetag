using Sandbox;
using System;
using System.Linq;
using Sandbox.Component;

namespace Sandbox;

partial class FreezetagPlayer : Player
{
	//VARIABLES
	public Color TeamColor = (Color)Color.Parse("#ffffff");
	public ClothingContainer Clothing = new();

	public FreezetagPlayer()
	{

	}

	protected override void UseFail()
	{
		PlaySound( "player_use_fail_new" );
		return;
	}

	public FreezetagPlayer( Client cl ):this()
	{
		Clothing.LoadFromClient( cl );
	}

	public string Playermodel = "models/citizen/citizen.vmdl";
	
	[Net,Predicted] public bool Frozen {get;set;} = false;
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	
	public override void Respawn()
	{
		Frozen = false;
		// DEFINE PLAYER
		CameraMode = new FirstPersonCamera();
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		// SET TAGS
		Tags.Add("player");
		SetModel( Playermodel );
		Clothing.DressEntity( this );

		if(Tags.Has("runner"))
			Runner();
		if(Tags.Has("tagger"))
			Tagger();

		base.Respawn();
	}

	public virtual void SetTeam()
	{
		var glow = Components.GetOrCreate<Glow>();
		glow.Active = true;
		glow.Color = TeamColor;
		glow.RangeMax = 10000000;
	}

	public virtual void Tagger()
	{
		TeamColor = (Color)Color.Parse("#ff0000");
		(Controller as WalkController).DefaultSpeed = 350.0f;
		(Controller as WalkController).SprintSpeed = 350.0f;
	}

	public virtual void Runner()
	{
		TeamColor = (Color)Color.Parse("#0051ff");
		(Controller as WalkController).DefaultSpeed = 300.0f;
		(Controller as WalkController).SprintSpeed = 300.0f;
	}

	public override void Simulate( Client cl )
	{
		var TagTrace = Trace.Ray(EyePosition, EyePosition + EyeRotation.Forward*200)
			.UseHitboxes()
			.Ignore(this)
			.Run();

		if(Input.Pressed(InputButton.PrimaryAttack))
		{
			if(Tags.Has("runner"))
					Runner();
				if(Tags.Has("tagger"))
					Tagger();
			if(TagTrace.Entity is FreezetagPlayer player)
				{
					var FreezeParticle = new Particles();
					if(Tags.Has("tagger") && player.Tags.Has("runner"))
					{
						player.UseAnimGraph = false;
						player.PlaySound("player_freeze");
						FreezeParticle = Particles.Create("particles/fire_extinguisher/extinguisher_destroy_fx_smoke.vpcf", player);
						player.Controller = null;
						Frozen = true;
						Task.Delay(20);
					}
					if(Tags.Has("runner") && player.Tags.Has("runner"))
					{
						if(Frozen)
							return;
						player.UseAnimGraph = true;
						player.PlaySound("player_thaw");
						Particles.Create("particles/impact.glass.vpcf", player);
						FreezeParticle.Destroy(true);
						player.Controller = new WalkController();
						Frozen = false;
						Task.Delay(20);
					}
				}
		}
	
		base.Simulate( cl );
		TickPlayerUse();
		SetTeam();

	}


	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
	}
}
