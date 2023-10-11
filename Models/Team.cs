using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

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
    }
}
