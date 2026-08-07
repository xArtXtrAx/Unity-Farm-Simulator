using FarmSimulator.Presentation.Interaction;
using UnityEngine;

namespace FarmSimulator.Presentation.Player
{
    /// <summary>
    /// Marks the authoritative player prefab and enforces runtime components
    /// that every playable player instance must provide.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerPrefabIdentity : MonoBehaviour
    {
        private void Awake()
        {
            if (GetComponent<PlayerInteractionController>() == null)
            {
                gameObject.AddComponent<PlayerInteractionController>();
            }
        }
    }
}
