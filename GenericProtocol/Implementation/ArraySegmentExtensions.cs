using System;

namespace GenericProtocol.Implementation
{
    public static class ArraySegmentExtensions
    {
        /// <summary>
        ///     Take a portion of this <see cref="ArraySegment{T}" />
        /// </summary>
        /// <param name="segment">this</param>
        /// <param name="start">The start index of the slice</param>
        /// <param name="count">The length of the slice</param>
        public static ArraySegment<T> SliceEx<T>(this ArraySegment<T> segment, int start, int count)
        {
            if (segment == default(ArraySegment<T>)) throw new ArgumentNullException(nameof(segment));
            if (start < 0 || start >= segment.Count) throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 1 || start + count > segment.Count) throw new ArgumentOutOfRangeException(nameof(count));

            return new ArraySegment<T>(segment.Array, start, count);
        }
    }
}