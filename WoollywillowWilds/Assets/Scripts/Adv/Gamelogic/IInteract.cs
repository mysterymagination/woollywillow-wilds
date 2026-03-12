using UnityEngine.EventSystems;

namespace WildsAdv
{
    /// <summary>
    ///     Handles user interaction story progression e.g. via mouse click.
    /// </summary>
    public interface IInteractHandler
    {
        /// <summary>
        ///     Method <c>OnInteract</c> modifies the game state and/or UI based on interaction with the implementing object.
        /// </summary>
        /// <param name="eventData">
        ///     The pointing device input data from the player.
        /// </param>
        void OnInteract(PointerEventData eventData);
        /// <summary>
        ///     Method <c>GenerateDescription</c> generates and returns a string describing what happens when the player interacts with the implementing object.
        /// </summary>
        /// <returns>A TreasureText describing what happens when the player interacted with this object and the mood for context, with its ToString() suitable for display in the story text view.</returns>
        TreasureText GenerateDescription();
    }
}