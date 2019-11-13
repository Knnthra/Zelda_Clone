namespace EksamenJuni2010
{
    using System.Collections.Generic;

    /// <summary>
    /// GameObject comparer.
    /// </summary>
    public class SorterRender : IComparer<GameObject>
    {
        /// <summary>
        /// Compares two GameObjects.
        /// Mainly sorts GameObjects of the type Item. Secondly sorts on GameObjects Y-coordinate.
        /// </summary>
        /// <param name="x">First GameObject to compare</param>
        /// <param name="y">Second GameObject to compare</param>
        /// <returns>Returns an integer that indicates their relationship to one another in the sort oder.</returns>
        public int Compare(GameObject x, GameObject y)
        {
            if (x is Item && !(y is Item))
            {
                return -1;
            }
            else if (!(x is Item) && y is Item)
            {
                return 1;
            }
            else if (x is Item && y is Item)
            {
                return 0;
            }

            if (x.Coords.Y < y.Coords.Y)
            {
                return -1;
            }
            else if (x.Coords.Y > y.Coords.Y)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
