using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
	public class FallThroughController : MonoBehaviour
	{
		[SerializeField]
		private string _animBreakTrigger;

		[SerializeField]
		private string _animFixTrigger;

		[SerializeField]
		private UnityEvent _breakAction;

		private Animator _animator;
		private int _animBreakTriggerId;
		private int _animFixTriggerId;

		private void Start()
		{
			_animator = GetComponent<Animator>();
			_animBreakTriggerId = Animator.StringToHash(_animBreakTrigger);
			_animFixTriggerId = Animator.StringToHash(_animFixTrigger);
		}

		public void Break()
		{
			if (_animator != null)
			{
				_animator.SetTrigger(_animBreakTriggerId);
			}
		}

		public void Fix()
		{
			if (_animator != null)
			{
				_animator.SetTrigger(_animFixTriggerId);
			}
		}

		public void InvokeBreakAction()
		{
			_breakAction?.Invoke();
		}
	}
}