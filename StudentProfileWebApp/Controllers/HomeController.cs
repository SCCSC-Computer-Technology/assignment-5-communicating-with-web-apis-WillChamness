using Microsoft.AspNetCore.Mvc;
using StudentProfileWebApp.Models;
using System.Diagnostics;
using System.Text.Json;

namespace StudentProfileWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory clientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            clientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// GETs student profiles from the API
        /// 
        /// Can either get by ID or get all student profiles
        /// </summary>
        /// <param name="getStudentId">The ID of the student to find</param>
        /// <returns>If there are no errors, a task that resolves to a view of an enumerable of student profiles. Otherwise, an empty view.</returns>
        public async Task<IActionResult> GetStudentProfiles(string? getStudentId)
        {
            IEnumerable<StudentProfile>? model; // view expects IEnumerable<StudentProfile>
            string uri;

            // allow only null and positive long values. default to empty results if criteria is not met
            try
            {
                if (getStudentId is not null) 
                { 
                    if(long.Parse(getStudentId) <= 0) // no need to store parsed value
                    {
                        throw new ArgumentException();
                    }
                }
            }
            catch(FormatException) { return View(); }
            catch(ArgumentException) { return View(); }
            catch(OverflowException) { return View(); }

            if (getStudentId is null)
            {
                ViewData["Title"] = "All Students";
                uri = "api/StudentProfiles";
            }
            else
            {
                ViewData["Title"] = "Student with ID " + getStudentId;
                uri = "api/StudentProfiles/" + getStudentId; 
            }

            HttpClient httpClient = clientFactory.CreateClient(name: "StudentProfilesWebApi");

            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = await httpClient.SendAsync(request);

            // guard clause for bad student ID
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound) { return View(); }

            if(getStudentId is null)
            {
                model = await response.Content.ReadFromJsonAsync<IEnumerable<StudentProfile>>();
            }
            else // json result is object, not array. must parse json and add it to a list of length 1 to satisfy type definition of models
            {
                string json = await response.Content.ReadAsStringAsync();
                StudentProfile? studentProfile = JsonSerializer.Deserialize<StudentProfile>(json, 
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                model = studentProfile is null  
                    ? new List<StudentProfile>() 
                    : (new List<StudentProfile>()).Append(studentProfile);
            }

            return View(model);
        }

        /// <summary>
        /// POSTs a new student profile to the API
        /// </summary>
        /// <param name="postName">The name of the new student</param>
        /// <param name="postGpa">The GPA of the new student</param>
        /// <param name="postEmail">The email address of the new student</param>
        /// <param name="postMajor">The major of the new student</param>
        /// <param name="postPhone">The phone number of the new student</param>
        /// <param name="postUsername">The username of the new student</param>
        /// <param name="postPassword">The password of the new student</param>
        /// <returns>If there are no errors, a task that resolves to a view of the new student profile. Otherwise, an empty view.</returns>
        public async Task<IActionResult> PostStudentProfile(string postName, string postGpa, string postEmail, string postMajor, string postPhone, string postUsername, string postPassword)
        {
            string uri = "api/StudentProfiles";
            HttpClient httpClient = clientFactory.CreateClient(name: "StudentProfilesWebApi");

            try
            {
                double parsedGpa = double.Parse(postGpa);
                if(parsedGpa < 0 || parsedGpa > 4)
                {
                    ViewData["Title"] = "Bad GPA";
                    return View();
                }

                var studentProfile = new StudentProfile();
                studentProfile.Id = 0; // ignored by api, can put anything here
                studentProfile.Name = postName;
                studentProfile.GPA = parsedGpa;
                studentProfile.StudentEmail = postEmail;
                studentProfile.Major = postMajor;
                studentProfile.PhoneNumber = postPhone;
                studentProfile.StudentUserName = postUsername;
                studentProfile.password = postPassword;

                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(uri, studentProfile);
                if(response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    ViewData["Title"] = "Success!";
                    return View(await response.Content.ReadFromJsonAsync<StudentProfile>());
                }
                else
                {
                    ViewData["Title"] = (int)response.StatusCode + " " + response.ReasonPhrase;
                    return View();
                }
            }
            catch(FormatException) {
                ViewData["Title"] = "Bad GPA";
                return View(); 
            }
        }

        /// <summary>
        /// Replaces an existing student profile
        /// </summary>
        /// <param name="putId">The student's ID</param>
        /// <param name="putName">The student's name</param>
        /// <param name="putGpa">The student's GPA</param>
        /// <param name="putEmail">The student's email address</param>
        /// <param name="putMajor">The student's major</param>
        /// <param name="putPhone">The student's phone number</param>
        /// <param name="putUsername">The student's username</param>
        /// <param name="putPassword">The student's password</param>
        /// <returns>If there are no errors, a task that resolves to a view of the new student profile. Otherwise, an empty view.</returns>
        public async Task<IActionResult> PutStudentProfile(string putId, string putName, string putGpa, string putEmail, string putMajor, string putPhone, string putUsername, string putPassword)
        {
            // get the ID and GPA
            long parsedId;
            double parsedGpa;
            try
            {
                parsedId = long.Parse(putId);
                parsedGpa = double.Parse(putGpa);
                if(parsedId <= 0 || parsedGpa < 0 || parsedGpa > 4) { throw new ArgumentException(); }
            }
            catch(FormatException) 
            {
                ViewData["Title"] = "Bad ID or GPA";
                return View(); 
            }
            catch(ArgumentException) 
            { 
                ViewData["Title"] = "Bad ID or GPA";
                return View(); 
            }
            catch(OverflowException) 
            { 
                ViewData["Title"] = "Bad ID or GPA";
                return View(); 
            }

            // create new student profile and send to API
            var studentProfile = new StudentProfile();
            studentProfile.Id = parsedId; 
            studentProfile.Name = putName;
            studentProfile.GPA = parsedGpa;
            studentProfile.StudentEmail = putEmail;
            studentProfile.Major = putMajor;
            studentProfile.PhoneNumber = putPhone;
            studentProfile.StudentUserName = putUsername;
            studentProfile.password = putPassword;

            string uri = "api/StudentProfiles/" + putId;
            HttpClient httpClient = clientFactory.CreateClient(name: "StudentProfilesWebApi");

            HttpResponseMessage response = await httpClient.PutAsJsonAsync(uri, studentProfile);

            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                ViewData["Title"] = "Success!";
                return View(studentProfile);
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewData["Title"] = "Could not find student with ID " + parsedId + ". Please try again.";
                return View();
            }
            else
            {
                ViewData["Title"] = (int)response.StatusCode + " " + response.ReasonPhrase;
                return View();
            }
        }

        /// <summary>
        /// DELETEs a student profile.
        /// </summary>
        /// <param name="deleteStudentId">The ID of the student</param>
        /// <returns>A task that resolves to an empty view regardless of errors.</returns>
        public async Task<IActionResult> DeleteStudentProfile(string? deleteStudentId)
        {
            if(deleteStudentId is null)
            {
                ViewData["Title"] = "Student ID field cannot be empty";
                return View();
            }

            // allow positive values only
            try
            {
                if(long.Parse(deleteStudentId) <= 0) // no need to store parsed value
                {
                    throw new ArgumentException();
                }
            }
            catch(FormatException)
            {
                ViewData["Title"] = "Bad ID";
                return View(); 
            }
            catch(ArgumentException) 
            {
                ViewData["Title"] = "Bad ID";
                return View(); 
            }
            catch(OverflowException) 
            {
                ViewData["Title"] = "Bad ID";
                return View(); 
            }

            string uri = "api/StudentProfiles/" + deleteStudentId;

            HttpClient httpClient = clientFactory.CreateClient(name: "StudentProfilesWebApi");

            HttpResponseMessage response = await httpClient.DeleteAsync(uri);
            
            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                ViewData["Title"] = "Success! Student deleted permanantly";
                return View();
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewData["Title"] = "Could not find student with ID " + deleteStudentId + ". Please try again.";
                return View();
            }
            else
            {
                ViewData["Title"] = "Error: " + (int)response.StatusCode + " " + response.ReasonPhrase;
                return View();
            }
        } 

    }
}
