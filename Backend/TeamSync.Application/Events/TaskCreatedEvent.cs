using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSync.Application.Events
{
    public class TaskCreatedEvent
    {
        public string TaskId { get; set; } = default!;
        public string ProjectId { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}