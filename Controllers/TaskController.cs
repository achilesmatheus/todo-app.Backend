﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Models;
using Todo.Repositories;
using Todo.ViewModels;

namespace Todo.Controllers;

[ApiController]
[Route("v1")]
public class TaskController : ControllerBase
{
    [HttpGet("tasks")]
    public async Task<IActionResult> GetAll(
        [FromServices] TaskRepository repository
    )
    {
        try
        {
            var tasks = await repository.GetAllAsync();
            var result = new ResultViewModel<IEnumerable<TaskModel>>(tasks);
            return StatusCode(200, result);

        }
        catch
        {
            var result = new ResultViewModel<string>("Could not fetch task list");
            return StatusCode(500, result);
        }
    }

    [HttpGet("task/{id:int}")]
    public async Task<IActionResult> GetById(
        [FromServices] TaskRepository repository,
        [FromRoute] int id
    )
    {
        try
        {
            var task = await repository.GetByIdAsync(id);
            if (task == null) throw new DbUpdateException("The task was not found");

            var result = new ResultViewModel<TaskModel>(task);
            return StatusCode(200, result);
        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(500, result);
        }
        catch
        {
            var result = new ResultViewModel<string>("Internal server error");
            return StatusCode(500, result);
        }
    }

    [HttpGet("tasks/{date:datetime}")]
    public async Task<IActionResult> GetByPeriod(
        [FromServices] TaskRepository repository,
        [FromRoute] DateTime date
    )
    {
        try
        {
            var tasks = await repository.GetByPeriodAsync(date);
            if (tasks == null) throw new DbUpdateException("The task was not found");

            var result = new ResultViewModel<IEnumerable<TaskModel>>(tasks);
            return StatusCode(200, result);
        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(404, result);
        }
        catch
        {
            var result = new ResultViewModel<string>("The specified items could not be found");
            return StatusCode(500, result);
        }

    }

    [HttpPost("task")]
    public async Task<IActionResult> CreateTask(
        [FromServices] TaskRepository taskRepository,
        [FromBody] TaskViewModel model
    )
    {
        try
        {
            var task = new TaskModel
            {
                Title = model.Title,
                Description = model.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            await taskRepository.CreateAsync(task);

            var result = new ResultViewModel<TaskModel>(task);
            return StatusCode(201, result);

        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(404, result);

        }
        catch
        {
            var result = new ResultViewModel<string>("Internal server error");
            return StatusCode(500, result);

        }
    }

    [HttpPut("task/{id:int}")]
    public async Task<IActionResult> UpdateTask(
        [FromServices] TaskRepository taskRepository,
        [FromBody] TaskViewModel viewModel,
        [FromRoute] int id
    )
    {
        try
        {
            var task = await taskRepository.GetByIdAsync(id);
            if (task == null) throw new DbUpdateException($"The task with id {id} was not found");

            task.Title = viewModel.Title;
            task.Description = viewModel.Description;
            task.UpdatedAt = DateTime.Now;

            await taskRepository.UpdateAsync(task);

            var result = new ResultViewModel<TaskModel>(task);
            return StatusCode(200, result);
        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(404, result);
        }
        catch
        {
            var result = new ResultViewModel<string>("Internal server error");
            return StatusCode(500, result);
        }
    }

    [HttpPut("task/{id:int}/mark-as-done")]
    public async Task<IActionResult> MarkAsDone(
        [FromServices] TaskRepository taskRepository,
        [FromRoute] int id
    )
    {
        try
        {
            var task = await taskRepository.GetByIdAsync(id);
            if (task == null) throw new DbUpdateException($"The task with id {id} was not found");

            task.Done = true;
            task.UpdatedAt = DateTime.Now;
            await taskRepository.UpdateAsync(task);

            var result = new ResultViewModel<TaskModel>(task);
            return StatusCode(200, result);
        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(404, result);
        }
        catch
        {
            var result = new ResultViewModel<string>("Internal server error");
            return StatusCode(500, result);
        }
    }

    [HttpPut("task/{id:int}/mark-as-undone")]
    public async Task<IActionResult> MarkAsUnDone(
        [FromServices] TaskRepository taskRepository,
        [FromRoute] int id
    )
    {
        try
        {
            var task = await taskRepository.GetByIdAsync(id);
            if (task == null) throw new DbUpdateException($"The task with id {id} was not found");

            task.Done = false;
            task.UpdatedAt = DateTime.Now;
            await taskRepository.UpdateAsync(task);

            var result = new ResultViewModel<TaskModel>(task);
            return StatusCode(200, result);
        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(404, result);
        }
        catch
        {
            var result = new ResultViewModel<string>("Internal server error");
            return StatusCode(500, result);
        }
    }

    [HttpPut("task/{taskId:int}/list/{listId:int}")]
    public async Task<IActionResult> MoveTaskToList(
        [FromServices] TaskRepository taskRepository,
        [FromServices] ListRepository listRepository,
        [FromRoute] int taskId,
        [FromRoute] int listId

    )
    {
        try
        {
            var task = await taskRepository.GetByIdAsync(taskId);
            var list = await listRepository.GetByIdAsync(listId);

            if (task == null) throw new DbUpdateException($"Task with id '{listId}' could not be found");
            if (list == null) throw new DbUpdateException($"List with id '{taskId}' could not be found");

            list.AddTask(task);
            await taskRepository.UpdateAsync(task);

            var result = new ResultViewModel<TaskModel>(task);
            return StatusCode(200, result);
        }
        catch (DbUpdateException ex)
        {
            var result = new ResultViewModel<string>(ex.Message);
            return StatusCode(404, result);
        }
        catch
        {
            var result = new ResultViewModel<string>("Internal server error");
            return StatusCode(500, result);
        }
    }
}