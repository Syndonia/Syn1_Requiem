using Sandbox;

public sealed class HealTrigger : Component.ITriggerListener
{
	[Property] float Amount { get; set; } = 10f;
	public void OnTriggerEnter( Collider other )
	{
		var player = other.Components.Get<CsPlayermovement>();
		if ( player != null )
		{
			player.Health += Amount;
			player.Health = MathX.Clamp( player.Health, 0, player.MaxHealth );
		}
	}

	public void OnTriggerExit( Collider other )
	{

	}
}
