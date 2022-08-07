
namespace Sandbox;

public partial class FreezetagPlayer : Player
{
    public Color TeamColor = (Color)Color.Parse("#ffffff");

    public virtual void SetTeam()
	{
        var glow = Components.GetOrCreate<Glow>();
		glow.Active = true;
		glow.Color = TeamColor;
		glow.RangeMax = 10000000;

		if(Tags.Has("runner"))
			Runner();
		if(Tags.Has("tagger"))
			Tagger();
	}
	public virtual void Tagger()
	{
		TeamColor = (Color)Color.Parse("#ff0000");
		(Controller as WalkController).Gravity = 600.0f;
		(Controller as WalkController).DefaultSpeed = 350.0f;
		(Controller as WalkController).SprintSpeed = 350.0f;
	}
	public virtual void Runner()
	{
		TeamColor = (Color)Color.Parse("#0051ff");
		(Controller as WalkController).Gravity = 600.0f;
		(Controller as WalkController).DefaultSpeed = 300.0f;
		(Controller as WalkController).SprintSpeed = 300.0f;
	}
}