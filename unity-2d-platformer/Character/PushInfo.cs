using UnityEngine;

namespace Platformer
{
	public class PushInfo
	{
		private Vector2 _impulse;
		public Vector2 Impulse { get { return _impulse; } }

		private bool _impulseApplied;
		public bool ImpulseApplied { get { return _impulseApplied; } set { _impulseApplied = value; } }

		public bool IsActive { get { return _timer > 0f; } }

		private float _timer;

		public PushInfo(Vector2 distance, float dur)
		{
			Debug.Assert(!GameUtil.FuzzyEquals(dur, 0f));

			_impulse = distance / dur;
			_timer = dur;
		}

		public void Update(float deltaTime)
		{
			_timer -= deltaTime;
			if (_timer < 0)
			{
				_timer = 0;
			}
		}
	}
}