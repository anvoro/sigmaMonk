using UnityEngine;

namespace Core
{
    public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T I { get; private set; }

        protected virtual void Awake()
        {
            if (I != null && I != this)
            {
                Destroy(this);
            }
            else
            {
                I = this as T;
            }
        }
    }
}