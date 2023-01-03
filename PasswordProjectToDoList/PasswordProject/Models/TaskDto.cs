namespace PasswordProjectToDoList.Models
{
    public class TaskDto
    {
        public bool Completed { get; set; }
        public string TaskDescription { get; set; }
        public string TaskPrio { get; set; }
        public string TaskTitle { get; set; }
    }
}