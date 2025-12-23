using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSync.Application.Events
{
    public class TaskUpdatedEvent
    {
        public string TaskId { get; set; } = default!;
        public string ProjectId { get; set; } = default!;
        public string UpdatedBy { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
    }
}
