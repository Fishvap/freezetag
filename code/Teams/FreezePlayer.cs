namespace Freezetag
{
    public partial class FreezeBasePlayer
    {
        public bool IsFrozen = false;
        Vector3 PreviousPosition = new();
        public Glow glow;
        public bool TaggerCanFreezeRunner = false;

        public void SetupRound()
        {
            IsFrozen = false;
            clothing.LoadFromClient( Client );
            SetModel( "models/citizen/citizen.vmdl" );
            clothing.DressEntity( this );
            Tags.Clear();
            
            EnableAllCollisions = true;
            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;
            EnableDrawing = true;

            // CreateHull();

            glow = Components.GetOrCreate<Glow>();
            glow.Active = true;
            glow.RangeMax = 10000000;
            
            SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

            ResetInterpolation();
        }
  
        public void SetupTagger()
        {
            CurrentTeam = TeamEnum.Tagger;
            
            SetupRound();
            CameraMode = new ThirdPersonCamera();
            Animator = new StandardPlayerAnimator();
            Controller = new WalkController();
            
            (Controller as WalkController).SprintSpeed = 450.0f;
            (Controller as WalkController).DefaultSpeed = 300.0f;
            (Controller as WalkController).Gravity = 600.0f;
            Tags.Add("tagger");
            glow.Color = Color.Red;
        }

        public void SetupRunner()
        {
            CurrentTeam = TeamEnum.Runner;

            SetupRound();
            CameraMode = new FirstPersonCamera();
            Animator = new StandardPlayerAnimator();
            Controller = new WalkController();
            
            (Controller as WalkController).SprintSpeed = 400.0f;
            (Controller as WalkController).DefaultSpeed = 250.0f;
            (Controller as WalkController).Gravity = 500.0f;
            Tags.Add("runner");
            glow.Color = Color.Cyan;
        }
        
        public void SetupSpectator()
        {
            CurrentTeam = TeamEnum.Spectator;
            
            IsFrozen = false;
            SetModel( null );
            Tags.Clear();
            
            EnableAllCollisions = false;
            EnableDrawing = false;

            CameraMode = new FirstPersonCamera();
            Controller = new NoclipController();
            Tags.Add("spectator");
        }

        public void Freeze( FreezeBasePlayer player )
        {
            player.IsFrozen = true;
            player.CameraMode = new ThirdPersonCamera();
            player.Controller = null;
            PlaySound("audio/player_freeze");
            Particles.Create("particles/impact.generic.smokering.vpcf", player.Position + Vector3.Up * 40.0f);
        }

        public void Thaw( FreezeBasePlayer player )
        {
            player.IsFrozen = false;
            player.CameraMode = new FirstPersonCamera();
            player.Controller = new WalkController();
            PlaySound("player_thaw");
            Particles.Create("particles/impact.glass.vpcf", player.Position + Vector3.Up * 40.0f);
            (player.Controller as WalkController).SprintSpeed = 350.0f;
            (player.Controller as WalkController).WalkSpeed = 250.0f;
            (player.Controller as WalkController).Gravity = 500.0f;
        }

        public override void Simulate( Client cl )
        {

            Vector3 PosLerp = new Vector3(PreviousPosition + Vector3.Up * 40.0f);
            Vector3 LerpPosition = PreviousPosition - Position;
            LerpPosition = LerpPosition.ClampLength(1.0f);

            var sphereCheck = FindInSphere(PosLerp.LerpTo(Position + Vector3.Up * 40.0f, 1.0f - LerpPosition.Length), 15f);
            /*if(IsServer)
            {
                if(Tags.Has("tagger"))
                    DebugOverlay.Sphere(PosLerp.LerpTo(Position + Vector3.Up * 40.0f, 1.0f - LerpPosition.Length), 15f, Color.Red);
                if(Tags.Has("runner"))
                    DebugOverlay.Sphere(PosLerp.LerpTo(Position + Vector3.Up * 40.0f, 1.0f - LerpPosition.Length), 15f, Color.Cyan); fishvap do be gay
            }*/

            PreviousPosition = Position;

            foreach(var ent in sphereCheck)
            {
                if(ent == cl.Pawn)
                    break;
                if(ent is not FreezeBasePlayer player)
                    break;
                if(CurrentTeam == TeamEnum.Tagger && player.CurrentTeam == TeamEnum.Runner)
                {
                    if(player.IsFrozen == false && TaggerCanFreezeRunner == true)
                    {
                        Log.Info( $"{Client.Name} froze {player.Client.Name}" );
                        Freeze( player );
                    }
                }
                if(CurrentTeam == TeamEnum.Runner && player.CurrentTeam == TeamEnum.Runner)
                {
                    if(player.IsFrozen == true && IsFrozen == false)
                    {
                        Log.Info( $"{Client.Name} thawed {player.Client.Name}" );
                        Thaw( player );
                    }
                }
            }

            base.Simulate( cl );

            /*CurrentClothingData = cl.GetClientData( "avatar" );
            if( CurrentClothingData != PreviousClothingData )
            {
                clothing.LoadFromClient( cl );
                SetModel( "models/citizen/citizen.vmdl" );
                clothing.DressEntity( this );
            }
            PreviousClothingData = CurrentClothingData;*/
        }

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );
		}

    }
}