namespace Platformer
{
	public class OnDashStateChangedMessage : Message
	{
		public ActionState OldDashState { get; }
		public ActionState NewDashState { get; }

		public OnDashStateChangedMessage(MovementController sender, ActionState newDashState, ActionState oldDashState)
			: base(sender)
		{
			NewDashState = newDashState;
			OldDashState = oldDashState;
		}
	}

	public class OnFacingChangedMessage : Message
	{
		public LR NewValue;
		public LR OldValue;

		public OnFacingChangedMessage(MovementController sender, LR newValue, LR oldValue)
			: base(sender)
		{
			NewValue = newValue;
			OldValue = oldValue;
		}
	}

	public class OnJumpStateChangedMessage : Message
	{
		public MovementController.JumpState OldJumpState { get; }
		public MovementController.JumpState NewJumpState { get; }

		public OnJumpStateChangedMessage(MovementController sender, MovementController.JumpState newJumpState, MovementController.JumpState oldJumpState)
			: base(sender)
		{
			NewJumpState = newJumpState;
			OldJumpState = oldJumpState;
		}
	}

	public class OnSwingStateChangedMessage : Message
	{
		public ActionState OldSwingState { get; }
		public ActionState NewSwingState { get; }

		public OnSwingStateChangedMessage(MovementController sender, ActionState newSwingState, ActionState oldSwingState)
			: base(sender)
		{
			NewSwingState = newSwingState;
			OldSwingState = oldSwingState;
		}
	}

	public class OnSurfaceStateChangedMessage : Message
	{
		public MovementController.SurfaceState OldSurfaceState { get; }
		public MovementController.SurfaceState NewSurfaceState { get; }

		public OnSurfaceStateChangedMessage(MovementController sender, MovementController.SurfaceState newSurfaceState, MovementController.SurfaceState oldSurfaceState)
			: base(sender)
		{
			NewSurfaceState = newSurfaceState;
			OldSurfaceState = oldSurfaceState;
		}
	}

	public class MoveXMessage : Message
	{
		public MoveXMessage(MovementController sender)
			: base(sender)
		{
		}
	}
}
