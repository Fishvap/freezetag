using Sandbox;
using Sandbox.UI;

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