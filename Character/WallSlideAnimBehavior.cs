using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Platformer
{
	public class WallSlideAnimBehavior : StateMachineBehaviour
	{
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PlayerAnimEvents animEvents = animator.gameObject.GetComponent<PlayerAnimEvents>();
			animEvents?.StartWallSlide();
		}

		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			PlayerAnimEvents animEvents = animator.gameObject.GetComponent<PlayerAnimEvents>();
			animEvents?.StopWallSlide();
		}
	}
}