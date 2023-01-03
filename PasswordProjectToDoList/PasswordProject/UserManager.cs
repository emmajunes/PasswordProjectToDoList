using System.Net.Mail;

namespace PasswordProject
{
    public class UserManager
    {
        public static int countId;
        public List<User> Users { get; set; }= new List<User>();
        private static string _path = ApplicationConstants.UserPath;
        public static User LoggedInUser { get; set; }

        public void GetUsers()
        {
            Users = FileManager.GetJson<User>(_path);

        }
        public void CreateUser()
        {
            GetUsers();

            Console.WriteLine("\nCREATE ACCOUNT");

            Console.WriteLine("\nEnter username: ");
            string username = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty. Try again!");
                CreateUser();
                return;
            }

            foreach (var user in Users)
            {
                if (user.Username == username)
                {
                    Console.WriteLine("Username already exists. Try again!");
                    CreateUser();
                    return;
                }
            }

            Console.WriteLine("Enter email: ");
            string email = Console.ReadLine();

            var isValidEmail = ValidateEmail(email);

            if (!isValidEmail)
            {
                Console.WriteLine("Invalid email!");
                CreateUser();
                return;
            }

            Console.WriteLine("Enter a strong password: ");
            string password = HidePassword();

            var isValidPassword = ValidatePassword(password);

            if (!isValidPassword || password.Contains("\u0000") || password.Length < 8)
            {
                Console.WriteLine("Invalid password! ");
                Console.WriteLine("Password needs to be at least 8 characters long.");
                Console.WriteLine("Include a specialcharacter.");
                Console.WriteLine("Include at least one uppercase character.");
                Console.WriteLine("Include at least one lowercase character.");
                Console.WriteLine("Include at least one number.\n");
                CreateUser();
                return;
            }

            var newUser = new User()
            {
                Username = username,
                Email = email,
                Password = password,
                Access = "User",
                UserId = Guid.NewGuid(),
            };
                
            Users.Add(newUser);

            MenuManager.UserPosition = Users.IndexOf(newUser);
            FileManager.UpdateJson(_path, Users);

        }

        public bool ValidateEmail(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            foreach (var user in Users)
            {
                if (user.Email == email)
                {
                    Console.WriteLine("\nEmail already exists!");
                    return false;
                }
            }

            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    
        public bool ValidatePassword(string passWord)
        {
            int validConditions = 0;
            foreach (char c in passWord)
            {
                if (c >= 'a' && c <= 'z')
                {
                    validConditions++;
                    break;
                }
            }
            foreach (char c in passWord)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    validConditions++;
                    break;
                }
            }
            if (validConditions == 0) return false;
            foreach (char c in passWord)
            {
                if (c >= '0' && c <= '9')
                {
                    validConditions++;
                    break;
                }
            }
            if (validConditions == 1) return false;
            if (validConditions == 2)
            {
                char[] special = { '@', '#', '$', '%', '^', '&', '+', '=', '!', '/', '?', '*', '-', '[', ']', '"', '(', ')', '{', '}', '~', '¤','´' };
                if (passWord.IndexOfAny(special) == -1) return false;
            }
            return true;
        }
        public string HidePassword()
        {
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    password += info.KeyChar;
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring(0, password.Length - 1);
                        int pos = Console.CursorLeft;
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(pos - 1, Console.CursorTop);
                    }
                }
                info = Console.ReadKey(true);
            }

            Console.WriteLine();
            return password;
        }

        public void LogInToSystem()
        {
            var maxAttempts = 2;
            var currentAttempt = 0;

            for (int i = 0; i < maxAttempts; i++)
            {
                Console.WriteLine("\nEnter username: ");
                var login = Console.ReadLine();

                foreach (var user in Users)
                {
                    if (user.Username == login)
                    {
                        MenuManager.LoggedInUserPosition = Users.IndexOf(user);
                        LoggedInUser = user;                        
                    }
                }

                if (MenuManager.LoggedInUserPosition == -1)
                {
                    Console.WriteLine("User does not exist!");
                    currentAttempt++;
                    continue;
                }

                currentAttempt = 0;

                for (int p = 0; p < maxAttempts; p++)
                {
                    Console.WriteLine("Enter password: ");
                    var passwordInput = HidePassword();

                    if (String.IsNullOrWhiteSpace(passwordInput))
                    {
                        Console.WriteLine("Password field cannot be empty");
                        passwordInput = HidePassword();
                    }

                    if (Users[MenuManager.LoggedInUserPosition].Password != passwordInput)
                    {
                        currentAttempt++;

                        if (currentAttempt < 2)
                        {
                            Console.WriteLine("Wrong password. Try again!\n");
                        }
                    }

                    var menu = new MenuManager();

                    if (Users[MenuManager.LoggedInUserPosition].Password == passwordInput)
                    {
                        Console.WriteLine("\nLogging in...");
                        Thread.Sleep(2000);
                        if (Users[MenuManager.LoggedInUserPosition].Access == "Admin")
                        {
                            menu.UserSystemMenuAdmin();
                        }
                        else if (Users[MenuManager.LoggedInUserPosition].Access == "Moderator")
                        {
                            menu.UserSystemMenuModerator();
                        }
                        else
                        {
                            menu.CallStartMenu();
                        }
                        break;
                    }
                }
            }

            Console.WriteLine("To many tries!");
        }

        public void SelectUser()
        {
            try
            {
                int selectedInput = 0;
                var validSelection = false;

                while(!validSelection)
                {
                    Console.WriteLine("\nSelect user or press B to go back: ");
                    var input = Console.ReadLine();

                    if(input.ToUpper() == "B")
                    {
                        var menu = new MenuManager();
                        menu.UserSystemMenuAdmin();
                        return;
                    }

                    if (!int.TryParse(input, out selectedInput) || String.IsNullOrWhiteSpace(selectedInput.ToString()) || selectedInput <= 0)
                    {
                        Console.WriteLine("Input cannot be empty and needs to be a value over 0");
                        SelectUser();
                        return;
                    }

                    validSelection = true;
                }

                MenuManager.UserPosition = selectedInput - 1;
        }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("User index does not exist!");
            }
            catch (FormatException)
            {
                Console.WriteLine("Wrong input!");
            }
        }
        public void GetAllUsernames()
        {
            GetUsers();

            Console.WriteLine("\nOVERVIEW OF USERS: \n");
            var index = 1;
            foreach (var user in Users)
            {
                Console.WriteLine($"[{index}] {user.Username} ({user.Access})");
                index++;
            }
        }

        public void GetIndividualUser()
        {
            Console.Clear();

            GetUsers();

            if (MenuManager.UserPosition < 0)
            {
                Console.WriteLine("\nINFO ABOUT USER: " + Users[MenuManager.LoggedInUserPosition].Username.ToUpper());
                Console.WriteLine("USERNAME: " + Users[MenuManager.LoggedInUserPosition].Username);
                Console.WriteLine("EMAIL: " + Users[MenuManager.LoggedInUserPosition].Email);
                Console.WriteLine("PASSWORD IS NOT AVAILABLE HERE");
                Console.WriteLine("ACCESS: " + Users[MenuManager.LoggedInUserPosition].Access);
            }

            try
            {
                if (MenuManager.UserPosition >= 0)
                {
                    Console.WriteLine("\nINFO ABOUT USER: " + Users[MenuManager.UserPosition].Username.ToUpper());
                    Console.WriteLine("USERNAME: " + Users[MenuManager.UserPosition].Username);
                    Console.WriteLine("EMAIL: " + Users[MenuManager.UserPosition].Email);
                    Console.WriteLine("PASSWORD IS NOT AVAILABLE HERE");
                    Console.WriteLine("ACCESS: " + Users[MenuManager.UserPosition].Access);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                GetAllUsernames();
                Console.WriteLine("\nUser index does not exist. Try again!");
                SelectUser();
                GetIndividualUser();
                return;
            }
        }


        public void PromoteUserAdmin()
        {
            GetUsers();

            var selectedUser = Users[MenuManager.UserPosition];

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nPromote user to: \n");
                Console.WriteLine("[1] Admin");
                Console.WriteLine("[2] Moderator");
                Console.WriteLine("[3] Go back to menu");

                Console.WriteLine("\nSelect an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        selectedUser.Access = "Admin";
                        FileManager.UpdateJson(_path, Users);
                        Console.Clear();
                        GetIndividualUser();
                        return;
                    case "2":
                        selectedUser.Access = "Moderator";
                        FileManager.UpdateJson(_path, Users);
                        Console.Clear();
                        GetIndividualUser();
                        return;
                    case "3":
                        var menu = new MenuManager();
                        GetIndividualUser();
                        menu.UserMenuForAdmin();
                        break;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        GetIndividualUser();
                        PromoteUserAdmin();
                        return;

                }

            }

        }

        public void PromoteUserModerator()
        {
            var selectedUser = Users[MenuManager.UserPosition];

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nPromote user to: \n");
                Console.WriteLine("[1] Moderator");
                Console.WriteLine("[2] Go back to menu");

                Console.WriteLine("\nSelect an option: ");
                var input = Console.ReadLine();

                switch (input)
                {

                    case "1":
                        selectedUser.Access = "Moderator";
                        FileManager.UpdateJson(_path, Users);
                        Console.Clear();
                        GetIndividualUser();
                        return;
                    case "2":
                        Console.Clear();
                        var menu = new MenuManager();
                        menu.UserSystemMenuModerator();
                        break;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        GetIndividualUser();
                        PromoteUserModerator();
                        return;

                }

            }

        }

        public void DemoteUser()
        {
            var selectedUser = Users[MenuManager.UserPosition];
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nDemote user to: \n");
                Console.WriteLine("[1] User");
                Console.WriteLine("[2] Moderator");
                Console.WriteLine("[3] Go back to user menu");

                Console.WriteLine("\nSelect an option: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        selectedUser.Access = "User";
                        FileManager.UpdateJson(_path, Users);
                        Console.Clear();
                        GetIndividualUser();
                        return;
                    case "2":
                        if(selectedUser.Access == "User")
                        {
                            Console.WriteLine("You can not demote user to moderator");
                            Thread.Sleep(1500);
                            GetIndividualUser();
                            return;
                        }
                        else
                        {
                            selectedUser.Access = "Moderator";
                            FileManager.UpdateJson(_path, Users);
                            Console.Clear();
                            GetIndividualUser();
                        }
                        return;
                    case "3":
                        Console.Clear();
                        var menu = new MenuManager();
                        GetIndividualUser();
                        menu.UserMenuForAdmin();
                        break;
                    default:
                        Console.WriteLine("There is no option recognized to your input. Try again!");
                        Thread.Sleep(1500);
                        Console.Clear();
                        GetIndividualUser();
                        DemoteUser();
                        return;

                }

            }

            FileManager.UpdateJson(_path, Users);
        }

        public void DeleteUser()
        {
            Console.WriteLine("\nDo you want to delete this user y/n? ");
            var deleteAnswer = Console.ReadLine().ToUpper();

            if (String.IsNullOrWhiteSpace(deleteAnswer))
            {
                Console.WriteLine("Input field cannot be empty");
                DeleteUser();
                return;
            }

            if (deleteAnswer == "Y")
            {
                Console.Clear();

                if (MenuManager.UserPosition < 0)
                {
                    Users.RemoveAt(MenuManager.LoggedInUserPosition);
                    FileManager.UpdateJson(_path, Users);

                    Console.WriteLine("You deleted your account.");
                    Thread.Sleep(1500);
                    var menu = new MenuManager();
                    menu.StartMenu();
                    return;
                }
                if (MenuManager.UserPosition >= 0)
                {
                    Console.WriteLine("Deleted user: " + Users[MenuManager.UserPosition].Username);
                    Users.RemoveAt(MenuManager.UserPosition);
                    FileManager.UpdateJson(_path, Users);
                    return;
                }
            }
            else if (deleteAnswer == "N")
            {
                return;
            }
            else
            {
                Console.WriteLine("Answer needs to be a letter of y or n");
                DeleteUser();
                return;
            }
        }
        public void EditUserName()
        {
            Console.WriteLine("Write a new username: ");
            var newUsername = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(newUsername))
            {
                Console.WriteLine("List title cannot be empty");
                EditUserName();
                return;
            }
            foreach (var user in Users)
            {
                if (user.Username == newUsername)
                {
                    Console.WriteLine("Username already exists. Try again!");
                    EditUserName();
                    return;
                }
            }

            if (MenuManager.UserPosition < 0)
            {
                var loggedInUser = Users[MenuManager.LoggedInUserPosition];
                loggedInUser.Username = newUsername;
            }
            if (MenuManager.UserPosition >= 0)
            {
                var selectedUser = Users[MenuManager.UserPosition];
                selectedUser.Username = newUsername;

            }

            FileManager.UpdateJson(_path, Users);
        }

        public void EditPassword()
        {
            Console.WriteLine("\nCurrent password: ");
            var currentPassword = HidePassword();

            if (MenuManager.UserPosition < 0)
            {
                var loggedInUser = Users[MenuManager.LoggedInUserPosition];
                if(loggedInUser.Password == currentPassword)
                {
                    Console.WriteLine("Password confirmed");
                }
                else
                {
                    Console.WriteLine("Wrong password!");
                    EditPassword();
                    return;
                }
            }
            if (MenuManager.UserPosition >= 0)
            {
                var selectedUser = Users[MenuManager.UserPosition];
                if (selectedUser.Password == currentPassword)
                {
                    Console.WriteLine("Password confirmed");
                }
                else
                {
                    Console.WriteLine("Wrong password!");
                    EditPassword();
                    return;
                }

            }
            
            Console.WriteLine("Write a new password: ");
            var newPassword = HidePassword();

            var isValidPassword = ValidatePassword(newPassword);

            if (!isValidPassword || newPassword.Contains("\u0000") || newPassword.Length < 8)
            {
                Console.WriteLine("Invalid password! ");
                Console.WriteLine("Password needs to be 8 characters long.");
                Console.WriteLine("Include a specialcharacter.");
                Console.WriteLine("Include at least one uppercase character.");
                Console.WriteLine("Include at least one lowercase character.");
                Console.WriteLine("Include at least one number.\n");
                EditPassword();
                return;
            }

            if (MenuManager.UserPosition < 0)
            {
                var loggedInUser = Users[MenuManager.LoggedInUserPosition];
                loggedInUser.Password = newPassword;
            }
            if (MenuManager.UserPosition >= 0)
            {
                var selectedUser = Users[MenuManager.UserPosition];
                selectedUser.Password = newPassword;

            }

            FileManager.UpdateJson(_path, Users);
        }

    }

}