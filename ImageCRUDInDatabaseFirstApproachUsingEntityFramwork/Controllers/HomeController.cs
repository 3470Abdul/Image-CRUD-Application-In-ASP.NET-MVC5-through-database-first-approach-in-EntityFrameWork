using ImageCRUDInDatabaseFirstApproachUsingEntityFramwork.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageCRUDInDatabaseFirstApproachUsingEntityFramwork.Controllers
{
    public class HomeController : Controller
    {
        ImageCRUDEntities db = new ImageCRUDEntities();
        public ActionResult Index()
        {
            var data = db.Employees.ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Employee e)
        {
            if(ModelState.IsValid == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                string extension = Path.GetExtension(e.ImageFile.FileName);
                HttpPostedFileBase postedFile = e.ImageFile;
                int length = postedFile.ContentLength;

                if(extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if(length <= 1000000)
                    {
                        fileName = fileName + extension;
                        e.image_path = "~/Images/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        e.ImageFile.SaveAs(fileName);
                        db.Employees.Add(e);
                        int a = db.SaveChanges();
                        if(a > 0)
                        {
                            TempData["Message"] = "Data Inserted Successfully";
                            ModelState.Clear();
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Image Size Should be less than 1MB";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Formate Not Supported";
                }
            }
                return View(); 
        }

        public ActionResult Edit(int id)
        {
            var row = db.Employees.Where(model => model.Id == id).FirstOrDefault();
            Session["Image"] = row.image_path;
            return View(row);
        }
        [HttpPost]
        public ActionResult Edit(Employee e)
        {
            if (ModelState.IsValid == true)
            {
                if (e.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(e.ImageFile.FileName);
                    string extension = Path.GetExtension(e.ImageFile.FileName);
                    HttpPostedFileBase postedFile = e.ImageFile;
                    int length = postedFile.ContentLength;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 1000000)
                        {
                            fileName = fileName + extension;
                            e.image_path = "~/Images/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/Images/"), fileName);
                            e.ImageFile.SaveAs(fileName);
                            db.Entry(e).State = EntityState.Modified;
                            int a = db.SaveChanges();
                            if (a > 0)
                            {
                                string ImagePath = Request.MapPath(Session["Image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["Message"] = "Data Updated Successfully";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Data Not Updated";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Image Size Should be less than 1MB";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Format Not Supported";
                    }
                }
                else
                {
                    e.image_path = Session["Image"].ToString();
                    db.Entry(e).State = EntityState.Modified;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["Message"] = "Data Updated Susessfully";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                else
                    {
                        TempData["ErrorMessage"] = "Data Not Updated";
                    }
                }
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            if(id > 0)
            {
                var row = db.Employees.Where(model => model.Id == id).FirstOrDefault();
                if(row != null)
                {
                    db.Entry(row).State = EntityState.Deleted;
                    int a = db.SaveChanges();
                    if(a > 0)
                    {
                        TempData["Message"] = "Data Deleted Successfully";
                        string ImagePath = Request.MapPath(row.image_path.ToString());
                        if(System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Data Not Deleted";
                    }
                }
               
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}