using UnityEngine;

namespace Platformer
{
	public class FallThroughAnimEvents : MonoBehaviour
	{
		[Header("Audio")]
		[SerializeField]
		private SoundEffect _sfxBreak1;

		[SerializeField]
		private SoundEffect _sfxBreak2;

		[SerializeField]
		private SoundEffect _sfxBreak3;

		[Header("Effects")]
		[SerializeField]
		private GameObject _vfxBreak1;

		[SerializeField]
		private GameObject _vfxBreak2;

		[SerializeField]
		private GameObject _vfxBreak3;

		[SerializeField]
		private Transform _vfxBreak1Transform;

		[SerializeField]
		private Transform _vfxBreak2Transform;

		[SerializeField]
		private Transform _vfxBreak3Transform;

		private FallThroughController _fallthrough;

		private void Start()
		{
			_fallthrough = GetComponent<FallThroughController>();

			_sfxBreak1.Initialize(transform, "fallthrough_break1");
			_sfxBreak2.Initialize(transform, "fallthrough_break2");
			_sfxBreak3.Initialize(transform, "fallthrough_break3");
		}

		private void AE_Break1()
		{
			_sfxBreak1?.Play();
			if (_vfxBreak1 != null)
			{
				Instantiate(_vfxBreak1, _vfxBreak1Transform);
			}
		}

		private void AE_Break2()
		{
			_sfxBreak2?.Play();
			if (_vfxBreak2 != null)
			{
				Instantiate(_vfxBreak2, _vfxBreak2Transform);
			}
		}

		private void AE_Break3()
		{
			_sfxBreak3?.Play();
			if (_vfxBreak3 != null)
			{
				Instantiate(_vfxBreak3, _vfxBreak3Transform);
			}
			_fallthrough?.InvokeBreakAction();
		}
	}
}