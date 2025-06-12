using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTaskTrackerApp.Classes;

namespace SimpleTaskTrackerApp.Components.Pages
{
    public partial class Home // needs to exists as partial
    {
        private List<TaskItem> tasks = new(); // list of all tasks
        private List<string> categories = new(); // used to add categories to tasks
        private string menuTitle = "Task List"; // the title on the top of each menu
        private string newTaskTitle = string.Empty;
        private string newTaskDescription = string.Empty;
        private string newTaskCategory = string.Empty;
        private bool isLoading = true;
        private bool isAddingTask = false; // will be used to have a seperate UI for adding a new task
        private bool didntSaveTasks = false; // if set to true, send a pop up to notify user
        private bool errorWhileAddingTask = false; // notify user if they failed to add a Task Title

        // runs on launch of app
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = true;
                categories = TS.LoadCategoriesAsync();
                tasks = await TS.LoadTasks();// load tasks from file
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading tasks: {ex.Message}"); // error while loading
            }
            finally
            {
                isLoading = false; // done
            }
            await base.OnInitializedAsync();
        }

        private void StartTaskAddingProcess()
        {
            isAddingTask = true;
            menuTitle = "Adding Task";
            StateHasChanged();
        }

        private void CancelTaskAddingProcess()
        {
            errorWhileAddingTask = false;
            isAddingTask = false;
            ClearNewTaskFields();
            menuTitle = "Task list";
            StateHasChanged();
        }

        private async Task AddNewTask(string title, string description, string category)
        {
            var newTask = TS.AddTask(title, description, category);
            if (!tasks.Contains(newTask) && !string.IsNullOrEmpty(title)) // make sure task adds properly. Was adding the task twice before
            {
                tasks.Add(newTask);
                isAddingTask = false;
                errorWhileAddingTask = false;
                ClearNewTaskFields();
                menuTitle = "Task list";
            }
            else
            {
                errorWhileAddingTask = true;
            }
            await TS.SaveTasks(); // save right away
            StateHasChanged(); // uses to refresh the UI
        }

        private async Task UpdateTask(TaskItem task)
        {
            await TS.SaveTasks();
            StateHasChanged();
        }

        private async Task DeleteTask(TaskItem task) // Remove the task from list
        {
            tasks.Remove(task); // remove from list
            TaskItem.NumberOfTasks = tasks.Count; // reduce task count
            await TS.SaveTasks(); // save right away
            StateHasChanged(); // update ui
        }

        private void ClearNewTaskFields()
        {
            newTaskTitle = string.Empty; // clear new vars to make sure the text fields are empty after adding
            newTaskDescription = string.Empty;
            newTaskCategory = string.Empty;
        }

    }
}
