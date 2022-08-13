global using Sandbox;
global using Sandbox.Component;
global using System;
global using System.Collections.Generic;
global using System.Linq;

namespace Freezetag
{
    public partial class FreezeBasePlayer : Player
    {
        [Net] public TimeSince TimeSinceDead {get; set;}
        public ClothingContainer clothing = new();
        public string PreviousClothingData;
        public string CurrentClothingData;
        public enum TeamEnum
        {
            Spectator,
            Runner,
            Tagger
        }
        
        //The players current team, default to spectator upon joining
        //Networked to the client
        //The change is used whenever the variable is changed, use the method
        [Net, Change( nameof( UpdateTeamNotify ))]
	    public TeamEnum CurrentTeam { get; set; } = TeamEnum.Runner;

        public FreezeBasePlayer()
        {
        }

        public override void OnKilled()
        {
            if(CurrentTeam == TeamEnum.Spectator)
                return;
            BecomeRagdollOnClient( Velocity, 0, 0, 0, GetHitboxBone( 0 ) );
            PlaySound("player_death");

            Controller = null;

            EnableAllCollisions = false;
            EnableDrawing = false;

            CameraMode = new SpectateRagdollCamera();

            foreach ( var child in Children )
            {
                child.EnableDrawing = false;
            }

            base.OnKilled();
        }

        public FreezeBasePlayer( Client cl ):this()
        {
            clothing.LoadFromClient( cl );
            PreviousClothingData = cl.GetClientData( "avatar" );
        }

        //Debugging notification in console, used to tell when players change teams to what
        public void UpdateTeamNotify(TeamEnum oldTeam, TeamEnum newTeam)
        {
            Log.Info( $"{Client.Name} changed from {oldTeam} to {newTeam}" );
        }

        /*public override void OnKilled()
        {
            Game.Current?.OnKilled( this );
            
            TimeSinceDead = 0;
            LifeState = LifeState.Dead;
            StopUsing();

            EnableDrawing = false;
        }*/

        public void ChangeTeam()
        {
            switch( CurrentTeam )
            {
                case TeamEnum.Spectator:
                    SetupSpectator();
                    break;
                case TeamEnum.Runner:
                    SetupRunner();
                    break;
                case TeamEnum.Tagger:
                    SetupTagger();
                    break;
            }
        }

        public override void Respawn()
        {
            base.Respawn();
            ChangeTeam();
        }
    }
}