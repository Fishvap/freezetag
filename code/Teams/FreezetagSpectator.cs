using Sandbox;
using System;
using System.Linq;
using Sandbox.Component;

namespace Freezetag
{
	class Spectator : Player
	{
		public Spectator()
		{
			base.Respawn();
		}

		public override void Respawn()
		{
			// DEFINE PLAYER
			CameraMode = new FirstPersonCamera();
			Controller = new NoclipController();
			Animator = new StandardPlayerAnimator();
			
			EnableAllCollisions = false;
			EnableDrawing = false;

			base.Respawn();
		}
	}
}