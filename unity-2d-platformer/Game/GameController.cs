using UnityEngine;

namespace Platformer
{
	public class GameController : MonoBehaviour
	{
		// Singleton
		private static GameController _instance;
		public static GameController Instance { get { return _instance; } }

		// Inspector
		[Header("Debug")]
		[SerializeField]
		private bool _verboseDebugLogging = false;

		[Header("Transitions")]
		[SerializeField]
		private string _devScenePlayerPrefKey = "DevScene";
		public string DevScenePlayerPrefKey { get { return _devScenePlayerPrefKey; } }

		[SerializeField]
		private string _startWaypointName;

		// Systems
		private MessageManager _messageManager;
		public MessageManager MessageManager { get { return _messageManager; } }

		private void Awake()
		{
			if (_instance != null)
			{
				Debug.LogError("Multiple instance of GameController singleton detected!");
			}
			_instance = this;

			_messageManager = new MessageManager();

			// TODO-Temp
			AudioListener.volume = 0.3f;
		}

		private void Start()
		{
			Debug.Assert(TransitionManager.Instance != null, "GameController - No TransitionController found.");

			int startBuildIndex = PlayerPrefs.GetInt(_devScenePlayerPrefKey);
			int startWaypointId = LevelController.GetWaypointId(_startWaypointName);
			TransitionInfo startTransition = new TransitionInfo(startBuildIndex, startWaypointId);

			TransitionManager.Instance.ExecuteTransition(startTransition, null);
		}

		public void DebugLog(string s)
		{
			if (_verboseDebugLogging)
			{
				Debug.Log(s);
			}
		}

		private void Update()
		{
			_messageManager.Update();
		}
	}
}