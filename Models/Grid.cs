namespace FiaMedKnuffGrupp4.Models
{
    /// <summary>
    /// Represents the game board grid that defines the maze layout.
    /// </summary>
    public class Grid
    {
        /// <summary>
        /// Gets the tile value at the specified row and column.
        /// </summary>
        /// <param name="row">The row index of the tile.</param>
        /// <param name="column">The column index of the tile.</param>
        /// <returns>The tile value at the specified position.</returns>
        public int GetTile(int row, int column)
        {
            // Return the tile value at the specified position.
            return gameGrid[row, column];
        }

        /// <summary>
        /// Sets the tile value at the specified row and column.
        /// </summary>
        /// <param name="row">The row index of the tile.</param>
        /// <param name="column">The column index of the tile.</param>
        /// <param name="newTile">The new tile value to set.</param>
        public void SetTile(int row, int column, int newTile)
        {
            // Set the tile value at the specified position to the new value.
            gameGrid[row, column] = newTile;
        }

        /// <summary>
        /// Gets the 2D array representing the entire game grid.
        /// </summary>
        /// <returns>A 2D array representing the game grid layout.</returns>
        public int[,] GetGameGrid()
        {
            // Return the entire game grid.
            return gameGrid;
        }

        // Define the game grid layout using a 2D array.
        // 0 = No Path
        // 1 = Up, 2 = Right,3 = up&right 4 = Down, 5 = down/right 6 = left/up 8 = Left 9 = left/down 11 = greencolumn, 12 = yellowcolumn, 13 = bluecolumn, 14 = redcolumn 15 = goal
        private int[,] gameGrid =
        {
            {0,0,0,0,0,0,2,12,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,4,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,4,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,4,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,4,4,0,0,0,0,0,0 },

            {0,0,0,0,0,0,1,4,5,0,0,0,0,0,0 },
            {2,2,2,2,2,3,0,15,0,2,2,2,2,2,4 },
            {11,2,2,2,2,2,15,15,15,8,8,8,8,8,13},
            {1,8,8,8,8,8,0,15,0,9,8,8,8,8,8 },
            {0,0,0,0,0,0,6,1,4,0,0,0,0,0,0 },

            {0,0,0,0,0,0,1,1,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,1,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,1,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,1,4,0,0,0,0,0,0 },
            {0,0,0,0,0,0,1,14,8,0,0,0,0,0,0 },
        };
    }
}