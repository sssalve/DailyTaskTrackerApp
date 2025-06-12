using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskTrackerApp.Classes
{
    public class TaskItem
    {
        public static int NumberOfTasks;
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsCompleted { get; set; }

        public TaskItem()
        {
            NumberOfTasks += 1;
        }

        public override string ToString()
        {
            return Id + ":" + Title + ":" + Description + ":" + Category;
        }

    }
}
