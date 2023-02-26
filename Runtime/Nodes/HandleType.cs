
namespace SimpleSplines.Nodes
{
    /// <summary>
    /// The behaviour of the node handles / tangents when being minipulated.
    /// </summary>
    public enum HandleType : byte
    {
        /// <summary>
        /// The forward and backward directions are opposite values.
        /// </summary>
        Mirrored,
        /// <summary>
        /// The forward and backward share the same direction but not length.
        /// </summary>
        Aligned,
        /// <summary>
        /// The forward and backward are independant of each other.
        /// </summary>
        Free,
    }
}
