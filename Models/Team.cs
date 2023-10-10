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
            teamcolor = TeamColor;
        }
        public void AddToken(Token token)
        {
            TeamTokens.Add(token);
        }

        public void RemoveToken(Token token)
        {
            TeamTokens.Remove(token);
        }
    }
}
