using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FiaMedKnuffGrupp4.Models
{   /// <summary>
/// Represents all the teams in the game
/// </summary>
    public class Teams
    {
        public List<Team> TeamList { get; set; }
        /// <summary>
        /// constructor for the teams class
        /// </summary>
        public Teams()
        {
            TeamList = new List<Team>();
        }


        /// <summary>
        /// Add a team to the team list
        /// </summary>
        /// <param name="team"></param>
        public void AddTeam(Team team)
        {
            TeamList.Add(team);
        }

        /// <summary>
        /// Remove a team from the team list
        /// </summary>
        /// <param name="team"></param>
        public void RemoveTeam(Team team)
        {
            TeamList.Remove(team);
        }

        /// <summary>
        /// Write out all the teams in the team list
        /// </summary>
        public void GetTeams()
        {
            foreach (Team team in TeamList)
            {
                Console.WriteLine(team);
            }
        }
    }
}
