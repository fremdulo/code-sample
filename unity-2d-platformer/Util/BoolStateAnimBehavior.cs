using UnityEngine;

namespace Platformer
{
	public class BoolStateAnimBehavior : StateMachineBehaviour
	{
		[SerializeField]
		private bool _setValue;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			BoolStateController boolState = GetBoolStateController(animator);
			Debug.Assert(boolState != null);
			if (boolState == null)
				return;

			boolState.ForceValue(_setValue);
		}

		protected virtual BoolStateController GetBoolStateController(Animator animator)
		{
			return animator.GetComponent<BoolStateController>();
		}
	}
}