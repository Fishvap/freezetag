using Sandbox;
using Sandbox.UI;

<<<<<<< Updated upstream
namespace Freezetag
=======
namespace Freezetag;

public partial class HUD : HudEntity<RootPanel>
>>>>>>> Stashed changes
{
    public partial class HUD : HudEntity<RootPanel>
    {
        public HUD()
        {
            if (!IsClient)
                return;
            RootPanel.AddChild<ChatBox>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<VoiceSpeaker>();
            RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
        }
    }
}