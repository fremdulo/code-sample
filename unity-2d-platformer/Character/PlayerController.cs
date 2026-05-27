using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
	public class PlayerController : MonoBehaviour, IMessageListener, IDamageZoneEnterer
	{
		[Flags]
		public enum ParalyzeFlags
		{
			None = 0x000000,
			Transition = 0x00000001,
		}

		// Inspector
		[Header("Debug")]
		[SerializeField]
		private bool _enableDebugVisualizations;

		[Header("Animation Variables")]
		[SerializeField]
		private string _animVarIsDashing = "isdashing";

		[SerializeField]
		private string _animVarIsMoving = "ismoving";

		[SerializeField]
		private string _animVarIsRising = "isrising";

		[SerializeField]
		private string _animVarOnGround = "onground";

		[SerializeField]
		private string _animVarOnWall = "onwall";

		[SerializeField]
		private string _animVarDashTrigger = "trigger_dash";

		[SerializeField]
		private string _animVarJumpTrigger = "trigger_jump";

		[SerializeField]
		private string _animVarSlashLeftTrigger = "trigger_left";

		[SerializeField]
		private string _animVarSlashRightTrigger = "trigger_right";

		[SerializeField]
		private string _animVarSlashUpTrigger = "trigger_up";

		[SerializeField]
		private string _animVarSlashDownTrigger = "trigger_down";

		[SerializeField]
		private string _animVarSwingLeftTrigger = "trigger_swing_left";

		[SerializeField]
		private string _animVarSwingRightTrigger = "trigger_swing_right";

		[SerializeField]
		private string _animVarSwingUpTrigger = "trigger_swing_up";

		[SerializeField]
		private string _animVarSwingDownTrigger = "trigger_swing_down";

		[SerializeField]
		private string _animVarWallSlideTrigger = "trigger_wallslide";

		[SerializeField]
		private float _swingUpDownThreshold = 0.7f;

		[Header("Camera")]
		[SerializeField]
		private GameObject _cameraTarget;
		public GameObject CameraTarget { get { return _cameraTarget; } }

		[SerializeField]
		private float _lookOffset;

		[Header("Dependencies")]
		[SerializeField]
		private PlayerAnimEvents _animEvents;
		public PlayerAnimEvents AnimEvents { get { return _animEvents; } }

		[SerializeField]
		private Animator _animatorPlayer;
		public Animator AnimatorPlayer { get { return _animatorPlayer; } }

		[SerializeField]
		private Animator _animatorSlash;
		public Animator AnimatorSlash { get { return _animatorSlash; } }

		[SerializeField]
		private SpriteRenderer _spriteRenderer;
		public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }

		[Header("Misc")]
		[SerializeField]
		private LR _flipFacing = LR.Left;

		// AnimIds
		private int _animIdIsDashing;
		private int _animIdIsMoving;
		private int _animIdIsRising;
		private int _animIdOnGround;
		private int _animIdOnWall;

		private int _animIdDashTrigger;
		private int _animIdJumpTrigger;
		private int _animIdSlashLeftTrigger;
		private int _animIdSlashRightTrigger;
		private int _animIdSlashUpTrigger;
		private int _animIdSlashDownTrigger;
		private int _animIdSwingLeftTrigger;
		private int _animIdSwingRightTrigger;
		private int _animIdSwingUpTrigger;
		private int _animIdSwingDownTrigger;
		private int _animIdWallSlideTrigger;

		// Components
		private MovementController _movementController;
		public MovementController MovementController { get { return _movementController; } }

		private SpriteRenderer _slashSprite;

		// Paralyze
		private ParalyzeFlags _paralyzeFlags;
		public bool IsParalyzed { get { return _paralyzeFlags != 0; } }

		// Interaction
		private HashSet<InteractableController> _activeInteractableControllers = new HashSet<InteractableController>();

		// Init/Deinit methods
		private void OnEnable()
		{
			Debug.Assert(GameController.Instance != null, "GameController instance found.", this);

			_movementController = GetComponent<MovementController>();
			Debug.Assert(_movementController != null, "MovementController instance found.", this);

			_slashSprite = _animatorSlash?.gameObject.GetComponent<SpriteRenderer>();
			Debug.Assert(_slashSprite != null, "SlashSprite instance found.", this);

			MessageManager mm = GameController.Instance.MessageManager;
			mm.RegisterListener(typeof(OnFacingChangedMessage), this);
			mm.RegisterListener(typeof(OnJumpStateChangedMessage), this);
			mm.RegisterListener(typeof(OnDashStateChangedMessage), this);
			mm.RegisterListener(typeof(OnSwingStateChangedMessage), this);
			mm.RegisterListener(typeof(MoveXMessage), this);
			mm.RegisterListener(typeof(OnHitMessage), this);

			_animIdIsDashing = Animator.StringToHash(_animVarIsDashing);
			_animIdIsMoving = Animator.StringToHash(_animVarIsMoving);
			_animIdIsRising = Animator.StringToHash(_animVarIsRising);
			_animIdOnGround = Animator.StringToHash(_animVarOnGround);
			_animIdOnWall = Animator.StringToHash(_animVarOnWall);

			_animIdDashTrigger = Animator.StringToHash(_animVarDashTrigger);
			_animIdJumpTrigger = Animator.StringToHash(_animVarJumpTrigger);
			_animIdWallSlideTrigger = Animator.StringToHash(_animVarWallSlideTrigger);

			_animIdSlashLeftTrigger = Animator.StringToHash(_animVarSlashLeftTrigger);
			_animIdSlashRightTrigger = Animator.StringToHash(_animVarSlashRightTrigger);
			_animIdSlashUpTrigger = Animator.StringToHash(_animVarSlashUpTrigger);
			_animIdSlashDownTrigger = Animator.StringToHash(_animVarSlashDownTrigger);

			_animIdSwingLeftTrigger = Animator.StringToHash(_animVarSwingLeftTrigger);
			_animIdSwingRightTrigger = Animator.StringToHash(_animVarSwingRightTrigger);
			_animIdSwingUpTrigger = Animator.StringToHash(_animVarSwingUpTrigger);
			_animIdSwingDownTrigger = Animator.StringToHash(_animVarSwingDownTrigger);
		}

		private void OnDisable()
		{
			MessageManager mm = GameController.Instance.MessageManager;
			mm.UnregisterListener(typeof(OnFacingChangedMessage), this);
			mm.UnregisterListener(typeof(OnJumpStateChangedMessage), this);
			mm.UnregisterListener(typeof(OnDashStateChangedMessage), this);
			mm.UnregisterListener(typeof(OnSwingStateChangedMessage), this);
			mm.UnregisterListener(typeof(MoveXMessage), this);
			mm.UnregisterListener(typeof(OnHitMessage), this);
		}

		// Mesage Handler
		public void OnMessage(Message message)
		{
			// Confirm this message came from this player
			Component msgComp = message.Sender as Component;
			PlayerController msgPlayer = msgComp?.gameObject?.GetComponent<PlayerController>();
			if (msgPlayer == this)
			{
				if (HandleFacingChange(message))
					return;

				if (HandleJumpStateChange(message))
					return;

				if (HandleDashStateChange(message))
					return;

				if (HandleSwingStateChange(message))
					return;

				if (HandleMoveXChange(message))
					return;
			}

			if (HandleOnHit(message))
				return;
		}

		// IDamageZoneEnterer
		public void OnDamageZoneEntered(DamageZoneController damageZone, TransitionAsset asset)
		{
			TransitionManager.Instance.RestoreCheckpoint(asset);
		}

		// Input Event Handlers
		public void OnDashInput(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				_movementController?.Dash();
			}
		}

		public void OnInteract(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				foreach (InteractableController interactable in _activeInteractableControllers)
				{
					interactable?.Interact(InteractionType.Input, gameObject);
				}
			}
		}

		public void OnInteractUp(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				foreach (InteractableController interactable in _activeInteractableControllers)
				{
					interactable?.Interact(InteractionType.InputUp, gameObject);
				}
			}
		}

		public void OnJumpInput(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				_movementController?.Jump();
			}
			else if (context.phase == InputActionPhase.Canceled)
			{
				_movementController?.StopJump();
			}
		}

		public void OnLookDown(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				if (_cameraTarget != null)
				{
					Vector2 v =_cameraTarget.transform.localPosition;
					v.y = -_lookOffset;
					_cameraTarget.transform.localPosition = v;
				}
			}
			else if (context.phase == InputActionPhase.Canceled)
			{
				if (_cameraTarget != null)
				{
					Vector2 v = _cameraTarget.transform.localPosition;
					v.y = 0;
					_cameraTarget.transform.localPosition = v;
				}
			}
		}

		public void OnLookUp(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				if (_cameraTarget != null)
				{
					Vector2 v = _cameraTarget.transform.localPosition;
					v.y = _lookOffset;
					_cameraTarget.transform.localPosition = v;
				}
			}
			else if (context.phase == InputActionPhase.Canceled)
			{
				if (_cameraTarget != null)
				{
					Vector2 v = _cameraTarget.transform.localPosition;
					v.y = 0;
					_cameraTarget.transform.localPosition = v;
				}
			}
		}

		public void OnMoveInput(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			Vector2 newValue = context.ReadValue<Vector2>();
			_movementController?.SetMoveVector(newValue);
		}

		public void OnSwingInput(InputAction.CallbackContext context)
		{
			if (IsParalyzed)
				return;

			if (context.phase == InputActionPhase.Performed)
			{
				_movementController?.Swing();
			}
		}

		// Triggers
		private void OnTriggerEnter2D(Collider2D collision)
		{
			InteractableController interactableController = collision.GetComponent<InteractableController>();
			if (interactableController != null)
			{
				_activeInteractableControllers.Add(interactableController);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			InteractableController interactableController = collision.GetComponent<InteractableController>();
			if (interactableController != null)
			{
				_activeInteractableControllers.Remove(interactableController);
			}
		}

		// Public
		public void Paralyze(ParalyzeFlags flag, bool v)
		{
			if (v)
			{
				_paralyzeFlags = (_paralyzeFlags | flag);
			}
			else
			{
				_paralyzeFlags = (_paralyzeFlags & ~flag);
			}

			if (IsParalyzed)
			{
				_movementController.MovementDisabled = true;
				_movementController.SetMoveVector(Vector2.zero);
			}
			else
			{
				_movementController.MovementDisabled = false;
			}
		}

		public bool GetParalyzeValue(ParalyzeFlags flag)
		{
			return (_paralyzeFlags & flag) != 0;
		}

		public void SetPosition(LR facing, Vector2 pos, bool immediate)
		{
			_movementController.SetFacing(facing);
			_movementController.SetPosition(pos);
			if (immediate)
			{
				transform.position = pos;
			}
		}

		public void SpawnEffect(GameObject effect, float xOffset = 0, float yOffset = 0)
		{
			if (effect != null)
			{
				float facing = GameUtil.GetDirectionInt(_movementController.Facing);
				Vector3 spawnPos = transform.position + new Vector3(xOffset * facing, yOffset, 0.0f);
				GameObject newObj = Instantiate(effect, spawnPos, Quaternion.identity) as GameObject;
				newObj.transform.localScale = newObj.transform.localScale.x * new Vector3(facing, 1, 1);
			}
		}

		// Update methods
		private void Update()
		{
			// Debug visualization
			if (_enableDebugVisualizations)
			{
				if (!_movementController.DoubleJumpAvailable)
				{
					_spriteRenderer.color = Color.blue;
				}
				else if (_movementController.IsDropDownActive)
				{
					_spriteRenderer.color = Color.green;
				}
				else if (_movementController.InLedgeForgivness)
				{
					_spriteRenderer.color = Color.yellow;
				}
				else if (_movementController.OnGround)
				{
					_spriteRenderer.color = Color.red;
				}
				else
				{
					_spriteRenderer.color = Color.white;
				}
			}
		}

		private void FixedUpdate()
		{
		}

		// Private methods

		private bool HandleDashStateChange(Message message)
		{
			if (message is OnDashStateChangedMessage msg)
			{
				bool oldInFlight = msg.OldDashState == ActionState.InFlight;
				bool newInFlight = msg.NewDashState == ActionState.InFlight;

				_animatorPlayer.SetBool(_animIdIsDashing, _movementController.StateDash == ActionState.InFlight);

				if (!oldInFlight && newInFlight)
				{
					_animatorPlayer.SetTrigger(_animIdDashTrigger);
				}
				return true;
			}
			return false;
		}

		private bool HandleFacingChange(Message message)
		{
			if (message is OnFacingChangedMessage msg)
			{
				bool flip = _movementController.Facing == LR.Left;
				if (_spriteRenderer != null)
				{
					if (_spriteRenderer.flipX != flip)
					{
						_spriteRenderer.flipX = flip;
					}
				}
				return true;
			}
			return false;
		}

		private int GetJumpCount(MovementController.JumpState jumpState)
		{
			switch(jumpState)
			{
			case MovementController.JumpState.DoubleJumping:
			{
				return 2;
			}
			case MovementController.JumpState.Jumping:
			case MovementController.JumpState.WallJumping:
			case MovementController.JumpState.PogoJumping:
			{
				return 1;
			}
			case MovementController.JumpState.Falling:
			case MovementController.JumpState.Grounded:
			case MovementController.JumpState.WallSliding:
			default:
			{
				return 0;
			}
			}
		}

		private bool HandleJumpStateChange(Message message)
		{
			if (message is OnJumpStateChangedMessage msg)
			{
				int oldJumpCount = GetJumpCount(msg.OldJumpState);
				int newdJumpCount = GetJumpCount(msg.NewJumpState);

				_animatorPlayer.SetBool(_animIdOnGround, _movementController.OnGround);
				_animatorPlayer.SetBool(_animIdOnWall, _movementController.OnWall);
				_animatorPlayer.SetBool(_animIdIsRising, _movementController.InAir && _movementController.RigidBody.velocity.y > GameUtil.NearZero);

				if (newdJumpCount > oldJumpCount)
				{
					_animatorPlayer.SetTrigger(_animIdJumpTrigger);
				}

				if (msg.OldJumpState != MovementController.JumpState.WallSliding && msg.NewJumpState == MovementController.JumpState.WallSliding)
				{
					_animatorPlayer.SetTrigger(_animIdWallSlideTrigger);
				}

				return true;
			}
			return false;
		}

		private bool HandleSwingStateChange(Message message)
		{
			if (message is OnSwingStateChangedMessage msg)
			{
				bool oldInFlight = msg.OldSwingState == ActionState.InFlight;
				bool newInFlight = msg.NewSwingState == ActionState.InFlight;

				if (!oldInFlight && newInFlight)
				{
					LR facing = _movementController.Facing;
					_slashSprite.flipX = facing == _flipFacing;

					if (_movementController.MoveVector.y > _swingUpDownThreshold)
					{
						_animatorPlayer.SetTrigger(_animIdSwingUpTrigger);
						_animatorSlash.SetTrigger(_animIdSlashUpTrigger);
					}
					else if (_movementController.StateJump != MovementController.JumpState.Grounded && _movementController.MoveVector.y < -_swingUpDownThreshold)
					{
						_animatorPlayer.SetTrigger(_animIdSwingDownTrigger);
						_animatorSlash.SetTrigger(_animIdSlashDownTrigger);
					}
					else if (facing == LR.Left)
					{
						_animatorPlayer.SetTrigger(_animIdSwingLeftTrigger);
						_animatorSlash.SetTrigger(_animIdSlashLeftTrigger);
					}
					else
					{
						_animatorPlayer.SetTrigger(_animIdSwingRightTrigger);
						_animatorSlash.SetTrigger(_animIdSlashRightTrigger);
					}
				}

				return true;
			}
			return false;
		}

		private bool HandleMoveXChange(Message message)
		{
			if (message is MoveXMessage)
			{
				_animatorPlayer.SetBool(_animIdIsMoving, _movementController.IsMovingX);
				return true;
			}
			return false;
		}

		private bool HandleOnHit(Message message)
		{
			if (message is OnHitMessage msg)
			{
				if (_movementController != null)
				{
					HitSource source = msg.Source;
					HitTarget target = msg.Target;
					if (source != null && target != null)
					{
						if (source.Direction == Dir4.South && source.CanPogo && target.CanPogo)
						{
							_movementController.Pogo();
						}
						if ((source.Direction == Dir4.East || source.Direction == Dir4.West) &&
							!GameUtil.FuzzyEquals(target.PushBackDistance, Vector2.zero) && target.PushBackTime >= GameUtil.NearZero)
						{
							Vector2 distance = target.PushBackDistance;
							distance.x *= GameUtil.GetDirectionInt(_movementController.Facing);
							_movementController.Push(distance, target.PushBackTime);
						}
					}
				}
				return true;
			}
			return false;
		}

		private void OnGUI()
		{
			if (_enableDebugVisualizations)
			{
				string txt = "";
				txt += "VelocityX=" + _movementController.RigidBody.velocity.x + "\n";
				txt += "VelocityY=" + _movementController.RigidBody.velocity.y + "\n";
				txt += "Facing=" + _movementController.Facing + "\n";
				txt += "JumpState=" + _movementController.StateJump + "\n";
				txt += "SurfaceState=" + _movementController.StateSurface + "\n";
				txt += "DashState=" + _movementController.StateDash + "\n";
				txt += "SwingState=" + _movementController.StateSwing + "\n";
				txt += "Contacts\n";
				txt += "--------\n";
				for (int i = 0; i < _movementController.ContactPointCount; ++i)
				{
					ContactPoint2D contact = _movementController.ContactPoints[i];
					txt += "Normal[" + contact.normal + "] vel[" + contact.relativeVelocity + "] layer[" + contact.collider.gameObject.layer + "]\n";
				}

				GUILayout.Label(txt);
			}
		}
	}
}
