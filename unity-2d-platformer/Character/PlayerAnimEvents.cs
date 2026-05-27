using UnityEngine;

namespace Platformer
{
	// TODO - move spawn effect locations to locator sub objects on the Player, and account for facing

	public class PlayerAnimEvents : MonoBehaviour
	{
		[Header("Audio")]
		[SerializeField]
		private SoundEffect _sfxDash;

		[SerializeField]
		private SoundEffect _sfxJump;

		[SerializeField]
		private SoundEffect _sfxLanding;

		[SerializeField]
		private SoundEffect _sfxSwing;

		[SerializeField]
		private SoundEffect _sfxWallSlide;

		[Header("Effects")]
		[SerializeField]
		private GameObject _vfxJumpDust;

		[SerializeField]
		private GameObject _vfxLandingDust;

		[SerializeField]
		private GameObject _vfxRunStopDust;

		[SerializeField]
		private GameObject _vfxWallJumpDust;

		[Header("Dissolve")]
		[SerializeField]
		private string _propertyStartTime = "_StartTime";

		[SerializeField]
		private string _propertyDissolveIn = "_DissolveIn";

		[Header("Dissolve Fire")]
		[SerializeField]
		private Material _dissolveMaterial_Fire;

		[SerializeField]
		private GameObject _vfxDissolveOut_Fire;

		[Header("Dissolve Blood")]
		[SerializeField]
		private Material _dissolveMaterial_Blood;

		[SerializeField]
		private GameObject _vfxDissolveOut_Blood;

		[Header("Dependencies")]
		[SerializeField]
		private PlayerController _player;

		private bool _isInTransitionAnim;
		public bool IsInTransitionAnim { get { return _isInTransitionAnim; } set { _isInTransitionAnim = value; } }

		private Material _originalMaterial;
		private SpriteRenderer _spriteRenderer;

		private void Start()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_originalMaterial = _spriteRenderer.material;

			_sfxDash.Initialize(transform, "player_dash");
			_sfxJump.Initialize(transform, "player_jump");
			_sfxLanding.Initialize(transform, "player_landing");
			_sfxSwing.Initialize(transform, "player_swing");
			_sfxWallSlide.Initialize(transform, "player_wallslide");

			Debug.Assert(_player != null);
		}

		public void StartWallSlide()
		{
			if (_sfxWallSlide != null && !_sfxWallSlide.IsPlaying())
			{
				_sfxWallSlide.Play();
			}
		}

		public void StopWallSlide()
		{
			if (_sfxWallSlide != null && _sfxWallSlide.IsPlaying())
			{
				_sfxWallSlide.Stop();
			}
		}

		private void AE_Dash()
		{
			_sfxDash?.Play();
		}

		private void AE_DissolveOut_Fire_Start()
		{
			if (_spriteRenderer != null && _dissolveMaterial_Fire != null)
			{
				_spriteRenderer.material = _dissolveMaterial_Fire;
				_dissolveMaterial_Fire.SetFloat(_propertyStartTime, Time.time);
				_dissolveMaterial_Fire.SetInt(_propertyDissolveIn, 0);
			}

			if (_vfxDissolveOut_Fire != null)
			{
				Instantiate(_vfxDissolveOut_Fire, transform.position, Quaternion.identity);
			}
		}

		private void AE_DissolveOut_Fire_End()
		{
			if (_spriteRenderer != null)
			{
				_spriteRenderer.material = _originalMaterial;
			}
		}

		private void AE_DissolveOut_Blood_Start()
		{
			if (_spriteRenderer != null && _dissolveMaterial_Blood != null)
			{
				_spriteRenderer.material = _dissolveMaterial_Blood;
				_dissolveMaterial_Blood.SetFloat(_propertyStartTime, Time.time);
				_dissolveMaterial_Blood.SetInt(_propertyDissolveIn, 0);
			}

			if (_vfxDissolveOut_Blood != null)
			{
				Instantiate(_vfxDissolveOut_Blood, transform.position, Quaternion.identity);
			}
		}

		private void AE_DissolveOut_Blood_End()
		{
			if (_spriteRenderer != null)
			{
				_spriteRenderer.material = _originalMaterial;
			}
		}

		// TODO - remove
		//private void AE_DissolveIn_Start()
		//{
		//	if (_spriteRenderer != null && _dissolveMaterial_Fire != null)
		//	{
		//		_spriteRenderer.material = _dissolveMaterial_Fire;
		//		_dissolveMaterial_Fire.SetFloat(_propertyStartTime, Time.time);
		//		_dissolveMaterial_Fire.SetInt(_propertyDissolveIn, 1);
		//	}
		//}

		//private void AE_DissolveIn_End()
		//{
		//	if (_spriteRenderer != null)
		//	{
		//		_spriteRenderer.material = _originalMaterial;
		//	}
		//}

		private void AE_Jump()
		{
			_sfxJump?.Play();

			if (_player.MovementController.StateJump != MovementController.JumpState.WallJumping)
			{
				_player.SpawnEffect(_vfxJumpDust, 0.0f, -0.55f);
			}
			else
			{
				_player.SpawnEffect(_vfxWallJumpDust, 0.0f, 0.0f);
			}
		}

		private void AE_Landing()
		{
			_sfxLanding?.Play();
			_player.SpawnEffect(_vfxLandingDust, 0.0f, -0.55f);
		}

		private void AE_RunStop()
		{
			_player.SpawnEffect(_vfxRunStopDust, 0.5f, -0.55f);
		}

		private void AE_Swing()
		{
			_sfxSwing?.Play();
		}

		private void AE_TransitionAnimStart()
		{
			_isInTransitionAnim = true;
		}

		private void AE_TransitionAnimEnd()
		{
			_isInTransitionAnim = false;
		}
	}
}