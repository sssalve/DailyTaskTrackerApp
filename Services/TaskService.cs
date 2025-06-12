using SimpleTaskTrackerApp.Classes;

namespace SimpleTaskTrackerApp.Services
{
    public class TaskService
    {
        private List<TaskItem> tasks = new();
        private List<string> categories = new()
        {
            "Other",
            "School",
            "Work",
            "Home",
            "Health",
            "Family"
        };

        // load all tasks from the task save file
        public async Task<List<TaskItem>> LoadTasks()
        {
            tasks.Clear();

            string filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.txt");

            try
            {
                // check if file exists in writable location
                if (File.Exists(filePath))
                {
                    var lines = await File.ReadAllLinesAsync(filePath);

                    foreach (string line in lines)
                    {
                        string[] split = line.Split(':');

                        // validate we have all parts (Id, Title, Description, Category)
                        if (split.Length >= 4)
                        {
                            tasks.Add(new TaskItem
                            {
                                Id = int.Parse(split[0]),  // using the saved Id
                                Title = split[1],
                                Description = split[2],
                                Category = split[3],
                                IsCompleted = false
                            });
                        }
                    }
                }
                else
                {
                    // load initial tasks from resources if first run
                    await InitializeTaskFile();
                }

                TaskItem.NumberOfTasks = tasks.Count;
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}"); // tasks didnt load right
                return tasks;
            }
        }

        // save all tasks to the file
        public async Task SaveTasks()
        {
            try
            {
                // prepare task strings
                var taskStrings = tasks.Select(t => t.ToString()).ToArray();

                // get the writable app data directory
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.txt");

                // write to the file
                await File.WriteAllLinesAsync(filePath, taskStrings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }
        }

        private async Task InitializeTaskFile()
        {
            try
            {
                // copy from resources to app data if file doesn't exist
                using var stream = await FileSystem.OpenAppPackageFileAsync("tasks.txt");
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                string filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.txt");
                await File.WriteAllTextAsync(filePath, content);
            }
            catch
            {
                // create empty file if no resource exists
                string filePath = Path.Combine(FileSystem.AppDataDirectory, "tasks.txt");
                await File.WriteAllTextAsync(filePath, string.Empty);
            }
        }

        // add a new task and add it to the list
        public TaskItem AddTask(string title, string desc, string category)
        {
            // do not add the task to the Service Class List that is local to this file during this method
            // It will create tasks when the title is empty and we will have empty tasks
            TaskItem task = new();
            task.Id = tasks.Count;
            // add a unique id number
            bool isUnique = false;
            while (!isUnique)
            {
                foreach (var taskItem in tasks)
                {
                    if (task.Id == taskItem.Id)
                    {
                        task.Id++;
                    }
                }
                isUnique = true;
            }
            task.Title = title;
            task.Description = desc;
            task.Category = category;
            task.IsCompleted = false;
            return task;
        }

        public List<string> LoadCategoriesAsync()
        {
            return categories;
        }

    }
}
