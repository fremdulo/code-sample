using UnityEngine;

namespace Platformer
{
	public class DamageZoneController : MonoBehaviour
	{
		[SerializeField]
		private LayerMask _layers;

		[SerializeField]
		private SoundEffect _sound;

		[SerializeField]
		private TransitionAsset _transitionAsset;

		// TODO - remove
		[SerializeField]
		private string _animEventName;

		private void Start()
		{
			_sound?.Initialize(transform, "DamageZoneSound");
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			IDamageZoneEnterer enterer = other?.gameObject?.GetComponent<IDamageZoneEnterer>();

			if (enterer != null && ((1 << other.gameObject.layer) & _layers.value) != 0)
			{
				enterer.OnDamageZoneEntered(this, _transitionAsset);
				_sound?.Play();
			}
		}
	}
}