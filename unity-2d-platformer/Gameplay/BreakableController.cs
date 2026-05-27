using UnityEngine;
using UnityEngine.Events;

namespace Platformer
{
	public class BreakableController : MonoBehaviour
	{
		[SerializeField]
		private string _animHitTrigger;

		[SerializeField]
		private string _animBreakTrigger;

		[SerializeField]
		private UnityEvent _breakAction;

		[SerializeField]
		private Dir4 _hitDir;

		[SerializeField]
		[Min(1)]
		private uint _hits = 1;

		private Animator _animator;
		private int _animHitTriggerId;
		private int _animBreakTriggerId;

		private uint _hitsRemaining;

		private void Start()
		{
			_animator = GetComponent<Animator>();
			_animHitTriggerId = Animator.StringToHash(_animHitTrigger);
			_animBreakTriggerId = Animator.StringToHash(_animBreakTrigger);
			_hitsRemaining = _hits;
		}

		public void Hit(GameObject hitter)
		{
			HitSource hitSource = hitter.GetComponent<HitSource>();
			if (hitSource == null)
				return;

			if (!CheckDir(hitSource.Direction))
				return;

			if (_hitsRemaining > 0)
			{
				--_hitsRemaining;
			}

			if (_animator != null)
			{
				if (_hitsRemaining > 0)
				{
					if (_animHitTriggerId != 0)
					{
						_animator.SetTrigger(_animHitTriggerId);
					}
				}
				else
				{
					if (_animBreakTriggerId != 0)
					{
						_animator.SetTrigger(_animBreakTriggerId);
					}
					_breakAction?.Invoke();
				}
			}
		}

		private bool CheckDir(Dir4 sourceDir)
		{
			return _hitDir == sourceDir;
		}
	}
}
