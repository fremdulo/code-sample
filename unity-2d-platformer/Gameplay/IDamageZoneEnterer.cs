namespace Platformer
{
	public interface IDamageZoneEnterer
	{
		void OnDamageZoneEntered(DamageZoneController damageZone, TransitionAsset asset);
	}
}