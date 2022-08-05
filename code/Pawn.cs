using Sandbox;
using System;
using System.Linq;
using Sandbox.Component;

namespace Sandbox;

partial class Pawn : Player
{
	//VARIABLES
	public Color TeamColor = (Color)Color.Parse("#ffffff");
	public ClothingContainer Clothing { get; protected set; }

	public Pawn()
	{
		
	}

	protected override void UseFail()
	{
		return;
	}

	public Pawn( Client cl ):this()
	{

	}

	public string Playermodel = "models/citizen/citizen.vmdl";
	
	[Net,Predicted] public bool Frozen {get;set;} = false;
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	
	public override void Spawn()
	{
		// DEFINE PLAYER
		CameraMode = new FirstPersonCamera();
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		
		EnableHitboxes = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		SurroundingBoundsMode = SurroundingBoundsType.Hitboxes;
		EnableAllCollisions = true;

		// SET PLAYERMODEL
		SetModel( Playermodel );

		// SET TAGS
		Tags.Add("player");

		base.Spawn();
	}

	public virtual void SetTeam()
	{
		var glow = Components.GetOrCreate<Glow>();
		glow.Active = true;
		glow.Color = (Color)TeamColor;
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

	public override void Respawn()
	{
		base.Respawn();
		if(Tags.Has("runner"))
			Runner();
		if(Tags.Has("tagger"))
			Tagger();
	}

	public override void Simulate( Client cl )
	{
		var TagTrace = Trace.Ray(EyePosition, EyePosition + EyeRotation.Forward*200)
			.UseHitboxes()
			.WithTag("player")
			.Ignore(this)
			.Run();

		if(Input.Pressed(InputButton.PrimaryAttack))
		{
			Log.Info(Host.Name); 
			Log.Info(TagTrace.Entity);
			Log.Info("------");
			if(TagTrace.Entity is Pawn player)
				{
					var FreezeParticle = new Particles();
					if(Tags.Has("tagger") && player.Tags.Has("runner") && !Frozen)
					{
						Frozen = true;
						player.UseAnimGraph = false;
						player.PlaySound("player_freeze");
						FreezeParticle = Particles.Create("particles/fire_extinguisher/extinguisher_destroy_fx_smoke.vpcf", player);
						player.Controller = null;
					}
					if(Tags.Has("runner") && player.Tags.Has("runner") && Frozen)
					{
						Frozen = false;
						player.UseAnimGraph = true;
						player.PlaySound("player_thaw");
						Particles.Create("particles/impact.glass.vpcf", player);
						FreezeParticle.Destroy(true);
						player.Controller = new WalkController();
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
