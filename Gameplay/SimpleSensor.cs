using UnityEngine;

namespace Platformer
{
	public class SimpleSensor : MonoBehaviour
    {
        private int _collisionCount;

        private void OnEnable()
        {
            _collisionCount = 0;
        }

        public bool State()
        {
            return _collisionCount > 0;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            _collisionCount++;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            _collisionCount--;
        }
    }
}