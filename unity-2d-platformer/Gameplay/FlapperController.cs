using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
	public class FlapperController : MonoBehaviour
	{
		[SerializeField]
		private string _leftTrigger = "LeftTrigger";

		[SerializeField]
		private string _rightTrigger = "RightTrigger";

		private Animator _animator;
		private int _leftTriggerId;
		private int _rightTriggerId;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_leftTriggerId = Animator.StringToHash(_leftTrigger);
			_rightTriggerId = Animator.StringToHash(_rightTrigger);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			LR dir = GameUtil.GetDirectionLR(collision.attachedRigidbody.velocity.x);
			if (dir == LR.Left)
			{
				_animator.SetTrigger(_leftTriggerId);
			}
			else if (dir == LR.Right)
			{
				_animator.SetTrigger(_rightTriggerId);
			}
		}
	}
}