using Sandbox;
using System;
using System.Linq;
using Sandbox.Component;

namespace Sandbox;

public class FreezetagBasePlayer : Player
{
	private ClothingContainer Clothing;

	public FreezetagBasePlayer()
	{

	}

	public FreezetagBasePlayer( Client cl ):this()
	{
		Clothing.LoadFromClient( cl );
	}

	public override void Respawn()
	{
		// DEFINE PLAYER
		SetModel( "models/citizen/citizen.vmdl" );
		Controller = new WalkController();
		CameraMode = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();

		if( DevController is NoclipController )
		{
			DevController = null;
		}
		/*
		Clothing ??= new ClothingContainer();
		Clothing.LoadFromClient( Client );
		Clothing.DressEntity( this );
		*/

		UseAnimGraph = true;
		EnableHitboxes = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		base.Respawn();
	}
}
