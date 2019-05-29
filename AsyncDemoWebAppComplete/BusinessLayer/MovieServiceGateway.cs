using AsyncDemoWebAppComplete.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AsyncDemoWebAppComplete
{
    public static class DataProvider
    {
        private static readonly HttpClient s_httpClient = new HttpClient();
        private static readonly string[] sources =
        {
            "https://matlusstorage.blob.core.windows.net/membervideos/action.json",
            "https://matlusstorage.blob.core.windows.net/membervideos/drama.json",
            "https://matlusstorage.blob.core.windows.net/membervideos/thriller.json",
            "https://matlusstorage.blob.core.windows.net/membervideos/scifi.json",
        };

        public static IEnumerable<IEnumerable<Movie>> DownloadData()
        {
            var allMovies = new List<IEnumerable<Movie>>();

            foreach (var url in sources)
            {
                allMovies.Add(DownloadMovies(url));
            }

            return allMovies;
        }

        /// <summary>
        /// Since the only place this method would have an "await" would be in the last line
        /// "Task.WhenAll" and it returns. We should not mark this method with the async modifier
        /// At the call site there is no difference, however, the code generated for this method
        /// will be quite a bit less (without the await). That is no state machine
        /// </summary>
        /// <returns></returns>
        public static Task<IEnumerable<Movie>[]> DownloadDataAsync()
        {
            var allMoviesTasks = new List<Task<IEnumerable<Movie>>>();

            foreach (var url in sources)
            {
                allMoviesTasks.Add(DownloadMoviesAsync(url));
            }

            return Task.WhenAll(allMoviesTasks);
        }

        public static Task<IEnumerable<Movie>[]> DownloadDataAllAsync()
        {
            var allMoviesTasks = new List<Task<IEnumerable<Movie>>>();
            allMoviesTasks.Add(DownloadMoviesAsync("https://matlusstorage.blob.core.windows.net/membervideos/AllMovies.json"));
            return Task.WhenAll(allMoviesTasks);
        }

        public static IEnumerable<IEnumerable<Movie>> DownloadDataParallel()
        {
            var allMovies = new List<IEnumerable<Movie>>();

            Parallel.ForEach(sources, url =>
            {
                allMovies.Add(DownloadMovies(url));
            });

            return allMovies;
        }

        private static IEnumerable<Movie> DownloadMovies(string url)
        {
            var httpResponseMessage = s_httpClient.GetAsync(url).GetAwaiter().GetResult();
            httpResponseMessage.EnsureSuccessStatusCode();
            return httpResponseMessage.Content.ReadAsAsync<IEnumerable<Movie>>().GetAwaiter().GetResult();
        }

        private static async Task<IEnumerable<Movie>> DownloadMoviesAsync(string url)
        {
            var httpResponseMessage = await s_httpClient.GetAsync(url).ConfigureAwait(false);
            httpResponseMessage.EnsureSuccessStatusCode();
            using (var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(responseStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var jsonSerializer = new JsonSerializer();
                return jsonSerializer.Deserialize<IEnumerable<Movie>>(jsonReader);
            }
        }
    }
}