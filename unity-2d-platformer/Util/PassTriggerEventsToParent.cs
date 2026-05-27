using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
	public interface ITriggerEventListener
	{
		void OnTriggerEnter2D(Collider2D collision);
		void OnTriggerExit2D(Collider2D collision);
	}

	public class PassTriggerEventsToParent : MonoBehaviour
	{
		private List<ITriggerEventListener> _listeners = new List<ITriggerEventListener>();

		private void OnEnable()
		{
			_listeners.Clear();
			transform.parent.GetComponents(_listeners);
			Debug.Assert(_listeners.Count > 0);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			foreach (ITriggerEventListener listener in _listeners)
			{
				listener.OnTriggerEnter2D(collision);
			}
		}

		private void OnTriggerExit2D(Collider2D collision)
		{
			foreach (ITriggerEventListener listener in _listeners)
			{
				listener.OnTriggerExit2D(collision);
			}
		}
	}
}