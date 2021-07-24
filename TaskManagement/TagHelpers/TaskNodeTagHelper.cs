using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.TagHelpers

{
    public class TaskNodeTagHelper : TagHelper
    {
        public TaskNode Task { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div id=\"task-viewer\"";
            string listContent = "";
            if (Task != null)
            {
                listContent += $"<span class=\"detail-title\">{Task.Title}</span>";
                listContent += $"<p class=\"detail-description\">{Task.Description}</p>";
                listContent += $"<span class=\"detail-date-register\">{Task.RegisterDate}</span>";
                listContent += $"<span class=\"detail-executors\">{Task.Executors}</span>";
                listContent += $"<span class=\"detail-detail-state\">{Task.TaskState}</span>";
                listContent += $"<span class=\"detail-time-planned\">{Task.ExecutionTimePlanned}</span>";
                listContent += $"<span class=\"detail-time-actual\">{Task.ExecutionTimeActual}</span>";
                if (Task.TaskState == TaskNode.State.Complete)
                    listContent += $"<span class=\"detail-date-complete\">{Task.CompleteDate}</span>";
            }

            output.Content.SetHtmlContent(listContent);
        }
    }
}
