using UnityEngine;

namespace Platformer
{
	public class BoolStateController : MonoBehaviour
	{
		[SerializeField]
		private string _animatorVariableName;

		[SerializeField]
		private bool _initialValue;

		[SerializeField]
		private bool _offEnable = true;

		[SerializeField]
		private bool _onEnable = true;

		private Animator _animator;

		private bool _requestedValue;
		public bool RequestedValue { get { return _requestedValue; } }

		private bool _actualValue;
		public bool ActualValue { get { return _actualValue; } }

		public bool IsAnimating { get { return _requestedValue != _actualValue; } }

		private void Start()
		{
			_animator = GetComponent<Animator>();
			Debug.Assert(_animator != null);
			Debug.Assert(!string.IsNullOrEmpty(_animatorVariableName));

			_requestedValue = _initialValue;
			_animator.SetBool(_animatorVariableName, _requestedValue);
		}

		public virtual void ForceValue(bool value)
		{
			if (_actualValue != value)
			{
				_actualValue = value;
			}
		}

		protected virtual void InternalSetRequestValue(bool value)
		{
			_requestedValue = value;
			_animator.SetBool(_animatorVariableName, _requestedValue);
		}

		public void RequestValue(bool value)
		{
			if (value && !_onEnable)
				return;

			if (!value && !_offEnable)
				return;

			if (_requestedValue != value)
			{
				InternalSetRequestValue(value);
			}
		}

		public void Toggle()
		{
			RequestValue(!_requestedValue);
		}

		public void TurnOff()
		{
			RequestValue(false);
		}

		public void TurnOn()
		{
			RequestValue(true);
		}
	}
}