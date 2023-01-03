namespace PasswordProject
{
    internal class Program
    {

        static void Main(string[] args)
        {

            FileManager.CreateJson(ApplicationConstants.UserPath);

            var jsonFile = new FileManagerToDoList();
            jsonFile.CreateJson();

            var menuManager = new MenuManager();
            menuManager.StartMenu();

        }
    }
}