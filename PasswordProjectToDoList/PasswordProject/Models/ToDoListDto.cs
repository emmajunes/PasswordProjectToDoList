namespace PasswordProjectToDoList.Models
{
    public class ToDoListDto
    {
        public Guid ListId { get; set; }
        public string ListDateTime { get; set; }        
        public string ListTitle { get; set; }
        public List<TaskDto> Tasks { get; set; }
        public string TitleColor { get; set; }
        public Guid UserId { get; set; }
    }
}