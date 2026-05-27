using UnityEngine;

namespace Platformer
{
	public enum ActionState
	{
		Ready,
		InFlight,
		Cooldown,
	}

	public enum LR
	{
		None,
		Left,
		Right
	}

	public enum Dir4
	{
		None,
		North,
		East,
		South,
		West,
	}

	public static class GameUtil
	{
		public const float NearZero = 0.0001f;

		public static Vector2 DegreeToVector2(float degree)
		{
			return RadianToVector2(degree * Mathf.Deg2Rad);
		}

		public static Vector2 DegreeToVector2(float degree, float length)
		{
			return RadianToVector2(degree * Mathf.Deg2Rad) * length;
		}

		public static LR GetDirectionLR(float v, float threshold = NearZero)
		{
			return v < -threshold ? LR.Left : (v > threshold ? LR.Right : LR.None);
		}

		public static int GetDirectionInt(float v, float threshold = NearZero)
		{
			return v < -threshold ? -1 : (v > threshold ? 1 : 0);
		}

		public static int GetDirectionInt(LR v)
		{
			return v == LR.Left ? -1 : (v == LR.Right ? 1 : 0);
		}

		public static Dir4 GetDirectionDir4(Vector2 v, float threshold = NearZero)
		{
			if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
			{
				return v.x < -threshold ? Dir4.West : (v.x > threshold ? Dir4.East : Dir4.None);
			}
			else
			{
				return v.y < -threshold ? Dir4.South : (v.y > threshold ? Dir4.North : Dir4.None);
			}
		}

		public static LR GetLRFromDir4(Dir4 dir)
		{
			if (dir == Dir4.West)
				return LR.Left;
			else if (dir == Dir4.East)
				return LR.Right;
			else
				return LR.None;
		}

		public static bool FuzzyEquals(float v1, float v2, float threshold = NearZero)
		{
			float d = v1 - v2;
			return d < threshold && d > -threshold;
		}

		public static bool FuzzyEquals(Vector2 v1, Vector2 v2)
		{
			return FuzzyEquals(v1.x, v2.x) && FuzzyEquals(v1.y, v2.y);
		}

		public static Dir4 OppositeDir4(Dir4 x)
		{
			if (x == Dir4.North)
				return Dir4.South;
			else if (x == Dir4.South)
				return Dir4.North;
			else if (x == Dir4.West)
				return Dir4.East;
			else if (x == Dir4.East)
				return Dir4.West;
			else
				return Dir4.None;
		}

		public static LR OppositeLR(LR x)
		{
			if (x == LR.Left)
				return LR.Right;
			else if (x == LR.Right)
				return LR.Left;
			else
				return LR.None;
		}

		public static int HashString(string s)
		{
			int value = 0;
			if (s != null)
				foreach (char c in s.ToLowerInvariant())
					value = c + 31 * value;
			return value;
		}

		public static long HashString64(string s)
		{
			long value = 0;
			if (s != null)
			{
				value = 1125899906842597L; // prime
				foreach (char c in s.ToLowerInvariant())
				{
					value = c + (31 * value);
				}
			}
			return value;
		}

		public static Vector2 RadianToVector2(float radian)
		{
			return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
		}

		public static Vector2 RadianToVector2(float radian, float length)
		{
			return RadianToVector2(radian) * length;
		}

		public static void SnapCameraToPlayer(PlayerController player)
		{
			if (player == null)
				return;

			Cinemachine.CinemachineBrain brain = Object.FindObjectOfType<Cinemachine.CinemachineBrain>();
			Cinemachine.CinemachineVirtualCamera vCam = brain?.ActiveVirtualCamera as Cinemachine.CinemachineVirtualCamera;
			if (vCam != null)
			{
				Vector3 pos = vCam.transform.position;
				pos.x = player.transform.position.x;
				pos.y = player.transform.position.y;
				vCam.ForceCameraPosition(pos, player.transform.rotation);
			}
		}

		// Child Iterator
		//----------------------------------------------------
		public delegate void ChildHandler(GameObject child);

		public static void IterateChildren(GameObject gameObject, ChildHandler childHandler, bool recursive)
		{
			DoIterate(gameObject, childHandler, recursive);
		}

		private static void DoIterate(GameObject gameObject, ChildHandler childHandler, bool recursive)
		{
			foreach (Transform child in gameObject.transform)
			{
				childHandler(child.gameObject);
				if (recursive)
					DoIterate(child.gameObject, childHandler, true);
			}
		}
		//----------------------------------------------------

	}
}