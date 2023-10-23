using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuffGrupp4.Models
{
    /// <summary>
    /// Represents a team of tokens.
    /// </summary>
    public class Team { 
        public List<Token> TeamTokens { get; set; }
        public Color TeamColor { get; set; }
        public bool AI = false;

        /// <summary>
        /// Constructor for the team class
        /// </summary>
        /// <param name="teamcolor"></param>
        public Team(Color teamcolor)
        {
            TeamTokens = new List<Token>();
            TeamColor = teamcolor;
        }

        /// <summary>
        /// Add token to the team List
        /// </summary>
        /// <param name="token"></param>
        public void AddToken(Token token)
        {
            TeamTokens.Add(token);
        }

        /// <summary>
        /// Clear the team list
        /// </summary>
        public void ClearTokens()
        {
            TeamTokens.Clear();
        }

        /// <summary>
        /// Remove token from the team list
        /// </summary>
        /// <param name="token"></param>
        public void RemoveToken(Token token)
        {
            TeamTokens.Remove(token);
        }

        /// <summary>
        /// Returns the team color
        /// </summary>
        /// <returns>TeamColor</returns>
        public Color getTeamColor()
        {
            return TeamColor;
        }

        /// <summary>
        /// Cheks if the team has tokens on non zero cells in the grid
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>True if a token is not standing on a cell with the value of 0</returns>
        public bool HasTokensOnNonZeroCells(Grid grid)
        {
            foreach (Token token in TeamTokens)
            {
                if (grid.GetTile(token.getCurrentPositionRow(), token.getCurrentPositionCol()) != 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Cheksthe team has tokens on the base
        /// </summary>
        /// <param name="grid"></param>
        /// <returns>True if a token is standing in the base(cell with a value of 0)</returns>
        public bool HasTokensInBase(Grid grid)
        {
            foreach (Token token in this.TeamTokens)
            {
                if (token.isAtBase(grid))
                {
                    return true; // At least one token is at the base.
                }
            }
            return false; // No tokens are at the base.
        }

    }
}
