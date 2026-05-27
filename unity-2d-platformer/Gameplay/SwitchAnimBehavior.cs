using UnityEngine;

namespace Platformer
{
	public class SwitchAnimBehavior : BoolStateAnimBehavior
	{
		protected override BoolStateController GetBoolStateController(Animator animator)
		{
			return animator.GetComponent<SwitchController>();
		}
	}
}