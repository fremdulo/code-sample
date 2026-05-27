using System;
using System.Collections.Generic;

namespace Platformer
{
	public abstract class Message
	{
		protected readonly object _sender;
		public object Sender { get { return _sender; } }

		protected Message(object sender)
		{
			_sender = sender;
		}
	}

	public interface IMessageListener
	{
		public void OnMessage(Message message);
	}

	public class MessageManager
	{
		private Dictionary<Type, List<IMessageListener>> _listenerMap;

		private List<Message> _messageQueue;

		public MessageManager()
		{
			_listenerMap = new Dictionary<Type, List<IMessageListener>>();
			_messageQueue = new List<Message>();
		}

		public void RegisterListener(Type t, IMessageListener listener)
		{
			List<IMessageListener> listenerList;
			if (!_listenerMap.TryGetValue(t, out listenerList))
			{
				listenerList = new List<IMessageListener>();
				_listenerMap.Add(t, listenerList);
			}
			listenerList.Add(listener);
		}

		public void UnregisterListener(Type t, IMessageListener listener)
		{
			if (_listenerMap.TryGetValue(t, out List<IMessageListener> listenerList))
			{
				listenerList.Remove(listener);
				if (listenerList.Count == 0)
				{
					_listenerMap.Remove(t);
				}
			}
		}

		public void SendMessage(Message message)
		{
			_messageQueue.Add(message);
		}

		public void SendMessageImmediate(Message message)
		{
			Type t = message.GetType();
			if (_listenerMap.TryGetValue(t, out List<IMessageListener> listenerList))
			{
				foreach (IMessageListener listener in listenerList)
				{
					listener.OnMessage(message);
				}
			}
		}

		public void Update()
		{
			for (int i = 0; i < _messageQueue.Count; ++i)
			{
				SendMessageImmediate(_messageQueue[i]);
			}
			_messageQueue.Clear();
		}
	}
}