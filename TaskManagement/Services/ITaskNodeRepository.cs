using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    public interface ITaskNodeRepository
    {
        /// <summary>
        /// добавляет задачу
        /// </summary>
        /// <param name="taskNode">задача которую нужно добавить</param>
        /// <returns></returns>
        Task AddTaskAsync(TaskNode taskNode);

        /// <summary>
        /// сохраняет любые изменения
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();

        /// <summary>
        /// возвращает список всех объектов из бд
        /// </summary>
        /// <returns></returns>
        Task<List<TaskNode>> LoadAsync();

        /// <summary>
        /// удаляет задачу вместе со всеми её вложениями
        /// </summary>
        /// <param name="id">id удаляемой задачи</param>
        /// <returns>true если база данных содержала задачу и она была успешно удалена вместе со всеми подзадачами</returns>
        Task<bool> Remove(int id);

        /// <summary>
        /// получить конкретный объект
        /// </summary>
        /// <param name="id">id объекта</param>
        /// <returns>возвращает объект по его id</returns>
        Task<TaskNode> FindById(int id);
    }
}
