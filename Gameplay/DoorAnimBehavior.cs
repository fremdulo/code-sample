using UnityEngine;

namespace Platformer
{
	public class DoorAnimBehavior : BoolStateAnimBehavior
    {
        protected override BoolStateController GetBoolStateController(Animator animator)
        {
            return animator.GetComponent<DoorController>();
        }
    }
}