using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class TransitionManager : MonoBehaviour
    {
		// Singleton
		private static TransitionManager _instance;
		public static TransitionManager Instance { get { return _instance; } }

		[SerializeField]
		private string _animTriggerDefaultIn = "trigger_dissolve_in";

		[SerializeField]
		private string _animTriggerDefaultOut = "trigger_dissolve_out";

		private int _animIdTriggerDefaultIn;
		private int _animIdTriggerDefaultOut;

		private FaderController _fader;
		public FaderController Fader { get { return _fader; } }

		private bool _inProgress;
		public bool InProgress { get { return _inProgress; } }

		private int _loadedBuildIndex = 0;
		public int LoadedBuildIndex { get { return _loadedBuildIndex; } }

		private int _savedCheckpoint;
		public int SavedCheckpoint { get { return _savedCheckpoint; } }

		// Init/Deinit

		private void Awake()
		{
			if (_instance != null)
			{
				Debug.LogError("Multiple instance of TransitionManager singleton detected!");
			}
			_instance = this;
		}

		private void OnEnable()
		{
			_fader = GetComponent<FaderController>();
			Debug.Assert(_fader != null, "TransitionManager - No FaderController.");

			_animIdTriggerDefaultIn = Animator.StringToHash(_animTriggerDefaultIn);
			_animIdTriggerDefaultOut = Animator.StringToHash(_animTriggerDefaultOut);
		}

		// Public

		public void ExecuteTransition(TransitionInfo transition, TransitionAsset asset)
		{
			GameController gc = GameController.Instance;

			if (transition.BuildIndex == 0)
			{
				transition.BuildIndex = _loadedBuildIndex;
			}

			if (_inProgress)
			{
				gc.DebugLog("TransitionManager ExectureTransition failed. Another is in-flight. " + transition.BuildIndex);
				return;
			}

			if (transition.BuildIndex <= 0)
			{
				transition.BuildIndex = 1;
			}
			gc.DebugLog("TransitionManager ExectureTransition sceneIndex=" + transition.BuildIndex);

			StartCoroutine(InternalExecuteTransition(transition, asset));
		}

		public void RestoreCheckpoint(TransitionAsset asset)
		{
			TransitionInfo transition = new TransitionInfo(0, _savedCheckpoint);
			ExecuteTransition(transition, asset);
		}

		public void SaveCheckpoint(CheckpointController checkpoint)
		{
			_savedCheckpoint = checkpoint?.Waypoint?.Id ?? 0;
		}

		// Private

		private IEnumerator InternalExecuteTransition(TransitionInfo transition, TransitionAsset asset)
		{
			GameController gc = GameController.Instance;

			_inProgress = true;

			int animTriggerInId = asset != null ? Animator.StringToHash(asset.AnimTriggerIn) : _animIdTriggerDefaultIn;
			int animTriggerOutId = asset != null ? Animator.StringToHash(asset.AnimTriggerOut) : _animIdTriggerDefaultOut;

			if (_loadedBuildIndex > 0)
			{
				PlayerController player = LevelController.Instance.Player;
				PlayerAnimEvents playerAnimEvents = player.AnimEvents;
				Animator playerAnimator = player.AnimatorPlayer;
				Debug.Assert(player != null, "TransitionManager - No player found.");
				Debug.Assert(playerAnimator != null, "TransitionManager - No player animator found.");

				// Paralyze
				player?.Paralyze(PlayerController.ParalyzeFlags.Transition, true);

				// Play AnimOut
				if (animTriggerOutId != 0)
				{
					playerAnimator.SetTrigger(animTriggerOutId);
					if (playerAnimEvents != null)
					{
						playerAnimEvents.IsInTransitionAnim = true;
						yield return new WaitUntil(() => { return !playerAnimEvents.IsInTransitionAnim; });
					}
				}

				// Fade Out
				_fader.RequestValue(true);
				while (_fader.IsAnimating)
				{
					yield return null;
				}
			}

			if (transition.BuildIndex != _loadedBuildIndex)
			{
				// Unload Old Scene
				AsyncOperation unloadOp = null;
				if (_loadedBuildIndex > 0)
				{
					gc.DebugLog("TransitionManager unloading buildIndex " + transition.BuildIndex);
					unloadOp = SceneManager.UnloadSceneAsync(_loadedBuildIndex);
				}

				// Load New Scene
				gc.DebugLog("TransitionManager loading buildIndex " + transition.BuildIndex);
				AsyncOperation loadOp = SceneManager.LoadSceneAsync(transition.BuildIndex, LoadSceneMode.Additive);
				while (!(loadOp?.isDone ?? true) || !(unloadOp?.isDone ?? true))
				{
					yield return null;
				}

				if (loadOp?.isDone ?? false)
				{
					_loadedBuildIndex = transition.BuildIndex;
				}
				else
				{
					gc.DebugLog("TransitionManager failed to load buildIndex " + transition.BuildIndex);
				}
			}

			if (_loadedBuildIndex > 0)
			{
				LevelController level = LevelController.Instance;
				PlayerController player = level.Player;
				PlayerAnimEvents playerAnimEvents = player.AnimEvents;
				Animator playerAnimator = player.AnimatorPlayer;
				SpriteRenderer playerSprite = player.SpriteRenderer;
				Debug.Assert(player != null, "TransitionManager - No player found.");
				Debug.Assert(playerAnimator != null, "TransitionManager - No player animator found.");
				Debug.Assert(playerSprite != null, "TransitionManager - No player sprite found.");

				// Paralyze
				player?.Paralyze(PlayerController.ParalyzeFlags.Transition, true);

				// Teleport to Waypoint
				WaypointController waypoint = level.FindWaypoint(transition.WaypointId);
				Debug.Assert(waypoint != null, string.Format("TransitionManager - Waypoint [{0}] not found.", transition.WaypointId));
				LR facing = waypoint != null ? waypoint.Facing : LR.Left;
				Vector2 position = waypoint != null ? (Vector2)waypoint.transform.position : Vector2.zero;
				player.Paralyze(PlayerController.ParalyzeFlags.Transition, true);
				player.SetPosition(facing, position, true);
				GameUtil.SnapCameraToPlayer(player);

				_savedCheckpoint = transition.WaypointId;

				// Fade In
				_fader.RequestValue(false);

				// Play AnimIn
				if (animTriggerInId != 0)
				{
					playerAnimator.SetTrigger(animTriggerInId);
					if (playerAnimEvents != null)
					{
						playerAnimEvents.IsInTransitionAnim = true;
						yield return new WaitUntil(() => { return !playerAnimEvents.IsInTransitionAnim; });
					}
				}

				// Un-paralyze
				player?.Paralyze(PlayerController.ParalyzeFlags.Transition, false);
			}

			_inProgress = false;
		}

	}
}

