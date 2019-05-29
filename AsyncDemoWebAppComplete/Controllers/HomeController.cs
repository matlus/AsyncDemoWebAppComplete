using AsyncDemoWebAppComplete.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AsyncDemoWebAppComplete.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Sync()
        {
            var sw = Stopwatch.StartNew();

            var data = DataProvider.DownloadData();

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }

        public async Task<ActionResult> ASync()
        {
            var sw = Stopwatch.StartNew();

            var data = await DataProvider.DownloadDataAsync().ConfigureAwait(false);

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }

        public ActionResult SyncP()
        {
            var sw = Stopwatch.StartNew();

            var data = DataProvider.DownloadDataParallel();

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }

        public async Task<ActionResult> AllAsync()
        {
            var sw = Stopwatch.StartNew();

            var data = await DataProvider.DownloadDataAllAsync().ConfigureAwait(false);

            sw.Stop();
            ViewBag.ElapsedMilliseconds = sw.ElapsedMilliseconds;

            return View("Index", data);
        }
    }
}