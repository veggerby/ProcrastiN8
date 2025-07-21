using ProcrastiN8.Metrics;

namespace ProcrastiN8.Services;

public class TaskService
{
    // Increment value for marking a task as done or not done
    private const int TaskDoneIncrement = 1;

    public void MarkTaskDone(bool succeeded)
    {
        if (succeeded)
        {
            ProcrastinationMetrics.TasksCompleted.Add(TaskDoneIncrement);
        }
        else
        {
            ProcrastinationMetrics.TasksNeverDone.Add(TaskDoneIncrement);
        }
    }
}