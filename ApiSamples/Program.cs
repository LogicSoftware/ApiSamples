using EasyProjects.Client;
using EasyProjects.ClientModel.Entities;
using System;
using System.Linq;

namespace ApiSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient(new Uri("http://{youraccount}.go.easyprojects.net/rest/v1/"), 
                "{username}", "{password}");

            var user = client.Query<User>()
                    .Where(u => u.Login == "{username}") // Replace with real login
                    .ToList().Single();

            // create new project project
            var p = client.Post<Project>(new Project { Name = "Project" });

            // add project memeber
            var member = client.Post<ProjectMember>(
                string.Format("projects/{0}/members", p.ProjectID), 
                new ProjectMember 
                {
                    ProjectID = p.ProjectID, 
                    UserID = user.UserID 
                });

            // get all ProjectMembers for Project
            var members = client.QueryAsResource<ProjectMember>(
                string.Format("projects/{0}/members", p.ProjectID), 
                p.ProjectID.ToString()
                ).ToList();

            var memberToAssign = members.First();

            // Create Task
            // Supported TaskTypes are: 1 - Task, 2 - Issue, 3 - Request
            var task_1 = client.Post<Task>(
                new Task 
                {
                    Name = "Task 1", 
                    ProjectID = p.ProjectID, 
                    TaskTypeID = 1 
                });

            // assign Task 1 to a project member
            var assignee1 = client.Post<TaskAssignee>(
                string.Format("activities/{0}/assignees", task_1.TaskID),
                new TaskAssignee { ProjectMemberID = memberToAssign.ProjectMemberID }
                );

            //create subtask for Task 1
            var subtask = client.Post<Task>(
                new Task 
                {
                    Name = "Task 11", 
                    ProjectID = p.ProjectID, 
                    ParentID = task_1.TaskID, 
                    TaskTypeID = 1 
                });

            // specify TaskID for add TaskMessage, ProjectId for add ProjectMessage
            var message = client.Post<Message>(
                new Message 
                { 
                    TaskID = task_1.TaskID, 
                    UserID = 1, 
                    PostDate = DateTime.Now, 
                    MessageText = "Message Text"
                });
        }
    }
}
