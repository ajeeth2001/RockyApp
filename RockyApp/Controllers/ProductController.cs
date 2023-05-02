using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RockyApp.Data;
using RockyApp.Models;
using RockyApp.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace RockyApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            //IEnumerable<Product> objList = _db.Products.ToList();

            //foreach (var obj in objList)
            //{
            //    obj.Category = _db.Categories.FirstOrDefault(x => x.Id == obj.CategoryId);
            //    obj.ApplicationType = _db.ApplicationTypes.FirstOrDefault(x => x.Id == obj.ApplicationTypeId);

            //    //_db.Dispose();
            //}

            IEnumerable<Product> objList = _db.Products.Include(u => u.Category).Include(u => u.ApplicationType);

            return View(objList);
        }


        //Upsert-Get
        public IActionResult Upsert(int? id)
        {
            //Product product = new Product();
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Categories.Select(x => new SelectListItem
            //{
            //    Text=x.Name,
            //    Value = x.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;

            ////conversion required for viewdata
            //ViewData["CategoryDropDown"] = CategoryDropDown; //dictionary type [key]=value;

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }


            //IEnumerable<SelectListItem> ApplicationTypeDropDown = _db.ApplicationTypes.Select(x => new SelectListItem
            //{
            //    Text = x.Name,
            //    Value = x.Id.ToString()
            //});

            //ViewBag.ApplicationTypeDropDown = ApplicationTypeDropDown;
            //if (id == null)
            //{
            //    return View(product);

            //}
            //else
            //{
            //    product = _db.Products.Find(id);
            //    if (product == null) ;
            //    {
            //        return NotFound();

            //    }
            //    return View(product);
            //}

        }

        //POST-Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //creating
                    string upload = webRootPath + WC.ImagePath;
                    string filename = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using(var filesStream=new FileStream(Path.Combine(upload, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStream);
                    }

                    productVM.Product.Image = filename + extension;

                    _db.Add(productVM.Product);
                }
                else
                {
                    //updating
                    var objFromDb = _db.Products.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);
                    if (files.Count() > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string filename = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (var filesStream = new FileStream(Path.Combine(upload, filename + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filesStream);
                        }
                        productVM.Product.Image = filename + extension;

                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Products.Update(productVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            productVM.CategorySelectList = _db.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            productVM.ApplicationTypeSelectList = _db.ApplicationTypes.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(productVM);

        }



        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();

            }

            //Product obj = _db.Products.Find(id);
            //obj.Category = _db.Categories.Find(obj.CategoryId);

            //OR
            //eager loading
            Product product = _db.Products.Include(u => u.Category).Include(u=>u.ApplicationType).FirstOrDefault(u => u.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }


        //Post-Delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            Product obj = _db.Products.Find(id);
            if (obj == null)
            {
                return NotFound();

            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string delete = webRootPath + WC.ImagePath;
            var oldFile = Path.Combine(delete, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
    }
}
