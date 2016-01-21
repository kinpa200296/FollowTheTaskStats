using System.Windows.Media;

namespace FollowTheTaskStats.Model
{
    public class UserStats
    {
        public Brush BorderBrush { get; set; }

        public string UserName { get; set; }

        public string Role { get; set; }

        public int Total { get; set; }

        public int TotalCompleted { get; set; }

        public int InTime { get; set; }

        public int NotInTime { get { return TotalCompleted - InTime; } }
    }
}
