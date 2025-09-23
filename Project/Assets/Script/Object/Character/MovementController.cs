using UnityEngine;

namespace Backend.Object.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        protected Rigidbody Rigidbody;

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
