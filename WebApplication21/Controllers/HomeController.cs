using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WebApplication21.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadImage(string someText, HttpPostedFileBase imageFile)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            imageFile.SaveAs($"{Server.MapPath("/UploadedImages/")}{fileName}");
            new Db(Properties.Settings.Default.ConStr)
                .Add(fileName, someText);
            return RedirectToAction("Index");
        }

        public ActionResult ViewAll()
        {
            return View(new Db(Properties.Settings.Default.ConStr).GetAll());
        }

        public ActionResult ViewSingle(int imageId)
        {
            return View(new Db(Properties.Settings.Default.ConStr).GetAll()
                .FirstOrDefault(i => i.Id == imageId));
        }
    }


    public class UploadedImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Text { get; set; }
    }

    public class Db
    {
        private string _conStr;

        public Db(string conStr)
        {
            _conStr = conStr;
        }

        public void Add(string fileName, string text)
        {
            using (var con = new SqlConnection(_conStr))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Images (FileName, Text) " +
                                  "VALUES (@fileName, @text)";
                cmd.Parameters.AddWithValue("@fileName", fileName);
                cmd.Parameters.AddWithValue("@text", text);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<UploadedImage> GetAll()
        {
            using (var con = new SqlConnection(_conStr))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Images";
                var images = new List<UploadedImage>();
                con.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    images.Add(new UploadedImage
                    {
                        Id = (int)reader["Id"],
                        FileName = (string)reader["FileName"],
                        Text = (string)reader["Text"]
                    });
                }

                return images;
            }
        }
    }
}