using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class TaskService
{
    public void MarkTaskDone(bool succeeded)
    {
        if (succeeded)
        {
            ProcrastinationMetrics.TasksCompleted.Add(1);
        }
        else
        {
            ProcrastinationMetrics.TasksNeverDone.Add(1);
        }
    }
}