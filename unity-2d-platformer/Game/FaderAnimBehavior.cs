using UnityEngine;

namespace Platformer
{
	public class FaderAnimBehavior : BoolStateAnimBehavior
	{
		protected override BoolStateController GetBoolStateController(Animator animator)
		{
			return animator.GetComponent<FaderController>();
		}
	}
}