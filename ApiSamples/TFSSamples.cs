using EasyProjects.Client;
using EasyProjects.ClientModel.Entities;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Linq;
using Project = Microsoft.TeamFoundation.WorkItemTracking.Client.Project;

namespace ApiSamples
{

    class TFSSamples
    {
        class TaskData
        {
            public string Name;
        }

        internal static void RunSamples(HttpClient client)
        {
            Uri collectionUri = new Uri("http://tfshost:8080/tfs/DefaultCollection/");
            TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(collectionUri);
            WorkItemStore workItemStore = tpc.GetService<WorkItemStore>();

            TaskData sampleDataFromTFS = GetSomeWorkItemFromTFS(workItemStore);
            CreateTaskInEP(client, sampleDataFromTFS);

            TaskData sampleDataFromEP = GetSomeTaskFromEP(client);
            CreateWorkItemInTFS(workItemStore, sampleDataFromEP);
        }

        private static void CreateWorkItemInTFS(WorkItemStore workItemStore, TaskData taskData)
        {
            Project teamProject = workItemStore.Projects["TestProject"];
            WorkItemType taskItemType = teamProject.WorkItemTypes["Task"];

            WorkItem newTask = new WorkItem(taskItemType)
            {
                Title = taskData.Name
            };

            newTask.Save();
        }

        private static TaskData GetSomeTaskFromEP(HttpClient client)
        {
            var tasks = client.Query<Task>().Take(1).ToArray();

            return new TaskData { Name = tasks[0].Name };
        }

        private static TaskData GetSomeWorkItemFromTFS(WorkItemStore workItemStore)
        {
            WorkItemCollection queryResults = workItemStore.Query("SELECT [Title] FROM WorkItems");

            return new TaskData { Name = queryResults[0].Title };
        }

        private static void CreateTaskInEP(HttpClient client, TaskData sampleDataFromTFS)
        {
            // Supported TaskTypes are: 1 - Task, 2 - Issue, 3 - Request
            var task_1 = client.Post<Task>(
                new Task
                {
                    Name = sampleDataFromTFS.Name,
                    ProjectID = 1, // replace with real project ID
                    TaskTypeID = 1
                });
        }
    }
}
