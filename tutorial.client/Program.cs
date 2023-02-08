using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using tutorial.client;

namespace tutorialClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7200");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                while (true)
                {
                    Console.WriteLine("Select an Operation:\n1-Get an item by ID\n2-Put an item on the query\n" +
                        "3-Update an item by ID\n4-Delete an item by ID\n5-Get ALL items\n6-EXIT");
                    Console.Write("Your  choice: ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            await GetItem(client);
                            continue;
                        case "2":
                            await PutItem(client);
                            continue;
                        case "3":
                            await UpdateItem(client);
                            continue;
                        case "4":
                            await DelItem(client,null);
                            continue;
                        case "5":
                            await GetAll(client);
                            continue;
                        case "6":
                            break;
                        default: 
                            Console.WriteLine("Enter a correct input!!");
                            continue;
                    }

                    break;
                }
            }            
            Console.WriteLine("Exitting.....");
        }

        static async Task GetItem(HttpClient client)
        {
            Console.WriteLine("Current Operation --> GET");

            while(true)
            {
                Console.Write("Enter the ID (Enter 0 to Exit): ");

                if (Int32.TryParse(Console.ReadLine(), out int id))
                {
                    if (id == 0)
                    {
                        break;
                    }
                    else
                    {
                        string s1 = string.Format("api/issue/id?id={0}", id);
                        HttpResponseMessage response = await client.GetAsync(s1);
                        try
                        {
                            response.EnsureSuccessStatusCode();
                        }
                        catch (HttpRequestException)
                        {
                            Console.WriteLine("No results!!!");
                        }


                        if (response.IsSuccessStatusCode)
                        {
                            var issue = await response.Content.ReadFromJsonAsync<IssueDto>();
                            Console.WriteLine(issue.Id);
                            Console.WriteLine(issue.Name);
                            Console.WriteLine(issue.Priority);
                            Console.WriteLine(issue.IssueType);
                            Console.WriteLine(issue.Created);
                            Console.WriteLine(issue.Completed);
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Enter a valid ID value!!");
                }
            }       
        }

        static async Task GetAll(HttpClient client)
        {
            Console.WriteLine("Current Operation --> GET ALL");
            HttpResponseMessage response = await client.GetAsync("api/issue");
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var issues = await response.Content.ReadFromJsonAsync<IEnumerable<IssueDto>>();

                foreach(var issue in issues)
                {
                    Console.WriteLine(issue.Id);
                    Console.WriteLine(issue.Name);
                    Console.WriteLine(issue.Priority);
                    Console.WriteLine(issue.IssueType);
                    Console.WriteLine(issue.Created);
                    Console.WriteLine(issue.Completed);
                    Console.WriteLine("---------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("No results!!!");
            }
        }

        static async Task PutItem(HttpClient client)
        {
            HttpResponseMessage response;

            Console.WriteLine("Current Operation --> PUT");

            IssueDto newIssue = new IssueDto();

            ///Read Properties
             Console.Write("Enter Name: ");
            newIssue.Name = Console.ReadLine();

            while (true)
            {
                Console.Write("Selected Priority ([0] - Low, [1] - Medium, [2] - High): ");
                switch (Console.ReadLine())
                {
                    case "0":
                        newIssue.Priority = Priority.Low;
                        break;
                    case "1":
                        newIssue.Priority = Priority.Medium;
                        break;
                    case "2":
                        newIssue.Priority = Priority.High;
                        break;
                    default:
                        Console.WriteLine("Wrong input, try again!!");
                        continue;
                }

                break;
            }

            while (true)
            {
                Console.Write("Selected Issue Type ([0] - Feature, [1] - Bug, [2] - Documentation): ");
                switch (Console.ReadLine())
                {
                    case "0":
                        newIssue.IssueType = IssueType.Feature;
                        break;
                    case "1":
                        newIssue.IssueType = IssueType.Bug;
                        break;
                    case "2":
                        newIssue.IssueType = IssueType.Documentation;
                        break;
                    default:
                        Console.WriteLine("Wrong input, try again!!");
                        continue;
                }

                break;
            }

            newIssue.Created = DateTime.Now;
            newIssue.Completed = DateTime.Now;

            
            response = await client.PostAsJsonAsync("api/issue", newIssue);            

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.ToString());
            }
        }

        static async Task UpdateItem(HttpClient client)
        {
            Console.WriteLine("Current Operation --> UPDATE");
            Console.WriteLine("\n\nIMPORTANT  NOTE --- Leave the fields empty for the unchanged properties.\n\n");

            while (true)
            {
                Console.Write("Enter the ID: ");

                if (Int32.TryParse(Console.ReadLine(), out int id))
                {
                    string s1 = string.Format("api/issue/id?id={0}", id);
                    HttpResponseMessage response;
                    response = await client.GetAsync(s1);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException)
                    {
                        Console.WriteLine("No results!!!");
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var issueOld = await response.Content.ReadFromJsonAsync<IssueDto>();
                        Console.WriteLine(response.ToString());
                        Console.Write("Enter Name (old name: " + issueOld.Name + "): ");
                        string? new_name = Console.ReadLine();

                        if (!string.IsNullOrEmpty(new_name))
                        {
                            issueOld.Name = new_name;
                        }

                        while (true)
                        {
                            Console.Write("Selected Priority ([0] - Low, [1] - Medium, [2] - High) (old priority: " + issueOld.Priority + "): ");
                            switch (Console.ReadLine())
                            {
                                case "0":
                                    issueOld.Priority = Priority.Low;
                                    break;
                                case "1":
                                    issueOld.Priority = Priority.Medium;
                                    break;
                                case "2":
                                    issueOld.Priority = Priority.High;
                                    break;
                                case "":
                                    break;
                                default:
                                    Console.WriteLine("Wrong input, try again!!");
                                    continue;
                            }

                            break;
                        }

                        while (true)
                        {
                            Console.Write("Selected Issue Type ([0] - Feature, [1] - Bug, [2] - Documentation) (old Issue Type: " + 
                                issueOld.IssueType + "): ");
                            switch (Console.ReadLine())
                            {
                                case "0":
                                    issueOld.IssueType = IssueType.Feature;
                                    break;
                                case "1":
                                    issueOld.IssueType = IssueType.Bug;
                                    break;
                                case "2":
                                    issueOld.IssueType = IssueType.Documentation;
                                    break;
                                case "":
                                    break;
                                default:
                                    Console.WriteLine("Wrong input, try again!!");
                                    continue;
                            }
                            break;
                        }
                        issueOld.Completed = DateTime.Now;

                        ///await DelItem(client, id);
                        ///
                        response = await client.PutAsJsonAsync(s1, issueOld);
                        Console.WriteLine(response.ToString());
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine(response.ToString());
                        }
                        break;
                    }
                    else
                    {
                        Console.WriteLine("No results!!!");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Enter a valid ID value!!");
                }
            }
        }

        static async Task DelItem(HttpClient client, int? opt)
        {
            bool flag = true && opt ==null;

            if (flag)
            {
                Console.WriteLine("Current Operation --> DELETE");
                Console.WriteLine("\n\nIMPORTANT  NOTE --- This operation cannot be reversed.\n\n");
                while (true)
                {
                    Console.Write("Enter the ID: ");

                    if (Int32.TryParse(Console.ReadLine(), out int id))
                    {
                        string s1 = string.Format("api/issue/{0}", id);
                        HttpResponseMessage response = await client.DeleteAsync(s1);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine(response.ToString);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Check ID number!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter a valid ID value!!");
                    }
                }
            }
            else
            {
                string s1 = string.Format("api/issue/{0}", opt);
                HttpResponseMessage response = await client.DeleteAsync(s1);
            }
        }
    }

}

