﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskGroupsSample.cs" company="">
//   
// </copyright>
// <summary>
//   The task groups sample.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.TeamServices.Samples.Client.TaskGroups
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.TeamFoundation.DistributedTask.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// The task groups sample.
    /// </summary>
    [ClientSample(TaskResourceIds.AreaName, TaskResourceIds.TaskGroupsResource)]
    public class TaskGroupsSample : ClientSample
    {
        /// <summary>
        /// The added task group id.
        /// </summary>
        private Guid addedTaskGroupId;

        /// <summary>
        /// The add a task group.
        /// </summary>
        /// <returns>
        /// The <see cref="TaskGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public TaskGroup CreateTaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            Dictionary<string, string> inputs = new Dictionary<string, string>()
                                                    {
                                                        { "scriptType", "inlineScript" },
                                                        { "inlineScript", "Write-Host \"Hello World\"" },
                                                    };
            // Task inside the task group
            TaskGroupStep taskGroupStep = new TaskGroupStep()
            {
                Enabled = true,
                DisplayName = "PowerShell Script",
                Inputs = inputs,
                Task = new TaskDefinitionReference { Id = new Guid("e213ff0f-5d5c-4791-802d-52ea3e7be1f1"), VersionSpec = "1.*", DefinitionType = "task" },
            };

            // Task group object
            TaskGroup taskGroup = new TaskGroup()
            {
                Tasks = new List<TaskGroupStep>() { taskGroupStep },
                Category = "Deploy",
                Name = "PowerShell TG1",
                Visibility = { "Build", "Release" },
                InstanceNameFormat = "Task group: TG",
                Version = new TaskVersion { IsTest = false, Major = 1, Minor = 0, Patch = 0 }
            };

            List<TaskGroup> taskGroups = taskClient.GetTaskGroupsAsync(project: projectName).Result;

            // Create task group
            var addedTg = taskClient.AddTaskGroupAsync(project: projectName, taskGroup: taskGroup).Result;

            this.addedTaskGroupId = addedTg.Id;

            // Show the added task group
            Console.WriteLine("{0} {1}", addedTg.Id.ToString().PadLeft(6), addedTg.Name);

            return addedTg;
        }

        /// <summary>
        /// The update task group.
        /// </summary>
        /// <returns>
        /// The <see cref="TaskGroup"/>.
        /// </returns>
        [ClientSampleMethod]
        public TaskGroup UpdateTaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // Get task group to update
            List<TaskGroup> taskGroups = taskClient.GetTaskGroupsAsync(project: projectName, taskGroupId: this.addedTaskGroupId).Result;

            Dictionary<string, string> inputs = new Dictionary<string, string>()
                                                    {
                                                        { "scriptType", "inlineScript" },
                                                        { "inlineScript", "Write-Host \"New task\"" },
                                                    };
            // New Task to be added in the task group
            TaskGroupStep newTask = new TaskGroupStep()
                                              {
                                                  Enabled = true,
                                                  DisplayName = "PowerShell Script",
                                                  Inputs = inputs,
                                                  Task = new TaskDefinitionReference { Id = new Guid("e213ff0f-5d5c-4791-802d-52ea3e7be1f1"), VersionSpec = "1.*", DefinitionType = "task" },
                                              };

            // Update comment in the task group object.
            TaskGroup taskGroup = taskGroups.FirstOrDefault();
            taskGroup.Comment = "Updated the task group";
            taskGroup.Tasks.Add(newTask);
            TaskGroup updatedTaskGroup = taskClient.UpdateTaskGroupAsync(project: projectName, taskGroup: taskGroup).Result;

            Console.WriteLine("{0} {1}", updatedTaskGroup.Id.ToString().PadLeft(6), updatedTaskGroup.Comment);

            return updatedTaskGroup;
        }

        /// <summary>
        /// List all versions of a task group.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [ClientSampleMethod]
        public List<TaskGroup> AllVersionsOfTaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // List all task groups
            List<TaskGroup> taskGroups = taskClient.GetTaskGroupsAsync(project: projectName, taskGroupId: addedTaskGroupId).Result;

            foreach (TaskGroup taskGroup in taskGroups)
            {
                Console.WriteLine("{0} {1}", taskGroup.Id.ToString().PadLeft(6), taskGroup.Name);
            }

            return taskGroups;
        }

        /// <summary>
        /// List all task groups with all versions.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [ClientSampleMethod]
        public List<TaskGroup> AllTaskGroups()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // List all task groups
            List<TaskGroup> taskGroups = taskClient.GetTaskGroupsAsync(project: projectName).Result;

            foreach (TaskGroup taskGroup in taskGroups)
            {
                Console.WriteLine("{0} {1}", taskGroup.Id.ToString().PadLeft(6), taskGroup.Name);
            }

            return taskGroups;
        }
        /// <summary>
        /// The delete a task group.
        /// </summary>
        [ClientSampleMethod]
        public void DeleteATaskGroup()
        {
            string projectName = ClientSampleHelpers.FindAnyProject(this.Context).Name;

            // Get a task agent client instance
            VssConnection connection = Context.Connection;
            TaskAgentHttpClient taskClient = connection.GetClient<TaskAgentHttpClient>();

            // Delete the already created task group
            taskClient.DeleteTaskGroupAsync(project: projectName, taskGroupId: this.addedTaskGroupId).SyncResult();
        }
    }
}