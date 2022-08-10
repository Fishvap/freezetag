using Sandbox;
using System;
using System.Linq;

namespace Freezetag
{
	public class Tagger : BasePlayer
	{
		public Tagger()
		{
		}
		
		public Tagger( Client cl ):this()
		{
			Clothing.LoadFromClient( cl );
		}
		
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			/*var Entities = FindInSphere( Position.z * 20.0f, 40.0f );
			DebugOverlay.Sphere( Position.z * 20.0f, 40.0f, Color.Orange );

			foreach ( var ent in Entities )
			{
				if( ent == this )
					continue;
				if ( cl.Pawn is not Runner pawn )
					continue;
				pawn.Controller = null;
				PlaySound("player_freeze");
			}*/
		}
	}
}