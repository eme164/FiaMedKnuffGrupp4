using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FiaMedKnuffGrupp4.Models
{
    public class Teams
    {
        public List<Team> TeamList { get; set; }

        public Teams()
        {
            TeamList = new List<Team>();
        }



        public void AddTeam(Team team)
        {
            TeamList.Add(team);
        }

        public void RemoveTeam(Team team)
        {
            TeamList.Remove(team);
        }
        public void GetTeams()
        {
            foreach (Team team in TeamList)
            {
                Console.WriteLine(team);
            }
        }
    }
}
