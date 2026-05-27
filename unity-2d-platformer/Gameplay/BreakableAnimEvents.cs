using UnityEngine;

namespace Platformer
{
	public class BreakableAnimEvents : MonoBehaviour
	{
		[Header("Audio")]
		[SerializeField]
		private SoundEffect _sfxBreak;

		[SerializeField]
		private SoundEffect _sfxHit;

		[Header("Effects")]
		[SerializeField]
		private GameObject _vfxBreak;

		[SerializeField]
		private Transform _vfxBreakTransform;

		[SerializeField]
		private GameObject _vfxHit;

		[SerializeField]
		private Transform _vfxHitTransform;

		private void Start()
		{
			_sfxBreak.Initialize(transform, "breakable_break");
			_sfxHit.Initialize(transform, "breakable_hit");
		}

		private void AE_Break()
		{
			_sfxBreak?.Play();
			if (_vfxBreak != null)
			{
				Instantiate(_vfxBreak, _vfxBreakTransform);
			}
		}

		private void AE_Hit()
		{
			_sfxHit?.Play();
			if (_vfxHit != null)
			{
				Instantiate(_vfxHit, _vfxHitTransform);
			}
		}
	}
}