namespace UserInterface.Highlighting
{
    /// <summary>
    /// Interface for classes that draw or control objects with the ability to be visually highlighted.
    /// </summary>
    public interface IHighlightable
    {
        /// <summary>
        /// Turn highlighting on. Use <c>parameters</c> to specify sub-elements, or empty for any/all.
        /// </summary>
        /// <param name="parameters">Additional instructions, e.g. for highlighting only a subset of elements governed
        /// by the implementing script.</param>
        void EnableHighlights(string parameters = "");

        /// <summary>
        /// Turn highlighting off. Use <c>parameters</c> to specify sub-elements, or empty for any/all.
        /// </summary>
        /// <param name="parameters">Additional instructions, e.g. for un-highlighting only a subset of elements governed
        /// by the implementing script.</param>
        void DisableHighlights(string parameters = "");

    }
}
