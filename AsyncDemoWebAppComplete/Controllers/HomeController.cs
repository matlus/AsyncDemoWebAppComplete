using AsyncDemoWebAppComplete.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AsyncDemoWebAppComplete.Controllers
{
    public class HomeController : Controller
    {
        string[] sources =
        {
            "https://matlusmovies.azurewebsites.net/api/movies/category/action",
            "https://matlusmovies.azurewebsites.net/api/movies/category/drama",
            "https://matlusmovies.azurewebsites.net/api/movies/category/thriller",
            "https://matlusmovies.azurewebsites.net/api/movies/category/sci-fi",
        };

        public ActionResult Sync()
        {
            var sw = Stopwatch.StartNew();

            var data = DownloadData();

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }

        public async Task<ActionResult> ASync()
        {
            var sw = Stopwatch.StartNew();

            var data = await DownloadDataAsync();

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }

        public ActionResult SyncP()
        {
            var sw = Stopwatch.StartNew();

            var data = DownloadDataParallel();

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }

        private IEnumerable<IEnumerable<Movie>> DownloadData()
        {
            var allMovies = new List<IEnumerable<Movie>>();

            foreach (var url in sources)
            {
                allMovies.Add(DownloadMovies(url));
            }

            return allMovies;
        }

        private async Task<IEnumerable<IEnumerable<Movie>>> DownloadDataAsync()
        {
            var allMoviesTasks = new List<Task<IEnumerable<Movie>>>();

            foreach (var url in sources)
            {
                allMoviesTasks.Add(DownloadMoviesAsync(url));
            }

            return await Task.WhenAll(allMoviesTasks);
        }

        private object DownloadDataParallel()
        {
            var allMovies = new List<IEnumerable<Movie>>();

            Parallel.ForEach(sources, url =>
            {
                allMovies.Add(DownloadMovies(url));
            });

            return allMovies;
        }

        private IEnumerable<Movie> DownloadMovies(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResponseMessage = httpClient.GetAsync(url).Result;
                httpResponseMessage.EnsureSuccessStatusCode();
                return httpResponseMessage.Content.ReadAsAsync<IEnumerable<Movie>>().Result;
            }
        }

        private async Task<IEnumerable<Movie>> DownloadMoviesAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var httpResponseMessage = await httpClient.GetAsync(url);
                httpResponseMessage.EnsureSuccessStatusCode();
                return await httpResponseMessage.Content.ReadAsAsync<IEnumerable<Movie>>();
            }
        }

        private async Task<IEnumerable<Movie>> GetMoviesAsync(string url)
        {
            var genreAsString = url.Substring(url.LastIndexOf('/') + 1);
            var movies = new List<Movie>();

            SqlConnection sqlConnection = new SqlConnection(@"Data Source = .\SQL2014ENT; Initial Catalog = Movies; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False");
            SqlCommand sqlCommand = null;
            SqlDataReader sqlDataReader = null;

            try
            {
                await sqlConnection.OpenAsync();
                sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "GetMoviesByGenre";
                var sqlParameter = sqlCommand.Parameters.Add("@Genre", SqlDbType.VarChar)
                sqlParameter.Value = genreAsString;
                sqlDataReader = await sqlCommand.ExecuteReaderAsync();
                while (sqlDataReader.Read())
                {
                    movies.Add(new Movie()
                    {
                        Title = (string)sqlDataReader["Title"],
                        Year = (int)sqlDataReader["Year"],
                        Category = (string)sqlDataReader["Genre"],
                        ImageUrl = (string)sqlDataReader["ImageUrl"]
                    });
                }
                return movies;
            }
            finally
            {
                if (sqlDataReader != null)
                    sqlDataReader.Close();

                if (sqlCommand != null)
                    sqlCommand.Dispose();

                if (sqlConnection != null)
                    sqlConnection.Dispose();
            }
        }
    }
}