using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
	public class ActionBoolStateController : BoolStateController
	{
		[SerializeField]
		private UnityEvent _onAction;

		[SerializeField]
		private UnityEvent _offAction;

		[SerializeField]
		private UnityEvent _onRequestAction;

		[SerializeField]
		private UnityEvent _offRequestAction;

		public void TriggerSwitch()
		{
			if (!IsAnimating)
			{
				Toggle();
			}
		}

		public override void ForceValue(bool value)
		{
			base.ForceValue(value);
			if (value)
			{
				_onAction?.Invoke();
			}
			else
			{
				_offAction?.Invoke();
			}
		}

		protected override void InternalSetRequestValue(bool value)
		{
			base.InternalSetRequestValue(value);
			if (value)
			{
				_onRequestAction?.Invoke();
			}
			else
			{
				_offRequestAction?.Invoke();
			}
		}
	}
}