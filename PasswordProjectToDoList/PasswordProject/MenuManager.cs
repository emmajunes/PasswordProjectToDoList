using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace PasswordProject
{
    public class MenuManager
    {
        public static int UserPosition = -1;
        public static int LoggedInUserPosition = -1;
        private UserManager _userManager;

        public MenuManager()
        {
            _userManager = new UserManager();
        }
        public void StartMenu()
        {
            _userManager.GetUsers();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nMENU LOGIN\n");

                Console.WriteLine("[1] Login");
                Console.WriteLine("[2] Create account");
                Console.WriteLine("[3] Quit program");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "1":
                        _userManager.LogInToSystem();
                        break;
                    case "2":
                        _userManager.CreateUser();
                        Console.Clear();
                        Console.WriteLine("You created your account. Log in with your details.");
                        StartMenu();
                        break;
                    case "3":
                        isRunning = Quit(isRunning);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        StartMenu();
                        return;
                        
                }
            }
        }

        public void UserSystemMenu()
        {
            Console.Clear();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nMENU USERSYSTEM\n");

                Console.WriteLine("[1] Edit my account");
                Console.WriteLine("[2] Delete my account");
                Console.WriteLine("[3] Go back");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "1":
                        _userManager.GetIndividualUser();
                        EditMenu();
                        break;
                    case "2":
                        _userManager.GetIndividualUser();
                        _userManager.DeleteUser();
                        break;
                    case "3":
                        CallStartMenu();
                        break;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        UserSystemMenu();
                        return;

                       
                }
            }
        }

        public void UserSystemMenuModerator()
        {
            Console.Clear();
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nMENU USERSYSTEM FOR MODERATOR\n");

                Console.WriteLine("[1] Edit my account");
                Console.WriteLine("[2] Delete my account");
                Console.WriteLine("[3] View all users");
                Console.WriteLine("[4] Log out");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "1":
                        UserPosition = -1;
                        _userManager.GetIndividualUser();
                        EditMenu();
                        break;
                    case "2":
                        UserPosition = -1;
                        _userManager.GetIndividualUser();
                        _userManager.DeleteUser();
                        break;
                    case "3":
                        _userManager.GetAllUsernames();
                        _userManager.SelectUser();
                        _userManager.GetIndividualUser();
                        if (_userManager.Users[UserPosition].Access == "User")
                        {
                        _userManager.PromoteUserModerator();
                        }
                        if (_userManager.Users[UserPosition].Access == "Moderator" || _userManager.Users[UserPosition].Access == "Admin")
                        {
                            Console.WriteLine("\nPress enter to go back to menu");
                            Console.ReadKey();
                            Console.Clear();
                        }                      
                        break;
                    case "4":
                        isRunning = LogOut(isRunning);
                        if (!isRunning)
                        {
                            Console.Clear();
                            StartMenu();
                        }
                        break;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        UserSystemMenuModerator();
                        return;
                }
            }
        }
        public void UserSystemMenuAdmin()
        {
            Console.Clear();
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nMENU USERSYSTEM FOR ADMIN\n");

                Console.WriteLine("[1] Create account");
                Console.WriteLine("[2] View all users");
                Console.WriteLine("[3] Log out");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "1":
                        _userManager.CreateUser();
                        Console.Clear();
                        Console.WriteLine("New created user: " + _userManager.Users[UserPosition].Username);
                        break;
                    case "2":
                        Console.Clear();
                        _userManager.GetAllUsernames();
                        _userManager.SelectUser();
                        _userManager.GetIndividualUser();
                        UserMenuForAdmin();
                        break;
                    case "3":
                        isRunning = LogOut(isRunning);
                        if (!isRunning)
                        {
                            Console.Clear();
                            StartMenu();
                        }
                        break;
                    default:
                        Console.Clear();                       
                        Console.WriteLine("There is no option recognized to your input. Try again!");   
                        break;
                }
            }
        }

        public void UserMenuForAdmin()
        {

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nMENU INDIVIDUAL USER\n");

                Console.WriteLine("[1] Edit account");
                Console.WriteLine("[2] Promote user");
                Console.WriteLine("[3] Demote user");
                Console.WriteLine("[4] Delete user");
                Console.WriteLine("[5] Go back to menu for usersystem");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "1":
                        _userManager.GetIndividualUser();
                        EditMenu();
                        break;
                    case "2":
                        Console.Clear();
                        _userManager.GetIndividualUser();
                        _userManager.PromoteUserAdmin();
                        break;
                    case "3":
                        Console.Clear();
                        _userManager.GetIndividualUser();
                        _userManager.DemoteUser();
                        break;
                    case "4":
                        if (_userManager.Users[LoggedInUserPosition].Access == "Admin")
                        {
                            _userManager.DeleteUser();
                        }
                        else
                        {
                            Console.WriteLine("Only admin have access to delete a user");
                        }                      
                        UserSystemMenuAdmin();
                        break;
                    case "5":
                        UserSystemMenuAdmin();
                        return;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        _userManager.GetIndividualUser();
                        UserMenuForAdmin();
                        return;                  
                }
            }
        }


        public void EditMenu()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nEDIT USER\n");

                Console.WriteLine("[1] Edit username");
                Console.WriteLine("[2] Edit password");
                Console.WriteLine("[3] Go back to user menu");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine().Trim();

                switch (input)
                {
                    case "1":
                        _userManager.EditUserName();
                        Console.Clear();
                        _userManager.GetIndividualUser();
                        break;
                    case "2":
                        _userManager.EditPassword();
                        Console.Clear();
                        _userManager.GetIndividualUser();
                        break;
                    case "3":
                        GoBackToUserMenu();
                        break;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        _userManager.GetIndividualUser();
                        EditMenu();
                        break;

                }
            }
        }

        public void GoBackToUserMenu()
        {
            if (LoggedInUserPosition >= 0 && _userManager.Users[LoggedInUserPosition].Access == "Admin")
            {
                _userManager.GetAllUsernames();
                UserSystemMenuAdmin();
            }
            else if (LoggedInUserPosition > 0 && _userManager.Users[LoggedInUserPosition].Access == "Moderator")
            {
                UserSystemMenuModerator();
            }
            else
            {
                _userManager.GetIndividualUser();
                UserSystemMenu();
            }
        }

        public void CallStartMenu()
        {
            Console.Clear();

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nLIST MENU\n");

                Console.WriteLine("[1] Create a new list");
                Console.WriteLine("[2] View all lists");
                Console.WriteLine("[3] Open last created list");
                Console.WriteLine("[4] Sort lists");
                Console.WriteLine("[5] Delete list");
                Console.WriteLine("[6] Delete all lists");
                Console.WriteLine("[7] Go to user settings");
                Console.WriteLine("[8] Log out");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine();
                var list = new ToDoListService();
                switch (input)
                {
                    case "1":  
                      list.CreateList();
                        break;
                    case "2":
                        Console.Clear();
                        list.ViewAllLists();
                        list.ChooseList();
                        break;
                    case "3":
                        list.LastCreated();
                        break;
                    case "4":
                        list.SortList();
                        break;
                    case "5":
                        list.DeleteList();
                        break;
                    case "6":
                        list.DeleteAllLists();
                        break;
                    case "7":
                        UserSystemMenu();
                        break;
                    case "8":
                        isRunning = LogOut(isRunning);
                        if (!isRunning)
                        {
                            Console.Clear();
                            StartMenu();
                        }
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("There are no option recognized to your input. Try again!");
                        break;
                }

            }

        }

        public void CallListMenu(int listId)
        {
            bool isRunning = true;

            while (isRunning)
            {
                TaskManager.ViewTasks(listId);

                Console.WriteLine("\nLIST MENU\n");

                Console.WriteLine("[1] Edit title of list");
                Console.WriteLine("[2] Color title of list");
                Console.WriteLine("[3] Add task");
                Console.WriteLine("[4] View task");
                Console.WriteLine("[5] Toggle task");
                Console.WriteLine("[6] Sort tasks");
                Console.WriteLine("[7] Delete task");
                Console.WriteLine("[8] Go back to Start menu");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine();
                var list = new ToDoListService();

                switch (input)
                {
                    case "1":
                        list.EditList(listId);
                        break;
                    case "2":
                        list.ColorList(listId);
                        break;
                    case "3":
                        TaskManager.AddTask(listId);
                        break;
                    case "4":
                        ChooseTaskMenu(listId);
                        break;
                    case "5":
                        TaskManager.ToggleTask(listId);
                        break;
                    case "6":
                        TaskManager.SortTasks(listId);
                        break;
                    case "7":
                        var task = new TaskManager();
                        task.DeleteTask(null, listId);
                        break;
                    case "8":
                        Console.Clear();
                        CallStartMenu();
                        break;
                    default:
                        Console.WriteLine("There are no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        break;

                }
            }
        }

        public void CallTaskMenu(int taskId, int listId)
        {
            bool isRunning = true;

            while (isRunning)
            {
                TaskManager.ViewIndividualTask(taskId, listId);

                Console.WriteLine("\nTASK MENU\n");

                Console.WriteLine("[1] Edit task");
                Console.WriteLine("[2] Delete task");
                Console.WriteLine("[3] Go back to List menu");
                Console.WriteLine("[4] Go back to Start menu");

                Console.Write("\nSelect an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        TaskManager.EditTask(taskId, listId);
                        break;
                    case "2":
                        var task = new TaskManager();
                        task.DeleteTask(taskId, listId);
                        CallListMenu(listId);
                        return;
                    case "3":
                        Console.Clear();
                        CallListMenu(listId);
                        break;
                    case "4":
                        Console.Clear();
                        CallStartMenu();
                        break;
                    default:
                        Console.WriteLine("There are no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        break;
                }

            }

        }

        public void ChooseTaskMenu(int listId)
        {
            _userManager.GetUsers();
            var loggedInUser = _userManager.Users[MenuManager.LoggedInUserPosition];
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedInUser.UserId).ToList();

            var tasks = json[listId - 1].Tasks;

            int taskId;

            if (tasks.Count == 0)
            {
                Console.WriteLine("There are no tasks in the list");
                Thread.Sleep(1500);
                return;
            }

            try
            {
                Console.WriteLine("Which task do you want to view?");
                taskId = Convert.ToInt32(Console.ReadLine());

                if (taskId <= 0 || tasks.Count < taskId)
                {
                    Console.Clear();
                    TaskManager.ViewTasks(listId);
                    Console.WriteLine("Id does not exist. Try again!");
                    ChooseTaskMenu(listId);
                    return;
                }
            }
            catch (FormatException)
            {
                Console.Clear();
                TaskManager.ViewTasks(listId);
                Console.WriteLine("Id must be a number. Try again!");
                ChooseTaskMenu(listId);
                return;
            }

            CallTaskMenu(taskId, listId);
        }

        public bool LogOut(bool isRunning)
        {
            Console.Write("\nDo you want to log out y/n? ");
            var input = Console.ReadLine().ToUpper();

            if (String.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty");
                LogOut(isRunning);
                return isRunning;
            }

            if (input == "Y")
            {
                Console.WriteLine("\nLogging out...");
                Thread.Sleep(2000);
                UserPosition = -1;
                LoggedInUserPosition = -1;
                isRunning = false;
                return isRunning;
            }

            return isRunning;
        }

        public bool Quit(bool isRunning)
        {
            Console.Write("\nDo you want to quit y/n? ");
            var exit = Console.ReadLine().ToUpper();

            if (String.IsNullOrWhiteSpace(exit))
            {
                Console.WriteLine("Input cannot be empty");
                Quit(isRunning);
                return isRunning;
            }

            if (exit == "Y")
            {
                Environment.Exit(0);
            }

            Console.Clear();

            return isRunning;
        }
    }
}