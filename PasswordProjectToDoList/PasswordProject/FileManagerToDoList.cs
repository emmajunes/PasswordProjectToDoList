using PasswordProjectToDoList.Models;
using System.Text.Json;

namespace PasswordProject
{
    public class FileManagerToDoList
    {
        private static readonly string _currentDir = Environment.CurrentDirectory;
        private static readonly string _path = Directory.GetParent(_currentDir).Parent.Parent.FullName + @"\ToDoList.json";

        public void CreateJson()
        {
            if (!File.Exists(_path))
            {
                using (var fs = File.Create(_path)) { }

                File.WriteAllText(_path, "[]");
            }
        }

        public static List<ToDoListDto> GetJson()
        {
            var jsonData = File.ReadAllText(_path);

            var lists = JsonSerializer.Deserialize<List<ToDoListDto>>(jsonData);

            return lists;
        }

        public static List<ToDoListDto> GetCurrentLoggedInUsersLists()
        {
            var lists = GetJson();

            if (!UserManager.LoggedInUser.IsAdmin())
            {
                return lists.Where(x => x.UserId == UserManager.LoggedInUser.UserId).ToList();
            }

            return lists;
        }

        public static void UpdateJson(List<ToDoListDto> lists)
        {
            var jsonData = JsonSerializer.Serialize(lists, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(_path, jsonData);
        }

        public static void UpdateListJson(List<ToDoListDto> lists)
        {
            var allLists = GetJson();
            allLists.RemoveAll(x => x.UserId == UserManager.LoggedInUser.UserId);
            var union = allLists.Union(lists).ToList();

            var jsonData = JsonSerializer.Serialize(union, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(_path, jsonData);
        }

        public static void UpdateTaskJson(List<ToDoListDto> lists)
        {
            var allLists = GetJson();

            allLists.First(i => i.ListId == lists[0].ListId).Tasks = lists[0].Tasks;

            var jsonData = JsonSerializer.Serialize(allLists, new JsonSerializerOptions() { WriteIndented = true });

            File.WriteAllText(_path, jsonData);
        }
    }
}