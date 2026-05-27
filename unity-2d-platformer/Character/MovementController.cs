using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
	public class MovementController : MonoBehaviour
	{
		// Internal Types
		public enum JumpState
		{
			Falling,
			Grounded,
			WallSliding,
			Jumping,
			DoubleJumping,
			WallJumping,
			PogoJumping,
		}

		public enum SurfaceState
		{
			None,
			Solid,
			Platform
		}

		// ----- Inspector --------------------------------------------------------------------
		[Header("Abilities")]

		[SerializeField]
		private bool _enableDash;
		public bool EnableDash { get { return _enableDash; } set { _enableDash = value; } }

		[SerializeField]
		private bool _enableDoubleJump;
		public bool EnableDoubleJump { get { return _enableDoubleJump; } set { _enableDoubleJump = value; } }

		[SerializeField]
		private bool _enableDropDown;
		public bool EnableDropDown { get { return _enableDropDown; } set { _enableDropDown = value; } }

		[SerializeField]
		private bool _enableJump;
		public bool EnableJump { get { return _enableJump; } set { _enableJump = value; } }

		[SerializeField]
		private bool _enablePogoJump;
		public bool EnablePogoJump { get { return _enablePogoJump; } set { _enablePogoJump = value; } }

		[SerializeField]
		private bool _enableRun;
		public bool EnableRun { get { return _enableRun; } set { _enableRun = value; } }

		[SerializeField]
		private bool _enableSwing;
		public bool EnableSwing { get { return _enableSwing; } set { _enableSwing = value; } }

		[SerializeField]
		private bool _enableWallJump;
		public bool EnableWallJump { get { return _enableWallJump; } set { _enableWallJump = value; } }

		[SerializeField]
		private bool _enableWallSlide;
		public bool EnableWallSlide { get { return _enableWallSlide; } set { _enableWallSlide = value; } }

		//-------------------------------------------------------------------------------------
		[Header("Dash")]

		[SerializeField]
		[Min(0.1f)]
		private float _dashDistance = 5f; // Distance along x axis traveled in dashTime

		[SerializeField]
		[Min(0f)]
		private float _dashCooldown = 0.5f; // Time between dash finishing and the next available to start

		[SerializeField]
		[Min(0.1f)]
		private float _dashTime = 1f; // Time to complete a dash

		//-------------------------------------------------------------------------------------
		[Header("Drop Down")]

		[SerializeField]
		[Min(0f)]
		private float _dropDownDeadZone = 0.5f; // |_moveVector.y| must be larger than this value to register drop down

		[SerializeField]
		private SingleLayer _dropDownMoverLayer;

		[SerializeField]
		[Min(0f)]
		private float _dropDownResetDistance;

		//-------------------------------------------------------------------------------------
		[Header("Ground Check")]

		[SerializeField]
		private LayerMask _groundPlatformLayers; // Layers considered in the platform ground check

		[SerializeField]
		private LayerMask _groundSolidLayers; // Layers considered in the solid ground check

		[SerializeField]
		[Min(0f)]
		private float _ledgeForgivnessTime = 0.25f; // Time after falling off a ledge that the player can still jump (Coyote Time)

		//-------------------------------------------------------------------------------------
		[Header("Jump")]

		[SerializeField]
		[Min(0f)]
		private float _doubleJumpHeight = 3; // Height of double jump apex

		[SerializeField]
		[Min(0f)]
		private float _jumpHeight = 3; // Height of jump apex

		[SerializeField]
		[Min(0f)]
		private float _jumpHeightMin = 1; // Minimum jump height

		[SerializeField]
		[Min(0.1f)]
		private float _terminalVelocity = 20; // maximum fall speed units/sec

		//-------------------------------------------------------------------------------------
		[Header("Pogo Jump")]

		[SerializeField]
		[Min(0f)]
		private float _pogoHeight = 3; // Height of pogo jump apex

		[SerializeField]
		[Min(0f)]
		private float _pogoHeightMin = 3; // Minimum pogo jump height

		//-------------------------------------------------------------------------------------
		[Header("Sensors")]

		[SerializeField]
		private SimpleSensor[] _leftWallSensors;

		[SerializeField]
		private SimpleSensor[] _rightWallSensors;

		//-------------------------------------------------------------------------------------
		[Header("Swing")]

		[SerializeField]
		[Min(0f)]
		private float _swingCooldown = 0.25f; // Swing colldown time after swing finishes

		[SerializeField]
		[Min(0.1f)]
		private float _swingTime = 0.15f; // Time to complete a swing

		//-------------------------------------------------------------------------------------
		[Header("Run")]

		[SerializeField]
		[Min(0f)]
		private float _baseDistance = 3;

		[SerializeField]
		[Min(0.1f)]
		private float _baseRunTime = 1;

		[SerializeField]
		[Min(0f)]
		private float _moveDeadZone = 0.5f; // |_moveVector.x| must be larger than this value to register horz movement

		//-------------------------------------------------------------------------------------
		[Header("Wall Jump/Slide")]

		[SerializeField]
		[Min(0.1f)]
		private float _wallJumpTime = 1f;

		[SerializeField]
		[Min(0f)]
		private float _wallSlideVelocity = 5; // maximum fall speed units/sec while wall sliding

		// ----- End Inspector ----------------------------------------------------------------

		// Components
		private Rigidbody2D _rigidBody;
		public Rigidbody2D RigidBody { get { return _rigidBody; } }

		// Inputs
		private bool _dashRequested;
		private bool _jumpRequested;
		private bool _jumpStopRequested;
		private Vector2 _moveVector;
		private bool _pogoRequested;
		private bool _swingRequested;
		public bool IsMovingX { get { return GameUtil.GetDirectionInt(_moveVector.x, _moveDeadZone) != 0; } }
		public Vector2 MoveVector { get { return _moveVector; } }

		// Action States
		private ActionState _stateDash = ActionState.Ready;
		public ActionState StateDash { get { return _stateDash; } }

		private ActionState _stateSwing = ActionState.Ready;
		public ActionState StateSwing { get { return _stateSwing; } }

		// States
		private LR _facing = LR.Right;
		public LR Facing { get { return _facing; } }

		private bool _isMoving;
		public bool IsMoving { get { return _isMoving; } }

		private JumpState _stateJump = JumpState.Falling;
		public JumpState StateJump { get { return _stateJump; } }

		private SurfaceState _stateSurface = SurfaceState.None;
		public SurfaceState StateSurface { get { return _stateSurface; } }

		// ----- Actions --------------------------------------------------------------------
		// Dash
		private bool _dashAvailable = true;
		private float _dashTimer;
		private float _dashVelocity;

		// Swing
		private float _swingTimer;
		// ----------------------------------------------------------------------------------

		// DropDown
		private SingleLayer _defaultMoverLayer;
		private float _dropDownTarget = float.NaN;
		public bool IsDropDownActive { get { return !float.IsNaN(_dropDownTarget); } }

		// Ground check
		private ContactPoint2D[] _contactPoints = new ContactPoint2D[16];
		public ContactPoint2D[] ContactPoints { get { return _contactPoints; } }
		private int _contactPointCount;
		public int ContactPointCount { get { return _contactPointCount; } }
		private ContactFilter2D _groundContactFilter;
		private float _ledgeTimer;

		public bool OnGround { get { return _stateJump == JumpState.Grounded; } }
		public bool OnWall { get { return _stateJump == JumpState.WallSliding; } }
		public bool InAir { get { return !OnGround && !OnWall; } }
		public bool InLedgeForgivness { get { return _ledgeTimer > GameUtil.NearZero; } }

		// Jump - Double
		private bool _doubleJumpAvailable = false;
		public bool DoubleJumpAvailable { get { return _doubleJumpAvailable; } }
		private float _doubleJumpImpulse;
		private float _gravityDouble;

		// Jump - Pogo
		private float _gravityPogo;
		private float _pogoImpulse;

		// Jump - Standard
		private float _activeJumpHeightMin;
		private float _gravityFall;
		private float _gravityJump;
		private float _jumpImpulse;
		private float _jumpStartY;

		// Jump - Wall
		private float _wallJumpTimer;
		public bool IsWallJumping { get { return _wallJumpTimer > 0f; } }

		// Move
		private float _runVelocity;

		// Push
		private List<PushInfo> _pushes = new List<PushInfo>();

		// Control
		private bool _movementDisabled;
		public bool MovementDisabled { get { return _movementDisabled; } set { _movementDisabled = value; } }

		// Init/Deinit methods
		private void OnEnable()
		{
			Debug.Assert(GameController.Instance != null, "GameController instance found.", this);

			_rigidBody = GetComponent<Rigidbody2D>();
			Debug.Assert(_rigidBody != null, "No RigidBody2D component found.", this);

			_dashVelocity = _dashDistance / _dashTime;

			_defaultMoverLayer = new SingleLayer();
			_defaultMoverLayer.Set(gameObject.layer);

			_groundContactFilter.useTriggers = false;
			_groundContactFilter.SetLayerMask(new LayerMask() { value = _groundSolidLayers.value | _groundPlatformLayers.value });
			_groundContactFilter.useLayerMask = true;

			_gravityFall = (-2 * _jumpHeight) / (_baseRunTime * _baseRunTime);
			_gravityJump = (-2 * _jumpHeight) / (_baseRunTime * _baseRunTime);
			_gravityDouble = (-2 * _doubleJumpHeight) / (_baseRunTime * _baseRunTime);
			_gravityPogo = (-2 * _pogoHeight) / (_baseRunTime * _baseRunTime);

			_jumpImpulse = -_gravityJump * _baseRunTime;
			_doubleJumpImpulse = -_gravityDouble * _baseRunTime;
			_pogoImpulse = -_gravityPogo * _baseRunTime;

			_runVelocity = _baseDistance / _baseRunTime;
		}

		// Public methods
		public void Dash()
		{
			_dashRequested = true;
		}

		public void Jump()
		{
			if (_enableDropDown && _stateSurface == SurfaceState.Platform && _moveVector.y < -_dropDownDeadZone)
			{
				gameObject.layer = _dropDownMoverLayer.LayerIndex;
				_dropDownTarget = _rigidBody.position.y - _dropDownResetDistance;
			}
			else
			{
				_jumpRequested = true;
			}
		}

		public void Pogo()
		{
			_pogoRequested = true;
		}

		public void Push(Vector2 impulse, float dur)
		{
			_pushes.Add(new PushInfo(impulse, dur));
		}

		public void SetFacing(LR facing)
		{
			LR newFacing = facing != LR.None ? facing : LR.Right;

			if (_facing != newFacing)
			{
				LR oldFacing = _facing;
				_facing = newFacing;
				GameController.Instance.MessageManager.SendMessageImmediate(new OnFacingChangedMessage(this, _facing, oldFacing));
			}
		}

		public void SetMoveVector(Vector2 v)
		{
			if (!GameUtil.FuzzyEquals(_moveVector, v))
			{
				_moveVector = v;
			}
		}

		public void SetPosition(Vector2 pos)
		{
			_rigidBody.position = pos;
			_rigidBody.velocity = Vector2.zero;

			_dashRequested = false;
			_jumpRequested = false;
			_pogoRequested = false;
			_swingRequested = false;

			JumpState oldJumpState = _stateJump;
			_stateJump = JumpState.Falling;
			GameController.Instance.MessageManager.SendMessageImmediate(new OnJumpStateChangedMessage(this, _stateJump, oldJumpState));

			StopDash();
		}

		public void Swing()
		{
			_swingRequested = true;
		}

		public void StopDash()
		{
			if (_stateDash == ActionState.InFlight)
			{
				_dashTimer = 0f;
			}
		}

		public void StopJump()
		{
			_jumpStopRequested = true;
		}

		// Update methods
		private void FixedUpdate()
		{
			if (_movementDisabled)
			{
				// Temp - should pause whole game here
				_rigidBody.velocity = Vector2.zero;
				return;
			}

			Vector2 v = _rigidBody.velocity;

			// Hack - Stationary RigidBody sometimes has a +v.y greater that GameUtil.NearZero, no idea why.
			//        Need a larger threshhold
			if (GameUtil.FuzzyEquals(v.x, 0f, 0.001f))
				v.x = 0f;
			if (GameUtil.FuzzyEquals(v.y, 0f, 0.001f))
				v.y = 0f;

			SurfaceState oldSurfaceState = _stateSurface;
			JumpState oldJumpState = _stateJump;
			LR oldFacing = _facing;
			bool oldIsMoving = _isMoving;

			int moveX = GameUtil.GetDirectionInt(_moveVector.x, _moveDeadZone);
			LR moveDir = GameUtil.GetDirectionLR(_moveVector.x, _moveDeadZone);
			_isMoving = moveDir != LR.None;

			// Handle DropDown
			if (!float.IsNaN(_dropDownTarget))
			{
				if (_rigidBody.position.y < _dropDownTarget || _rigidBody.velocity.y > GameUtil.NearZero)
				{
					_dropDownTarget = float.NaN;
					gameObject.layer = _defaultMoverLayer.LayerIndex;
				}
			}

			// Handle actions
			bool isDashing = UpdateDash(Time.fixedDeltaTime, ref v);
			bool isSwinging = UpdateSwing(Time.fixedDeltaTime);

			if (!isDashing)
			{
				bool oldInAir = InAir;

				SurfaceState surface;
				Dir4 side;
				GetCollisionSurface(out surface, out side);

				// Update ground/wallslide state
				if (side == Dir4.South && v.y <= GameUtil.NearZero)
				{
					_stateJump = JumpState.Grounded;
					_stateSurface = surface;
				}
				else if (_enableWallSlide && !IsWallJumping && !isSwinging && surface == SurfaceState.Solid && (side == Dir4.East || side == Dir4.West) &&
					_stateJump != JumpState.Grounded && v.y < -GameUtil.NearZero && CheckWallSensors())
				{
					if (_stateJump == JumpState.WallSliding || _isMoving)
					{
						_stateJump = JumpState.WallSliding;
						_stateSurface = surface;

						// Clear pushes to prevent mover from "flickering" on and off a wall slide
						_pushes.Clear();
					}
				}
				else
				{
					_stateSurface = SurfaceState.None;

					if (!InAir)
					{
						// Dropping off ledge/wall
						_stateJump = JumpState.Falling;
					}
				}

				// Start Ledge Forgiveness time
				if (!oldInAir && _stateJump == JumpState.Falling)
				{
					_ledgeTimer = _ledgeForgivnessTime;
				}

				// Jump state resets
				if (InAir)
				{
					if (_wallJumpTimer > 0f)
					{
						_wallJumpTimer -= Time.fixedDeltaTime;
						if (_wallJumpTimer < 0f)
						{
							_wallJumpTimer = 0f;
						}
					}
				}
				else
				{
					ResetJump();
				}

				// Update ledge timer
				_ledgeTimer -= Time.fixedDeltaTime;
				if (_ledgeTimer < 0)
				{
					_ledgeTimer = 0;
				}

				// Update facing
				if (_stateJump == JumpState.WallSliding)
				{
					_facing = side == Dir4.East ? LR.Left : LR.Right;
				}
				else if (!isSwinging)
				{
					LR newFacing = GameUtil.GetDirectionLR(_moveVector.x, _moveDeadZone);
					if (newFacing != LR.None)
					{
						_facing = newFacing;
					}
				}

				// Update run movement
				if (_enableRun)
				{
					if (!IsWallJumping && (!OnWall || moveDir == _facing))
					{
						v.x = moveX * _runVelocity;
					}
				}
				// Apply gravity
				if (InAir && v.y < -GameUtil.NearZero)
				{
					_stateJump = JumpState.Falling;
				}
				if (!OnGround)
				{
					if (_stateJump == JumpState.Jumping)
					{
						v.y += _gravityJump * Time.fixedDeltaTime;
					}
					else if (_stateJump == JumpState.WallJumping)
					{
						v.y += _gravityJump * Time.fixedDeltaTime;
					}
					else if (_stateJump == JumpState.DoubleJumping)
					{
						v.y += _gravityDouble * Time.fixedDeltaTime;
					}
					else if (_stateJump == JumpState.PogoJumping)
					{
						v.y += _gravityPogo * Time.fixedDeltaTime;
					}
					else
					{
						v.y += _gravityFall * Time.fixedDeltaTime;
					}
				}

				// Handle Pushes Y
				foreach (PushInfo push in _pushes)
				{
					if (!push.ImpulseApplied)
					{
						push.ImpulseApplied = true;
						v.y += push.Impulse.y;
					}
				}

				// Handle Pogo
				if (_pogoRequested)
				{
					_pogoRequested = false;
					if (_enablePogoJump)
					{
						ResetJump();
						v.y = _pogoImpulse;
						_activeJumpHeightMin = _pogoHeightMin;
						_jumpStartY = _rigidBody.position.y;
						_stateJump = JumpState.PogoJumping;
					}
				}

				// Handle jump
				if (_jumpRequested)
				{
					_jumpRequested = false;
					if (_enableJump && (OnGround || _ledgeTimer > GameUtil.NearZero))
					{
						v.y = _jumpImpulse;
						_activeJumpHeightMin = _jumpHeightMin;
						_jumpStartY = _rigidBody.position.y;
						_stateJump = JumpState.Jumping;
					}
					else if (_enableJump && OnWall)
					{
						ResetJump();

						v.y = _jumpImpulse;
						v.x = GameUtil.GetDirectionInt(_facing) * _runVelocity;

						_activeJumpHeightMin = _jumpHeightMin;
						_jumpStartY = _rigidBody.position.y;
						_stateJump = JumpState.WallJumping;
						_wallJumpTimer = _wallJumpTime;
					}
					else if (_enableDoubleJump && InAir && _doubleJumpAvailable)
					{
						_doubleJumpAvailable = false;
						v.y = _doubleJumpImpulse;
						_activeJumpHeightMin = _jumpHeightMin;
						_jumpStartY = _rigidBody.position.y;
						_stateJump = JumpState.DoubleJumping;
					}
				}

				// Handle stop jump
				if (_jumpStopRequested)
				{
					if (!InAir)
					{
						_jumpStopRequested = false;
					}
					else if (v.y > GameUtil.NearZero)
					{
						if (_rigidBody.position.y >= _jumpStartY + _activeJumpHeightMin)
						{
							v.y = 0f;
							_stateJump = JumpState.Falling;
							_jumpStopRequested = false;
						}
					}
					else
					{
						_jumpStopRequested = false;
					}
				}

				// Correct for terminal velocity
				if (OnWall)
				{
					if (v.y < -_wallSlideVelocity)
					{
						v.y = -_wallSlideVelocity;
					}
				}
				else
				{
					if (v.y < -_terminalVelocity)
					{
						v.y = -_terminalVelocity;
					}
				}
			}

			// Handle Pushes X
			UpdatePush(Time.fixedDeltaTime);
			foreach (PushInfo push in _pushes)
			{
				v.x += push.Impulse.x;
			}

			_rigidBody.velocity = v;

			// Send messages
			if (_facing != oldFacing)
			{
				GameController.Instance.MessageManager.SendMessageImmediate(new OnFacingChangedMessage(this, _facing, oldFacing));
			}

			if (_stateJump != oldJumpState)
			{
				GameController.Instance.MessageManager.SendMessageImmediate(new OnJumpStateChangedMessage(this, _stateJump, oldJumpState));
			}

			if (_stateSurface != oldSurfaceState)
			{
				GameController.Instance.MessageManager.SendMessageImmediate(new OnSurfaceStateChangedMessage(this, _stateSurface, oldSurfaceState));
			}

			if (_isMoving != oldIsMoving)
			{
				GameController.Instance.MessageManager.SendMessageImmediate(new MoveXMessage(this));
			}
		}

		// Private methods
		private bool CheckWallSensors()
		{
			bool result = true;
			foreach (SimpleSensor sensor in _leftWallSensors)
			{
				if (sensor == null)
					continue;

				if (!sensor.State())
				{
					result = false;
					break;
				}
			}
			if (!result)
			{
				result = true;
				foreach (SimpleSensor sensor in _rightWallSensors)
				{
					if (sensor == null)
						continue;

					if (!sensor.State())
					{
						result = false;
						break;
					}
				}
			}
			return result;
		}

		private bool GetCollisionSurface(out SurfaceState outSurface, out Dir4 outSide)
		{
			outSurface = SurfaceState.None;
			outSide = Dir4.None;

			_contactPointCount = _rigidBody.GetContacts(_groundContactFilter, _contactPoints);
			for (int i = 0; i < _contactPointCount; ++i)
			{
				ContactPoint2D contact = _contactPoints[i];

				// Skip disabled contacts
				if (!contact.enabled)
					continue;

				Dir4 workingSide = GetCollidedSurfaceDirection(contact.normal);

				if (workingSide == Dir4.None)
					continue;

				// We don't care about ceiling collisions
				if (workingSide == Dir4.North)
					continue;

				// Ground collisions take priority over wall collisions
				if ((workingSide == Dir4.East || workingSide == Dir4.West) && outSide == Dir4.South)
					continue;

				SurfaceState workingSurface = SurfaceState.None;
				int workingLayer = 1 << contact.collider.gameObject.layer;

				if ((workingLayer & _groundSolidLayers.value) != 0)
				{
					workingSurface = SurfaceState.Solid;
				}
				else if ((workingLayer & _groundPlatformLayers.value) != 0)
				{
					workingSurface = SurfaceState.Platform;
				}

				// Ignore surfaces non-solid and non-platform surfaces
				if (workingSurface == SurfaceState.None)
					continue;

				// Solid ground surfaces take priority over Platform surfaces
				if (outSide == Dir4.South && outSurface == SurfaceState.Solid && workingSurface == SurfaceState.Platform)
					continue;

				outSurface = workingSurface;
				outSide = workingSide;
			}

			return outSurface != SurfaceState.None;
		}

		Dir4 GetCollidedSurfaceDirection(Vector2 hitNormal)
		{
			if (GameUtil.FuzzyEquals(hitNormal.y, 1f))
			{
				return Dir4.South;
			}
			else if (GameUtil.FuzzyEquals(hitNormal.y, -1f))
			{
				return Dir4.North;
			}
			else if (GameUtil.FuzzyEquals(hitNormal.x, 1f))
			{
				return Dir4.West;
			}
			else if (GameUtil.FuzzyEquals(hitNormal.x, -1f))
			{
				return Dir4.East;
			}
			else
			{
				return Dir4.None;
			}
		}

		private void ResetJump()
		{
			_doubleJumpAvailable = true;
			_dashAvailable = true;
			_wallJumpTimer = 0f;
			_activeJumpHeightMin = 0;
		}

		bool UpdateDash(float deltaTime, ref Vector2 v)
		{
			// Handle Dash
			ActionState oldDashState = _stateDash;
			if (_enableDash && _dashRequested && _dashAvailable && _stateDash == ActionState.Ready)
			{
				_dashAvailable = false;
				_stateDash = ActionState.InFlight;
				_stateJump = JumpState.Falling;
				_dashTimer = _dashTime;
			}
			_dashRequested = false;

			if (_stateDash == ActionState.InFlight)
			{
				v.x = _facing == LR.Left ? -_dashVelocity : _dashVelocity;
				v.y = 0f;
				_dashTimer -= deltaTime;

				if (_dashTimer <= 0f)
				{
					_stateDash = ActionState.Cooldown;
					_dashTimer = _dashCooldown;
				}
			}

			if (_stateDash == ActionState.Cooldown)
			{
				_dashTimer -= deltaTime;
				if (_dashTimer <= 0f)
				{
					_stateDash = ActionState.Ready;
					_dashTimer = 0f;
				}
			}

			if (_stateDash != oldDashState)
			{
				GameController.Instance.MessageManager.SendMessageImmediate(new OnDashStateChangedMessage(this, _stateDash, oldDashState));
			}

			return _stateDash == ActionState.InFlight;
		}

		private void UpdatePush(float deltaTime)
		{
			foreach (PushInfo push in _pushes)
			{
				push.Update(deltaTime);
			}
			_pushes.RemoveAll((push) => !push.IsActive);
		}

		bool UpdateSwing(float deltaTime)
		{
			// Handle Swing
			ActionState oldSwingState = _stateSwing;
			if (_enableSwing && _swingRequested && _stateSwing == ActionState.Ready && _stateDash != ActionState.InFlight)
			{
				_stateSwing = ActionState.InFlight;
				_swingTimer = _swingTime;
			}
			_swingRequested = false;

			if (_stateSwing == ActionState.InFlight)
			{
				_swingTimer -= deltaTime;

				if (_swingTimer <= 0f)
				{
					_stateSwing = ActionState.Cooldown;
					_swingTimer = _swingCooldown;
				}
			}

			if (_stateSwing == ActionState.Cooldown)
			{
				_swingTimer -= deltaTime;
				if (_swingTimer <= 0f)
				{
					_stateSwing = ActionState.Ready;
					_swingTimer = 0f;
				}
			}

			if (_stateSwing != oldSwingState)
			{
				GameController.Instance.MessageManager.SendMessageImmediate(new OnSwingStateChangedMessage(this, _stateSwing, oldSwingState));
			}

			return _stateSwing == ActionState.InFlight;
		}
	}

}
