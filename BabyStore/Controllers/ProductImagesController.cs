using BabyStore.DAL;
using BabyStore.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BabyStore.Controllers
{
    public class ProductImagesController : Controller
    {
        private StoreContext db = new StoreContext();

        // GET: ProductImages
        public ActionResult Index()
        {
            return View(db.ProductImages.ToList());
        }

        // GET: ProductImages/Create
        public ActionResult Upload()
        {
            return View();
        }

        // POST: ProductImages/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase[] files)
        {
            bool allValid = true;
            string inValidFiles = "";

            db.Database.Log = sql => Trace.WriteLine(sql);

            // Check the user has entered a file
            if (files[0] != null)
            {
                // if the user has entered less than ten files
                if (files.Length <= 10)
                {
                    // Check they are all valid
                    foreach (var file in files)
                    {
                        if (!ValidateFile(file))
                        {
                            allValid = false;
                            inValidFiles += ", " + file.FileName; //TODO cut path
                        }
                    }

                    // if they are all valid, than try to save them to disk
                    if (allValid)
                    {
                        foreach (var file in files)
                        {
                            try
                            {
                                SaveFileToDisk(file);
                            }
                            catch (Exception)
                            {
                                ModelState.AddModelError("FileName", "Sorry. an error occured saving the files to disk, please try again.");
                            }
                        }
                    }
                    else
                    {
                        // Else add a error listing out the invalid files
                        ModelState.AddModelError("FileName", "All files must be gif, png, jpeg or jpg and less than 2MB in size. The following files" + inValidFiles + " are not valid");
                    }
                }
                // The user has entered more than 10 files
                else
                {
                    ModelState.AddModelError("FileName", "Please only upload up to 10 files at a time");
                }
            }
            else
            {
                // If the user has not entered a file - return an error message
                ModelState.AddModelError("FileName", "Please choose a file");
            }

            if (ModelState.IsValid)
            {
                bool duplicates = false;
                bool otherDbError = false;
                string duplicateFiles = "";
                string otherDbErrorText = "";

                foreach (var file in files)
                {
                    // Try and save each file
                    var filename = System.IO.Path.GetFileName(file.FileName);
                    var productToAdd = new ProductImage { FileName = filename };
                    try
                    {
                        db.ProductImages.Add(productToAdd);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // if there is an exception, check if it is cause by a duplicate file
                        SqlException innerException = ex.InnerException.InnerException as SqlException;
                        if (innerException != null && innerException.Number == 2601)
                        {
                            duplicateFiles += ", " + filename;
                            duplicates = true;
                            db.Entry(productToAdd).State = EntityState.Detached;
                        }
                        else
                        {
                            otherDbError = true;
                            otherDbErrorText = ex.ToString();
                        }
                    }

                }

                // Add a list of duplicate files to the error message
                if (duplicates)
                {
                    ModelState.AddModelError("FileName", "All files uploaded except the files: "
                        + duplicateFiles
                        + ", witch already exist in the system.");
                    return View();
                }
                else if (otherDbError)
                {
                    ModelState.AddModelError("FileName", "Sorry, an error has occured saving to the database: " + otherDbErrorText);
                    return View();
                }
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: ProductImages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductImage productImage = db.ProductImages.Find(id);

            // Find all the mappings for this image
            var mappingsToDelete = productImage.ProductImageMappings
                    .Where(pim => pim.ProductImageID == id);
            foreach(var mappingToDelete in mappingsToDelete)
            {
                // find all mappings in current product
                var mappingsToUpdate = db.ProductImageMappings
                        .Where(pim => pim.ProductID == mappingToDelete.ProductID);
                
                // update greater mappings
                foreach (var mappingToUpdate in mappingsToUpdate)
                {
                    if (mappingToUpdate.ImageNumber > mappingToDelete.ImageNumber)
                    {
                        mappingToUpdate.ImageNumber--;
                    }
                }
            }

            System.IO.File.Delete(Request.MapPath(Constants.ProductImagePath + productImage.FileName));
            System.IO.File.Delete(Request.MapPath(Constants.ProductThumbnailPath + productImage.FileName));
            db.ProductImages.Remove(productImage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ValidateFile(HttpPostedFileBase file)
        {
            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            string[] allowedFileTypes = { ".gif", ".png", ".jpeg", ".jpg" };
            if ((file.ContentLength > 0 && file.ContentLength < 2097152)
                && allowedFileTypes.Contains(fileExtension))
            {
                return true;
            }
            return false;
        }

        private void SaveFileToDisk(HttpPostedFileBase file)
        {
            WebImage img = new WebImage(file.InputStream);
            if (img.Width > 190)
            {
                img.Resize(190, img.Height);
            }

            string fileName = System.IO.Path.GetFileName(file.FileName);
            img.Save(Constants.ProductImagePath + fileName);

            if (img.Width > 100)
            {
                img.Resize(100, img.Height);
            }
            img.Save(Constants.ProductThumbnailPath + fileName);
        }
    }
}
