using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace FiaMedKnuffGrupp4.Models
{
    public class Team { 
        public List<Token> TeamTokens { get; set; }
        public Color TeamColor { get; set; }
        public PlayerTypeEnum PlayerType { get; set; }

        public Team(Color teamColor, PlayerTypeEnum playerType)
        {
            TeamTokens = new List<Token>();
            TeamColor = teamColor;
            PlayerType = playerType;
        }

        // PlayerType Enum for StartMenu & GameState.
        public enum PlayerTypeEnum
        {
            User,
            Ai,
            // Add more player types as needed
        }

        public void AddToken(Token token)
        {
            TeamTokens.Add(token);
        }

        public void RemoveToken(Token token)
        {
            TeamTokens.Remove(token);
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
