using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
	public class CameraZoneUser : MonoBehaviour
	{
		[Header("Debug")]
		[SerializeField]
		private bool _verboseDebugLogging = false;

		[Header("Camera")]
		[SerializeField]
		private LayerMask _zoneLayerMask;

		[SerializeField]
		private string _defaultCameraName;

		private GameObject _active;
		private CinemachineVirtualCamera _defaultCamera;
		private List<GameObject> _stack = new List<GameObject>();

		private void AddToStack(GameObject go)
		{
			if (go != null)
			{
				if (_stack.Contains(go))
				{
					_stack.Remove(go);
				}
				_stack.Insert(0, go);
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (((1 << other.gameObject.layer) & _zoneLayerMask.value) != 0)
			{
				CameraZone zone = other.GetComponent<CameraZone>();
				GameObject newCameraObj = zone?.ZoneCamera?.gameObject;
				if (newCameraObj != null)
				{
					AddToStack(newCameraObj);
					UpdateCamera();
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (((1 << other.gameObject.layer) & _zoneLayerMask.value) != 0) 
			{
				CameraZone zone = other.GetComponent<CameraZone>();
				GameObject newCameraObj = zone?.ZoneCamera?.gameObject;
				if (newCameraObj != null)
				{
					RemoveFromStack(newCameraObj);
					UpdateCamera();
				}
			}
		}

		private void RemoveFromStack(GameObject go)
		{
			if (go != null)
			{
				if (_stack.Contains(go))
				{
					_stack.Remove(go);
				}
			}
		}

		private void Start()
		{
			GameObject obj = GameObject.Find(_defaultCameraName);
			_defaultCamera = obj?.GetComponent<CinemachineVirtualCamera>();
			_active = _defaultCamera.gameObject;
		}


		private void UpdateCamera()
		{
			GameObject newCameraObj = _stack.Count > 0 ? _stack[0] : null;
			if (newCameraObj == null)
			{
				newCameraObj = _defaultCamera.gameObject;
			}

			if (_active != newCameraObj)
			{
				if (_verboseDebugLogging)
				{
					Debug.Log(string.Format("Camera transition : {0}-->{1}", _active, newCameraObj));
				}

				newCameraObj?.SetActive(true);
				_active?.SetActive(false);
				_active = newCameraObj;
			}
		}
	}
}