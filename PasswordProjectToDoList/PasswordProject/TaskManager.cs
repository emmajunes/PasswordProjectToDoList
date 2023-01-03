using PasswordProjectToDoList.Models;

namespace PasswordProject
{
    public class TaskManager
    {
        private static UserManager _userManager = new UserManager();

        public static void AddTask(int listId) 
        {
            var loggedInUser = UserManager.LoggedInUser;
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();
            var currentList = json[listId - 1];

            Console.WriteLine("What task do you want to add?: ");
            var task = Console.ReadLine();

            Console.WriteLine("What description do you want to add to the task?: ");
            var description = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(task) || String.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("Input field cannot be empty");
                AddTask(listId);
                return;
            }

            Console.WriteLine("Select a prio 1-5 (optional): ");
            var prio = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(prio))
            {
                prio = "none";
            }

            var newTask = new TaskDto()
            {
                TaskTitle = task,
                TaskDescription = description,
                TaskPrio = prio
            };

            currentList.Tasks.Add(newTask);
    
            FileManagerToDoList.UpdateTaskJson(new List<ToDoListDto> { currentList });

        }

        public static void ViewTasks(int listId)
        {
            Console.Clear();
            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();
            
            var currentList = json[listId - 1];

            Dictionary<string, int> colors = new()
            {
                { "Magenta", 13 },
                { "Yellow", 14 },
                { "Blue", 9 },
                { "Red", 12 },
                { "Cyan", 11 },
                { "White", 15 }
            };

            Console.ForegroundColor = (ConsoleColor)colors[currentList.TitleColor];
            Console.WriteLine("\n" + currentList.ListTitle.ToUpper() + ":\n");

            Console.ForegroundColor = ConsoleColor.White;

            var index = 1;
            var completedCount = 0;
            foreach (var task in currentList.Tasks)
            {
                if (task.Completed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    completedCount++;
                }

                Console.WriteLine($"[{index}]\nTitle: {task.TaskTitle}");
                Console.WriteLine($"Descripton: {task.TaskDescription}\n");
                index++;

                Console.ForegroundColor = ConsoleColor.White;

                if (completedCount == currentList.Tasks.Count)
                {
                    Console.WriteLine("Good job! All tasks are completed!");
                }
            }
        }

        public static void ViewIndividualTask(int taskId, int listId)
        {
            Console.Clear();

            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();
            var currentList = json[listId - 1];
            var currentTask = currentList.Tasks[taskId - 1];
            TaskDto individualTask;

            individualTask = currentTask;

            if (individualTask.Completed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine($"[{taskId}]\nTitle: {individualTask.TaskTitle}");
            Console.WriteLine($"Descripton: {individualTask.TaskDescription}");

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void EditTask(int taskId, int listId)
        {
            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();

            var currentList = json[listId - 1];
            var currentTask = currentList.Tasks[taskId - 1];

            Console.Write("Do you want to write a new title of the task y/n? ");
            var inputTitle = Console.ReadLine().ToUpper();

            if (String.IsNullOrWhiteSpace(inputTitle))
            {
                Console.WriteLine("Input field cannot be empty");
                EditTask(taskId, listId);
                return;
            }

            if (inputTitle == "Y")
            {
                Console.WriteLine("Write a new title: ");
                var newTitle = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(newTitle))
                {
                    Console.WriteLine("List title cannot be empty");
                    return;
                }

                currentTask.TaskTitle = newTitle;
            }

            else
            {
                return;
            }

            Console.Write("Do you want to write a new description of the task y/n? ");
            var inputDescription = Console.ReadLine().ToUpper();

            if (String.IsNullOrWhiteSpace(inputDescription))
            {
                Console.WriteLine("Input cannot be empty");
                EditTask(taskId, listId);
                return;
            }

            if (inputDescription == "Y")
            {
                Console.WriteLine("Write a new description: ");
                var newDescription = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(newDescription))
                {
                    Console.WriteLine("List description cannot be empty");
                    return;
                }

                currentTask.TaskDescription = newDescription;
            }

            var allLists = FileManagerToDoList.GetJson();
            allLists.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
            var union = allLists.Union(json).ToList();
            FileManagerToDoList.UpdateJson(union);
         
        }

        public static void ToggleTask(int listId) 
        {
            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();

            AvailableTasks(listId);

            Console.Write("Select the number of the task that you want to toggle: ");
            var taskId = Console.ReadLine();

            try
            {
                var currentList = json[listId - 1];
                var currentTask = currentList.Tasks[Convert.ToInt32(taskId) - 1];

                currentTask.Completed = !currentTask.Completed;
        
            }

            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Id does not exist. Try again!");
                ToggleTask(listId);
                return;
            }

            catch (FormatException)
            {
                Console.WriteLine("Id must be a number. Try again!");
                ToggleTask(listId);
                return;
            }

            FileManagerToDoList.UpdateListJson(json);

        }

        public void DeleteTask(int? taskId, int listId) 
        {
            var loggedInUser = UserManager.LoggedInUser;
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();

            var currentList = json[listId - 1];

            AvailableTasks(listId);

            int deleteIndex = 0;
            if (taskId == null)
            {
                try
                {
                    Console.Write("Choose a task to delete: ");
                    deleteIndex = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Id must be a number. Try again!");
                    DeleteTask(taskId, listId);
                    return;
                }
            }

            Console.Write("Do you want to delete this task y/n? ");
            var deleteAnswer = Console.ReadLine()?.ToUpper();

            if (String.IsNullOrWhiteSpace(deleteAnswer))
            {
                Console.WriteLine("Input field cannot be empty");
                DeleteTask(taskId, listId);
                return;
            }

            if (deleteAnswer == "Y")
            {
                try
                {
                    if (taskId != null)
                    {
                        deleteIndex = (int)taskId;
                    }

                    var deleteTask = deleteIndex - 1;
                    currentList.Tasks.RemoveAt(deleteTask);
                    FileManagerToDoList.UpdateListJson(json);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Id does not exist. Try again!");
                    DeleteTask(taskId, listId);
                    return;

                }

            }
            else
            {
                Console.Clear();
                var menu = new MenuManager();
                menu.CallTaskMenu(Convert.ToInt32(taskId), listId);
                return;
            }

        }

        public static void SortTasks(int listId)
        {
            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();

            var tasks = json[listId - 1].Tasks;
            AvailableTasks(listId);

            Console.WriteLine("\nSORT TASKS:");

            Console.WriteLine("\n[1] Sort by prio");
            Console.WriteLine("[2] Sort by completed tasks");

            Console.Write("\nSelect an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    tasks = tasks.OrderBy(x => x.TaskPrio).ToList();
                    break;
                case "2":
                    tasks = tasks.OrderByDescending(x => x.Completed).ToList();
                    break;
                default:
                    Console.WriteLine("\nThere are no option recognized to your input. Try again!");
                    Thread.Sleep(1500);
                    Console.Clear();
                    ViewTasks(listId);
                    SortTasks(listId);
                    return;
            }

            json[listId - 1].Tasks = tasks;


            var allLists = FileManagerToDoList.GetJson();
            allLists.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
            var union = allLists.Union(json).ToList();
            FileManagerToDoList.UpdateJson(union);

            ViewTasks(listId);
        }

        public static void AvailableTasks(int listId)
        {
            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();

            var tasks = json[listId - 1].Tasks;

            if (tasks.Count == 0) 
            {
                Console.WriteLine("There are no tasks in the list");
                Thread.Sleep(1500);
                var menu = new MenuManager();
                menu.CallListMenu(listId);
                return;
            }
        }
    }
    
}