using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuffGrupp4.Models
{
    public class Team { 
        public List<Token> TeamTokens { get; set; }
        public Color TeamColor { get; set; }

        public Team(Color teamcolor)
        {
            TeamTokens = new List<Token>();
            TeamColor = teamcolor;
        }
        public void AddToken(Token token)
        {
            TeamTokens.Add(token);
        }

        public void RemoveToken(Token token)
        {
            TeamTokens.Remove(token);
        }
        public bool HasTokensLeftToMove() 
        {
            //TODO: Implement this method.

            return true;
        }
        public Color getTeamColor()
        {
            return TeamColor;
        }
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
    }
}
