using PasswordProjectToDoList;
using PasswordProjectToDoList.Models;
using System.Text.Json.Serialization.Metadata;

namespace PasswordProject
{
    public class ToDoListService
    {
        private static UserManager _userManager;

        private MenuManager _menuManager = new MenuManager(); 

        public ToDoListService()
        {
            _userManager = new UserManager();
        }

        public void CreateList()
        {
            var json = FileManagerToDoList.GetJson(); 

            Console.Write("Enter a title of the new list: ");
            var title = (Console.ReadLine());

            if (String.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("List title cannot be empty");

                CreateList();
                return;
            }
      
            var newList = new ToDoListDto()
            {
                ListDateTime = DateTime.Now.ToString(),
                ListId = Guid.NewGuid(),
                ListTitle = title,
                Tasks = new List<TaskDto>(),
                UserId = UserManager.LoggedInUser.UserId
            };

            json.Add(newList);

            FileManagerToDoList.UpdateJson(json);

            Console.Clear();
            Console.WriteLine("New created list: " + title);

            var userJson = FileManagerToDoList.GetCurrentLoggedInUsersLists();
            int createdList = userJson.Count;
            ColorList(createdList);
            Console.Clear();
        }

        public void ViewAllLists()
        {

            var loggedinUser = UserManager.LoggedInUser;
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedinUser.UserId).ToList();


            Dictionary<string, int> colors = new()
            {
                { "Magenta", 13 },
                { "Yellow", 14 },
                { "Blue", 9 },
                { "Red", 12 },
                { "Cyan", 11 },
                { "White", 15 }
            };

            Console.WriteLine("\nOVERVIEW OF LISTS: \n");

            int index = 1;
            foreach (var list in json)
            {
                Console.ForegroundColor = (ConsoleColor)colors[list.TitleColor];

                      Console.WriteLine($"[{index}] {list.ListTitle}");
                      index++;
                      Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void ChooseList()
        {
            AvailableLists();

            try
            {
                Console.Write("\nChoose a list or press B to go back: ");
                var input = Console.ReadLine();

                if(input.ToUpper() == "B")
                {
                    return;
                }

                _menuManager.CallListMenu(Convert.ToInt32(input));
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.Clear();
                
                ViewAllLists();
                Console.WriteLine("\nId does not exist. Try again!");
                ChooseList();
            }
            catch (FormatException)
            {
                Console.Clear();
                ViewAllLists();
                Console.WriteLine("\nId must be a number. Try again!");
                ChooseList();
            }
        }

        public void LastCreated() 
        {
            var loggedinUser = UserManager.LoggedInUser;
            var json = FileManagerToDoList.GetJson();

            json = json.Where(x => x.UserId == loggedinUser.UserId).ToList();
            var lastCreated = json.Count;

            AvailableLists();
            TaskManager.ViewTasks(lastCreated);

            _menuManager.CallListMenu(lastCreated);
        }

        public void EditList(int listId)
        {
            var json = FileManagerToDoList.GetCurrentLoggedInUsersLists();
            
            var currentList = json[listId - 1];

            Console.WriteLine("Write a new title to the list: ");
            var title = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("List title cannot be empty");
                EditList(listId);
                return;
            }

            currentList.ListTitle = title;

            var allLists = FileManagerToDoList.GetJson();
            allLists.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
            var union = allLists.Union(json).ToList();
            FileManagerToDoList.UpdateJson(union);

            //FileManagerToDoList.UpdateJson(json);
        }

        public void DeleteList()
        {
            var json = FileManagerToDoList.GetCurrentLoggedInUsersLists();
           
            AvailableLists();
            ViewAllLists();
            int deleteList;

            try
            {
                Console.Write("\nChoose a list to delete: ");
                var deleteIndex = Convert.ToInt32(Console.ReadLine());

                if (deleteIndex <= 0 || json.Count < deleteIndex)
                {
                    Console.Clear();
                    Console.WriteLine("Id does not exist. Try again!");
                    DeleteList();
                    return;
                }

                deleteList = deleteIndex - 1;

                Console.WriteLine("Do you want to delete this list y/n? ");
                var deleteAnswer = Console.ReadLine().ToUpper();

                if (String.IsNullOrWhiteSpace(deleteAnswer))
                {
                    Console.WriteLine("Input field cannot be empty");
                    DeleteList();
                    return;
                }

                if (deleteAnswer == "Y")
                {
                    Console.Clear();
                    Console.WriteLine($"Deleted list: {json[deleteList].ListTitle}");
                    json.RemoveAt(deleteList);

                    var allLists = FileManagerToDoList.GetJson();
                    allLists.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
                    var union = allLists.Union(json).ToList();
                    FileManagerToDoList.UpdateJson(union);
                    return;
                }

                else if (deleteAnswer == "N")
                {
                    return;
                }

                else
                {
                    Console.WriteLine("Answer needs to be a letter of y or n");
                    DeleteList();
                    return;
                }
            }

            catch (ArgumentOutOfRangeException)
            {
                Console.Clear();
                Console.WriteLine("Id does not exist. Try again!");
                DeleteList();
                return;
            }
            catch (FormatException)
            {
                Console.Clear();
                Console.WriteLine("Id must be a number. Try again!");
                DeleteList();
                return;
            }

        }

        public void DeleteAllLists()
        {
            var json = FileManagerToDoList.GetJson();

            AvailableLists();

            Console.WriteLine("Do you want to delete all lists y/n? ");
            var deleteAnswer = Console.ReadLine().ToUpper();

            if (deleteAnswer == "Y")
            {
                Console.Clear();
                Console.WriteLine("All lists deleted");
                json.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
                FileManagerToDoList.UpdateJson(json);
                return;
            }

            else if (deleteAnswer == "N")
            {
                Console.Clear();
                return;
            }

            else
            {
                Console.WriteLine("Answer needs to be a letter of y or n");
                DeleteAllLists();
                return;
            }

        }

        public void ColorList(int listId)
        {
            var json = FileManagerToDoList.GetCurrentLoggedInUsersLists();
            var currentList = json[listId - 1];

            Console.WriteLine("\n[1] Magenta");
            Console.WriteLine("[2] Yellow");
            Console.WriteLine("[3] Blue");
            Console.WriteLine("[4] Red");
            Console.WriteLine("[5] Cyan");
            Console.WriteLine("[6] White");

            Console.WriteLine("\nSelect color to list title: ");
            var colorId = Console.ReadLine();

            switch (colorId)
            {
                case "1":
                    currentList.TitleColor = "Magenta";
                    break;
                case "2":
                    currentList.TitleColor = "Yellow";
                    break;
                case "3":
                    currentList.TitleColor = "Blue";
                    break;
                case "4":
                    currentList.TitleColor = "Red";
                    break;
                case "5":
                    currentList.TitleColor = "Cyan";
                    break;
                case "6":
                    currentList.TitleColor = "White";
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("There are no option recognized to your input Try again!");
                    ColorList(listId);
                    return;
            }

            var allLists = FileManagerToDoList.GetJson();
            allLists.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
            var union = allLists.Union(json).ToList();
            FileManagerToDoList.UpdateJson(union);
        }

        public void SortList()
        {
            var json = FileManagerToDoList.GetJson();
            AvailableLists();

            Console.WriteLine("\n[1] Sort by latest created list");
            Console.WriteLine("[2] Sort by oldest created list");
            Console.WriteLine("[3] Sort by name");
            Console.WriteLine("[4] Sort by color");

            Console.Write("\nSelect an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    json = json.OrderByDescending(x => x.ListDateTime).ToList();
                    break;
                case "2":
                    json = json.OrderBy(x => x.ListDateTime).ToList();
                    break;
                case "3":
                    json = json.OrderBy(x => x.ListTitle).ToList();
                    break;
                case "4":
                    json = json.OrderBy(x => x.TitleColor).ToList();
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("\nThere are no option recognized to your input. Try again!");
                    SortList();
                    return;
            }

            FileManagerToDoList.UpdateJson(json);
            Console.Clear();
          
            ViewAllLists();
        }

        public void AvailableLists()
        {
            var loggedinUser = UserManager.LoggedInUser;
            var json = FileManagerToDoList.GetJson();

            if (!loggedinUser.IsAdmin()) 
            {
                json = json.Where(x => x.UserId == loggedinUser.UserId).ToList();
            }

            if (json.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("No lists available - press any key to go back");

                Console.ReadLine();
                _menuManager.CallStartMenu();                
            }
        }
    }
}