namespace EksamenJuni2010
{
    using System.Collections.Generic;

    /// <summary>
    /// Highscore comparer.
    /// </summary>
    public class SorterScores : IComparer<string>
    {
        /// <summary>
        /// Compares two scores, putting the lowest scores first.
        /// </summary>
        /// <param name="x">First highscore line to compare.</param>
        /// <param name="y">Second highscore line to compare.</param>
        /// <returns>Returns an integer that indicates their relationship to one another in the sort order.</returns>
        public int Compare(string x, string y)
        {
            int xH = int.Parse(x.Split(';')[0]);
            int yH = int.Parse(y.Split(';')[0]);

            if (xH == yH)
            {
                return 0;
            }
            else if (xH > yH)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
