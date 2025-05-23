﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using System.Dynamic;
using System.Data.Entity;
using System.Collections.Generic;
using PagedList;
using Org.BouncyCastle.Crypto.Generators;
using System.Drawing;





namespace WebApplication4.Controllers
{

    public class AdminController : Controller
    {
        private readonly dummyclubsEntities _db = new dummyclubsEntities();
        private readonly EmailService _emailService = new EmailService();  // Injecting EmailService


        // GET: Login Page
        public ActionResult Login()
        {
            return View();
        }

        // POST: Handle Login
        [HttpPost]

        // POST: Handle Login

    
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check credentials in Logins table
                var user = _db.Logins.FirstOrDefault(u => u.Email == model.Username && u.PasswordHash == model.Password);

                if (user != null)
                {
                    // Store user session data
                    Session["UserID"] = user.LoginID;
                    Session["UserRole"] = user.Role;
                    Session["UserEmail"] = user.Email;

                    if (user.Role == "Admin")
                    {
                        Session["User1Email"] = user.Email;
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user.Role == "UniversityAdministrator")
                    {
                        // Fetch assigned university
                        var university = _db.UNIVERSITies.FirstOrDefault(u => u.Email == user.Email);

                        if (university != null)
                        {
                            // Store university details in session
                            Session["UniversityID"] = university.UniversityID;
                            Session["UniversityName"] = university.UniversityNAME;
                            Session["UniversityLocation"] = university.Location;

                            // Redirect to University Admin Dashboard
                            return RedirectToAction("Index", "UniversityAdmin");
                        }
                        else
                        {
                            ViewBag.Message = "No university assigned to this administrator.";
                        }
                    }
                    else if (user.Role == "Mentor")
                    {
                        // Store the mentor's university ID in the session
                        Session["UniversityID"] = user.UniversityID;

                        // Optionally, you can store other details like the mentor's ID or role
                        Session["UserID"] = user.LoginID;
                        // This should be in your login controller
                        //Session["MentorID"] = loggedInMentor.MentorID;

                        Session["UserRole"] = user.Role;

                        Session["UserEmail"] = user.Email;


                        return RedirectToAction("Index", "Mentor");

                    }
                    else if(user.Role== "Club Admin")
                    {
                        Session["UserID"] = user.LoginID;
                        return RedirectToAction("Index", "ClubAdmin");
                    }
                    else
                    {
                        ViewBag.Message = "Access Denied! Invalid Role.";
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid email or password.";
                }
            }
            return View(model);
        }


        // Logout Action
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // Add Mentor Action
        //public ActionResult AddMentor()
        //{
        //    return View();
        //}


        //[HttpPost]
        /*public ActionResult AddMentor(USER model, HttpPostedFileBase Photo)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;
                return View(model);
            }

            try
            {
                if (Photo != null && Photo.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Path.GetFileName(Photo.FileName);
                    string path = Path.Combine(uploadDir, fileName);
                    Photo.SaveAs(path);
                    model.PhotoPath = "~/Uploads/" + fileName;
                }

                // Save model to the database
                _db.USERs.Add(model);
                _db.SaveChanges();

                

                // Store success message in TempData
               // TempData["SuccessMessage"] = "Mentor added successfully!";

                // Clear the model (this is optional, but it clears the form)
                model = new USER();  // Resets the model to clear form data

                // Optionally clear any session or other temporary data
                Session.Clear(); // Clears the session data (if needed)

                // Redirect to the same view (this will reload the page with the success message)
                return RedirectToAction("successmentor");
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(model);
            }
        }*/


        //public async Task<ActionResult> AddMentor(USER model, HttpPostedFileBase Photo)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        //        ViewBag.Errors = errors;
        //        return View(model);
        //    }

        //    try
        //    {
        //        // Upload photo if provided
        //        if (Photo != null && Photo.ContentLength > 0)
        //        {
        //            string uploadDir = Server.MapPath("~/Uploads/");
        //            if (!Directory.Exists(uploadDir))
        //            {
        //                Directory.CreateDirectory(uploadDir);
        //            }

        //            string fileName = Path.GetFileName(Photo.FileName);
        //            string path = Path.Combine(uploadDir, fileName);
        //            Photo.SaveAs(path);
        //            model.PhotoPath = "~/Uploads/" + fileName;
        //        }

        //        // Save model to the database
        //        _db.USERs.Add(model);
        //        await _db.SaveChangesAsync();  // Use async for better performance

        //        // Send the email to the mentor with their credentials
        //        string subject = "Welcome to Our Platform!";
        //        string body = $"Hello {model.FirstName},<br/><br/>" +
        //                      $"You have been successfully added as a mentor. Here are your login details:<br/>" +
        //                      $"Username: {model.Email}<br/>" +
        //                      $"Password: {model.Password}<br/><br/>" +
        //                      "Please login and complete your profile.";

        //        // Send email asynchronously
        //        await _emailService.SendEmailAsync(model.Email, subject, body);

        //        // Clear the model (optional, clears the form data)
        //        model = new USER();

        //        // Optionally clear any session or other temporary data
        //        Session.Clear();

        //        // Redirect to success page
        //        return RedirectToAction("successmentor");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the error and display error message
        //        ViewBag.ErrorMessage = "Error: " + ex.Message;
        //        return View(model);
        //    }
        //}

        // GET: Admin Dashboard
        public ActionResult Index()
        {
            return View(); // Renders Index.cshtml (dashboard view)
        }

        // Get dashboard statistics
        [HttpGet]
        public JsonResult GetDashboardStats()
        {
            System.Diagnostics.Debug.WriteLine("GetDashboardStats called");

            try
            {
                var stats = new
                {
                    totalUniversities = _db.UNIVERSITies.Count(),
                    totalSchools = _db.DEPARTMENTs.Count(),
                    totalClubs = _db.CLUBS.Count(),
                    totalMentors = _db.USERs.Count(u => u.Userrole == "Mentor")
                };

                return Json(stats, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetDashboardStats: {ex.Message}");
                return Json(new { error = "Failed to fetch dashboard stats", details = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Get clubs distribution by university (for chart)
        [HttpGet]
        public JsonResult GetClubsDistribution()
        {
            System.Diagnostics.Debug.WriteLine("GetClubsDistribution called");

            try
            {
                var distribution = _db.CLUBS
                    .Include(c => c.DEPARTMENT)
                    .Include(c => c.DEPARTMENT.UNIVERSITY)
                    .GroupBy(c => c.DEPARTMENT.UNIVERSITY)
                    .Select(g => new
                    {
                        universityName = g.Key.UniversityNAME,
                        clubCount = g.Count()
                    })
                    .OrderByDescending(d => d.clubCount)
                    .ToList();

                return Json(distribution, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetClubsDistribution: {ex.Message}");
                return Json(new { error = "Failed to fetch clubs distribution", details = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult successmentor()
        {
            return View();
        }

        // ✅ Show Add University Form
        public ActionResult AddUniversity()
        {
            return View();
        }

        // ✅ Handle University Submission
        [HttpPost]
        // POST: Handle Add University Submission

        
        public async Task<ActionResult> AddUniversity(UNIVERSITY model)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Any())
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                        }
                    }
                }
                return View(model);
            }

            try
            {
                // Set default values
                model.CreatedDate = model.CreatedDate ?? DateTime.Now;
                model.IsActive = model.IsActive ?? true;
                model.IsActiveDate = model.IsActiveDate ?? DateTime.Now;

                // Save University
                _db.UNIVERSITies.Add(model);
                await _db.SaveChangesAsync();

                // Add the administrator in the Logins table (Storing Plain-Text Password)
                var login = new Login
                {
                    Email = model.Email,
                    PasswordHash = "Administrator@123", // ⚠️ Storing password as plain text (Not Recommended)
                    Role = "UniversityAdministrator",
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    UniversityID = model.UniversityID // Assign the foreign key
                };

                _db.Logins.Add(login);
                await _db.SaveChangesAsync();

                // Send email to the administrator with login credentials
                string subject = "Welcome to Our Platform!";
                string body = $"Hello {model.AdministratorName},<br/><br/>" +
                              $"You have been successfully added as the administrator for {model.UniversityNAME}.<br/>" +
                              $"Your login credentials are:<br/>" +
                              $"Username: {model.Email}<br/>" +
                              $"Password: Administrator@123<br/><br/>" +
                              "Please login and manage your university.";

                // Send email asynchronously
                await _emailService.SendEmailAsync(model.Email, subject, body);

                // Success message
                TempData["SuccessMessage"] = "University and administrator added successfully!";
                return RedirectToAction("ManageUniversities");
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(model);
            }
        }


        // ✅ Success Page
        public ActionResult SuccessUniversity()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddDepartment()
        {
            var universities = _db.Set<UNIVERSITY>()
                .Where(u => u.IsActive == true)
                .ToList();
            ViewBag.Universities = new SelectList(universities, "UniversityID", "UniversityNAME");

            return View(new List<DEPARTMENT>());
        }

        [HttpPost]
        public async Task<ActionResult> AddDepartment(List<DEPARTMENT> Departments, int Universityid)
        {
            var universities = _db.Set<UNIVERSITY>()
                .Where(u => u.IsActive == true)
                .ToList();
            ViewBag.Universities = new SelectList(universities, "UniversityID", "UniversityNAME");

            if (Departments == null || Departments.Count == 0)
            {
                ModelState.AddModelError("", "At least one department is required.");
                return View(new List<DEPARTMENT>());
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage).ToList();
                return View(Departments);
            }

            try
            {
                foreach (var department in Departments)
                {
                    department.Universityid = Universityid;
                    department.createdDate = DateTime.Now;
                    department.IsActive = true;
                    department.IsActiveDate = DateTime.Now;

                    _db.Set<DEPARTMENT>().Add(department);
                }

                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Departments added successfully!";
                return RedirectToAction("AddDepartment");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(Departments);
            }
        }


        // ✅ Show Manage Universities Page
        // ManageUniversities
        public ActionResult ManageUniversities(int page = 1, int pageSize = 7)
        {
            var totalUniversities = _db.UNIVERSITies.Count();

            var universities = _db.UNIVERSITies
                                  .OrderByDescending(u => u.CreatedDate)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUniversities / pageSize);
            ViewBag.TotalItems = totalUniversities;

            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, totalUniversities);

            return View(universities);
        }

        [HttpPost]
        public async Task<ActionResult> DeactivateUniversity(int universityId, int page)
        {
            var university = await _db.UNIVERSITies.FindAsync(universityId);
            if (university == null)
            {
                TempData["ErrorMessage"] = "University not found!";
                return RedirectToAction("ManageUniversities", new { page = page, pageSize = 7 });
            }

            university.IsActive = false;
            university.IsActiveDate = DateTime.Now;

            var departments = _db.DEPARTMENTs.Where(d => d.Universityid == universityId).ToList();
            foreach (var dept in departments)
            {
                dept.IsActive = false;
                dept.IsActiveDate = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "University and its departments have been deactivated!";
            return RedirectToAction("ManageUniversities", new { page = page, pageSize = 7 });
        }

        [HttpPost]
        public async Task<ActionResult> ActivateUniversity(int universityId, int page)
        {
            var university = await _db.UNIVERSITies.FindAsync(universityId);
            if (university == null)
            {
                TempData["ErrorMessage"] = "University not found!";
                return RedirectToAction("ManageUniversities", new { page = page, pageSize = 7 });
            }

            university.IsActive = true;
            university.IsActiveDate = DateTime.Now;

            var departments = _db.DEPARTMENTs.Where(d => d.Universityid == universityId).ToList();
            foreach (var dept in departments)
            {
                dept.IsActive = true;
                dept.IsActiveDate = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "University and its departments have been activated!";
            return RedirectToAction("ManageUniversities", new { page = page, pageSize = 7 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUniversity(UNIVERSITY updatedUniversity)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Form is invalid: " + string.Join("; ", errors);
                return RedirectToAction("ManageUniversities");
            }

            var university = await _db.UNIVERSITies.FindAsync(updatedUniversity.UniversityID);
            if (university == null)
            {
                TempData["ErrorMessage"] = "University not found!";
                return RedirectToAction("ManageUniversities");
            }

            university.UniversityNAME = updatedUniversity.UniversityNAME;
            university.Abbreviation = updatedUniversity.Abbreviation;
            university.Location = updatedUniversity.Location;

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "University details updated successfully!";
            return RedirectToAction("ManageUniversities");
        }


        // ✅ Handle Activating a University
        [HttpPost]
        public async Task<ActionResult> ActivateUniversity(int universityId)
        {
            var university = await _db.UNIVERSITies.FindAsync(universityId);
            if (university == null)
            {
                TempData["ErrorMessage"] = "University not found!";
                return RedirectToAction("ManageUniversities");
            }

            university.IsActive = true;
            university.IsActiveDate = DateTime.Now;

            var departments = _db.DEPARTMENTs.Where(d => d.Universityid == universityId).ToList();
            foreach (var dept in departments)
            {
                dept.IsActive = true;
                dept.IsActiveDate = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "University and its departments have been activated!";
            return RedirectToAction("ManageUniversities");
        }

        // ✅ View All Universities (Active & Deactivated)
        // View All Universities with Pagination
        public ActionResult ViewUniversities(int page = 1)
        {
            int pageSize = 5;
            var allUniversities = _db.UNIVERSITies.OrderBy(u => u.UniversityNAME).ToList();
            var pagedUniversities = allUniversities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItemCount = allUniversities.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allUniversities.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, allUniversities.Count);
            ViewBag.ViewType = "ViewUniversities";
            ViewBag.Filter = "All";

            return View("ViewUniversities", pagedUniversities);
        }

        // View Active Universities
        public ActionResult ViewActiveUniversities(int page = 1)
        {
            int pageSize = 5;
            var activeUniversities = _db.UNIVERSITies
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.UniversityNAME)
                .ToList();
            var pagedUniversities = activeUniversities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItemCount = activeUniversities.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)activeUniversities.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, activeUniversities.Count);
            ViewBag.ViewType = "ViewActiveUniversities";
            ViewBag.Filter = "Active";

            return View("ViewUniversities", pagedUniversities);
        }

        // View Deactivated Universities
        public ActionResult ViewDeactivatedUniversities(int page = 1)
        {
            int pageSize = 5;
            var deactivatedUniversities = _db.UNIVERSITies
                .Where(u => u.IsActive == false)
                .OrderBy(u => u.UniversityNAME)
                .ToList();
            var pagedUniversities = deactivatedUniversities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItemCount = deactivatedUniversities.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)deactivatedUniversities.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, deactivatedUniversities.Count);
            ViewBag.ViewType = "ViewDeactivatedUniversities";
            ViewBag.Filter = "Deactivated";

            return View("ViewUniversities", pagedUniversities);
        }


        // Manage Schools (Departments)
        //manage schools
        public ActionResult ManageSchools(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var departments = _db.Set<DEPARTMENT>()
                                 .Include(d => d.UNIVERSITY)
                                 .OrderBy(d => d.DepartmentName)
                                 .ToPagedList(pageNumber, pageSize);

            return View(departments);
        }

        [HttpPost]
        public async Task<ActionResult> DeactivateDepartment(int departmentId)
        {
            var department = await _db.Set<DEPARTMENT>().FindAsync(departmentId);
            if (department == null)
            {
                TempData["ErrorMessage"] = "Department not found!";
                return RedirectToAction("ManageSchools");
            }

            department.IsActive = false;
            department.IsActiveDate = DateTime.Now;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Department has been deactivated!";
            return RedirectToAction("ManageSchools");
        }

        [HttpPost]
        public async Task<ActionResult> ActivateDepartment(int departmentId)
        {
            var department = await _db.Set<DEPARTMENT>().FindAsync(departmentId);
            if (department == null)
            {
                TempData["ErrorMessage"] = "Department not found!";
                return RedirectToAction("ManageSchools");
            }

            department.IsActive = true;
            department.IsActiveDate = DateTime.Now;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Department has been activated!";
            return RedirectToAction("ManageSchools");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDepartment(int DepartmentID, string DepartmentName, int UniversityID)
        {
            // Validate and update the department details
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data. Please try again.";
                return RedirectToAction("ManageSchools");
            }

            var department = await _db.Set<DEPARTMENT>().FindAsync(DepartmentID);
            if (department == null)
            {
                TempData["ErrorMessage"] = "Department not found!";
                return RedirectToAction("ManageSchools");
            }

            // Make sure the department is related to the correct university
            if (department.UNIVERSITY.UniversityID != UniversityID)
            {
                TempData["ErrorMessage"] = "Department does not belong to the selected university!";
                return RedirectToAction("ManageSchools");
            }

            // Update department details
            department.DepartmentName = DepartmentName;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Department updated successfully!";
            return RedirectToAction("ManageSchools");
        }

        // Show All Schools (Departments)
        //view schools
        
        public ActionResult ViewSchools(int? page, string filter = "all")
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            IQueryable<DEPARTMENT> query = _db.DEPARTMENTs.Include(d => d.UNIVERSITY);

            if (filter == "active")
            {
                query = query.Where(d => d.IsActive == true);
            }
            else if (filter == "deactivated")
            {
                query = query.Where(d => d.IsActive == false);
            }

            var departments = query.OrderBy(d => d.DepartmentName).ToPagedList(pageNumber, pageSize);

            ViewBag.Filter = filter;
            return View(departments);
        }

        // Show Active Schools
        public ActionResult ViewActiveSchools()
        {
            var departments = _db.DEPARTMENTs.Include(d => d.UNIVERSITY).Where(d => d.IsActive == true).ToList();
            return View("ViewSchools", departments); // Return same View with filtered data
        }

        // Show Deactivated Schools
        public ActionResult ViewDeactivatedSchools()
        {
            var departments = _db.DEPARTMENTs.Include(d => d.UNIVERSITY).Where(d => d.IsActive == false).ToList();
            return View("ViewSchools", departments); // Return same View with filtered data
        }


        // Show Statistics Page
        /* public ActionResult Statistics()
         {
             // Get universities along with their departments
             var universities = _db.UNIVERSITies
                                    .Include(u => u.DEPARTMENTs)  // Include departments for each university
                                    .ToList();

             return View(universities);  // Passing universities with departments to the view
         }*/

        // ✅ Show All Universities & Their Schools
        // View All Universities and Schools with Pagination
        public ActionResult ViewAllUniversitiesAndSchools(int page = 1)
        {
            int pageSize = 5;
            var allUniversities = _db.UNIVERSITies
                .Include(u => u.DEPARTMENTs)
                .OrderBy(u => u.UniversityNAME)
                .ToList();

            var pagedUniversities = allUniversities
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalItemCount = allUniversities.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allUniversities.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, allUniversities.Count);
            ViewBag.ViewType = "ViewAllUniversitiesAndSchools";
            ViewBag.Filter = "All";

            return View("ViewAllUniversitiesAndSchools", pagedUniversities);
        }

        // View Active Universities and Schools with Pagination
        public ActionResult ViewActiveUniversitiesAndSchools(int page = 1)
        {
            int pageSize = 5;
            var activeUniversities = _db.UNIVERSITies
                .Include(u => u.DEPARTMENTs)
                .Where(u => u.IsActive == true)
                .OrderBy(u => u.UniversityNAME)
                .ToList();

            var pagedUniversities = activeUniversities
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalItemCount = activeUniversities.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)activeUniversities.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, activeUniversities.Count);
            ViewBag.ViewType = "ViewActiveUniversitiesAndSchools";
            ViewBag.Filter = "Active";

            return View("ViewAllUniversitiesAndSchools", pagedUniversities);
        }

        // View Deactivated Universities and Schools with Pagination
        public ActionResult ViewDeactivatedUniversitiesAndSchools(int page = 1)
        {
            int pageSize = 5;
            var deactivatedUniversities = _db.UNIVERSITies
                .Include(u => u.DEPARTMENTs)
                .Where(u => u.IsActive == false)
                .OrderBy(u => u.UniversityNAME)
                .ToList();

            var pagedUniversities = deactivatedUniversities
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalItemCount = deactivatedUniversities.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)deactivatedUniversities.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, deactivatedUniversities.Count);
            ViewBag.ViewType = "ViewDeactivatedUniversitiesAndSchools";
            ViewBag.Filter = "Deactivated";

            return View("ViewAllUniversitiesAndSchools", pagedUniversities);
        }

        //addmentor
        // Add Mentor
        [HttpGet]
        public ActionResult AddMentor()
        {
            try
            {
                ViewBag.Universities = new SelectList(_db.UNIVERSITies
                    .Where(u => (bool)u.IsActive)
                    .Select(u => new { u.UniversityID, u.UniversityNAME }),
                    "UniversityID", "UniversityNAME");

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading universities: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public JsonResult GetActiveDepartments(int universityId)
        {
            var activeDepartments = _db.Set<DEPARTMENT>()
                .Where(d => d.Universityid == universityId && d.IsActive == true)
                .Select(d => new { d.DepartmentID, d.DepartmentName })
                .ToList();

            return Json(activeDepartments, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddMentor(USER model, HttpPostedFileBase Photo, int UniversityID, int DepartmentID)
        {
            ViewBag.Universities = new SelectList(_db.UNIVERSITies
                .Where(u => (bool)u.IsActive)
                .Select(u => new { u.UniversityID, u.UniversityNAME }),
                "UniversityID", "UniversityNAME");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (Photo != null && Photo.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Path.GetFileName(Photo.FileName);
                    string path = Path.Combine(uploadDir, fileName);
                    Photo.SaveAs(path);
                    model.PhotoPath = "~/Uploads/" + fileName;
                }

                model.RegistrationDate = DateTime.Now;
                model.IsActiveDate = DateTime.Now;
                model.IsActive = true;
                model.Userrole = "Mentor";
                model.DepartmentID = DepartmentID;

                _db.USERs.Add(model);
                await _db.SaveChangesAsync();

                var login = new Models.Login
                {
                    Email = model.Email,
                    PasswordHash = "Mentor@123",
                    Role = "Mentor",
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    UniversityID = UniversityID,
                    DepartmentID = DepartmentID
                };

                _db.Logins.Add(login);
                await _db.SaveChangesAsync();

                string subject = "Welcome to Our Platform!";
                string body = $"Hello {model.FirstName},<br/><br/>" +
                              $"You have been successfully added as a mentor. Here are your login details:<br/>" +
                              $"<strong>Username:</strong> {model.Email}<br/>" +
                              $"<strong>Password:</strong> Mentor@123 (Please change your password upon login).<br/><br/>" +
                              "Please log in and complete your profile.";

                await _emailService.SendEmailAsync(model.Email, subject, body);

                TempData["SuccessMessage"] = "Mentor added successfully.";
                return RedirectToAction("AddMentor");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error: " + ex.Message;
                return View(model);
            }
        }



        // View Mentors
        public ActionResult ViewMentors(int page = 1)
        {
            int pageSize = 5;
            var allMentors = _db.USERs.ToList();
            var pagedMentors = allMentors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItemCount = allMentors.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)allMentors.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, allMentors.Count);
            ViewBag.ViewType = "ViewMentors";

            return View("ViewMentors", pagedMentors);
        }

        public ActionResult ViewActiveMentors(int page = 1)
        {
            int pageSize = 5;
            var activeMentors = _db.USERs.Where(u => u.IsActive == true).ToList();
            var pagedMentors = activeMentors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItemCount = activeMentors.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)activeMentors.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, activeMentors.Count);
            ViewBag.ViewType = "ViewActiveMentors";

            return View("ViewMentors", pagedMentors);
        }

        public ActionResult ViewDeactivatedMentors(int page = 1)
        {
            int pageSize = 5;
            var inactiveMentors = _db.USERs.Where(u => u.IsActive == false).ToList();
            var pagedMentors = inactiveMentors.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalItemCount = inactiveMentors.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)inactiveMentors.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, inactiveMentors.Count);
            ViewBag.ViewType = "ViewDeactivatedMentors";

            return View("ViewMentors", pagedMentors);
        }




        // 1. Manage Mentors (View and Actions)
        // Manage Mentors
        public ActionResult ManageMentors(int page = 1)
        {
            int pageSize = 5;

            var totalMentors = _db.USERs.Count(u => u.Userrole == "Mentor");
            var mentors = _db.USERs
                            .Where(u => u.Userrole == "Mentor")
                            .OrderBy(u => u.UserID)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMentors / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, totalMentors);
            ViewBag.TotalItemCount = totalMentors;

            return View(mentors);
        }

        // Deactivate Mentor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeactivateMentor(int mentorId)
        {
            var mentor = _db.USERs.FirstOrDefault(m => m.UserID == mentorId);
            if (mentor != null)
            {
                mentor.IsActive = false;
                mentor.IsActiveDate = DateTime.Now;
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Mentor has been deactivated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Mentor not found.";
            }

            return RedirectToAction("ManageMentors");
        }

        // Activate Mentor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivateMentor(int mentorId)
        {
            var mentor = _db.USERs.FirstOrDefault(m => m.UserID == mentorId);
            if (mentor != null)
            {
                mentor.IsActive = true;
                mentor.IsActiveDate = DateTime.Now;
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Mentor has been activated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Mentor not found.";
            }

            return RedirectToAction("ManageMentors");
        }

        


        // Club Requests (for approving or rejecting clubs)
        public ActionResult ClubRequests()
        {
            var clubs = _db.CLUBS
                .Include(c => c.Login)  // ✅ Fetch Mentor Email from Logins Table
                .Include(c => c.DEPARTMENT.UNIVERSITY)  // ✅ Load University & Department
                .ToList();

            // ✅ Fetch Mentor Names from USERs Table Using Email
            foreach (var club in clubs)
            {
                var mentorUser = _db.USERs.FirstOrDefault(u => u.Email == club.Login.Email);
                club.MentorName = mentorUser != null ? mentorUser.FirstName + " " + mentorUser.LastName : "Not Assigned";
            }

            return View(clubs);
        }


        // Approve Club
        //public ActionResult ApproveClub(int id)
        //{
        //    var club = _db.CLUBS.Find(id);
        //    if (club != null)
        //    {
        //        club.ApprovalStatusID = 2; // 2 means 'APPROVED'
        //        club.IsActive = true;
        //        _db.SaveChanges();
        //    }

        //    return RedirectToAction("ClubRequests");
        //}


        public ActionResult ApproveClub(int id)
        {
            var club = _db.CLUBS.Find(id);
            if (club != null)
            {
                club.ApprovalStatusID = 2; // Approved
                club.IsActive = true;
                _db.SaveChanges();

                // ✅ Create a notification with Start & End Date
                var notification = new Notification
                {
                    LoginID = club.MentorID,  // Mentor's LoginID
                    Message = $"Your club '{club.ClubName}' has been approved!",
                    IsRead = false,  // Unread by default
                    StartDate = DateTime.Now,  // Starts now
                    EndDate = DateTime.Now.AddDays(7),  // Expires in 7 days
                    CreatedDate = DateTime.Now
                };

                _db.Notifications.Add(notification);
                _db.SaveChanges();
            }

            return RedirectToAction("ClubRequests");
        }


        // ❌ Reject Club with Reason
        [HttpPost]
        public ActionResult RejectClub(int id, string reason)
        {
            var club = _db.CLUBS.Find(id);

            if (club != null)
            {
                // ❌ Update Status to 'Rejected'
                club.ApprovalStatusID = 3; // 3 = Rejected
                _db.SaveChanges();

                // ✅ Ensure reason is not empty
                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = "No specific reason provided."; // Default message
                }

                // ✅ Debug: Check if Mentor ID exists
                if (club.MentorID == null)
                {
                    System.Diagnostics.Debug.WriteLine("🚨 Error: MentorID is NULL for club ID: " + id);
                    TempData["ErrorMessage"] = "Mentor ID not found for this club!";
                    return RedirectToAction("ClubRequests");
                }

                // ✅ Create a notification for the mentor
                var notification = new Notification
                {
                    LoginID = club.MentorID, // Mentor's LoginID
                    Message = $"❌ Your club '{club.ClubName}' was rejected.\nReason: {reason}",
                    IsRead = false, // Mark as unread
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(7), // Notification expires in 7 days
                    CreatedDate = DateTime.Now
                };

                _db.Notifications.Add(notification);
                int changes = _db.SaveChanges(); // Save notification

                // ✅ Debug: Check if notification was added successfully
                if (changes > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Notification added successfully for MentorID: {club.MentorID} with reason: {reason}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("🚨 Error: Notification NOT added!");
                }

                TempData["SuccessMessage"] = $"Club '{club.ClubName}' rejected successfully!";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("🚨 Error: Club not found for ID: " + id);
                TempData["ErrorMessage"] = "Club not found!";
            }

            return RedirectToAction("ClubRequests");
        }

        public ActionResult ClubStatus(int page = 1, int? status = null)
        {
            int pageSize = 5;

            var clubsQuery = _db.CLUBS
                .Include(c => c.DEPARTMENT)
                .Include(c => c.DEPARTMENT.UNIVERSITY)
                .Include(c => c.ApprovalStatusTable)
                .Include(c => c.Login)
                .AsQueryable();

            if (status.HasValue)
            {
                clubsQuery = clubsQuery.Where(c => c.ApprovalStatusID == status.Value);
            }

            var totalClubs = clubsQuery.Count();
            var clubs = clubsQuery
                .OrderBy(c => c.ClubName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mentorEmails = clubs.Select(c => c.Login.Email).Distinct().ToList();
            var mentorNames = _db.USERs
                .Where(u => mentorEmails.Contains(u.Email))
                .ToDictionary(u => u.Email, u => u.FirstName);

            foreach (var club in clubs)
            {
                club.MentorName = mentorNames.ContainsKey(club.Login?.Email)
                    ? mentorNames[club.Login.Email]
                    : "Unknown Mentor";
            }

            var mentorIds = clubs.Select(c => c.MentorID).ToList();
            var notifications = _db.Notifications
                .Where(n => n.LoginID.HasValue && mentorIds.Contains(n.LoginID.Value) && n.Message.Contains("rejected"))
                .ToList();

            ViewBag.Notifications = notifications;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalClubs / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, totalClubs);
            ViewBag.TotalItemCount = totalClubs;
            ViewBag.CurrentStatus = status;

            return View(clubs);
        }


        // ManageClubs action with pagination
        public ActionResult ManageClubs(int page = 1)
        {
            int pageSize = 5; // Number of clubs per page

            // Get the list of all clubs and order them by ClubName
            var clubs = _db.CLUBS.OrderBy(c => c.ClubName).ToList();

            // Apply pagination logic
            var pagedClubs = clubs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Calculate the total number of items, pages, and which items are displayed on the current page
            ViewBag.TotalItemCount = clubs.Count;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)clubs.Count / pageSize);
            ViewBag.FirstItemOnPage = ((page - 1) * pageSize) + 1;
            ViewBag.LastItemOnPage = Math.Min(page * pageSize, clubs.Count);

            // Pass the list of paged clubs to the view
            return View(pagedClubs);
        }

        // ActivateClub action to change club status to active
        public ActionResult ActivateClub(int id, int page = 1)
        {
            var club = _db.CLUBS.Find(id);
            if (club != null)
            {
                club.IsActive = true; // Activate the club
                _db.SaveChanges(); // Save the changes to the database
            }

            // Redirect back to the ManageClubs action with the current page
            return RedirectToAction("ManageClubs", new { page });
        }

        // DeactivateClub action to change club status to inactive
        public ActionResult DeactivateClub(int id, int page = 1)
        {
            var club = _db.CLUBS.Find(id);
            if (club != null)
            {
                club.IsActive = false; // Deactivate the club
                _db.SaveChanges(); // Save the changes to the database
            }

            // Redirect back to the ManageClubs action with the current page
            return RedirectToAction("ManageClubs", new { page });
        }



        //quickstats
        public ActionResult ViewAllUniversities()
        {
            var universities = _db.UNIVERSITies
                .Where(u => u.IsActive == true) // Filter only active universities
                .ToList();

            return View(universities);
        }

        public ActionResult GetDepartments(int universityId)
        {
            var departments = _db.DEPARTMENTs
                .Where(d => d.Universityid == universityId && d.IsActive == true) // Filter active departments
                .ToList();

            ViewBag.UniversityName = _db.UNIVERSITies
                .Where(u => u.UniversityID == universityId)
                .Select(u => u.UniversityNAME)
                .FirstOrDefault();

            return View(departments);
        }

        public ActionResult GetClubs(int departmentId)
        {
            var clubs = _db.CLUBS
                .Where(c => c.DepartmentID == departmentId && c.IsActive == true) // Filter active clubs
                .ToList();

            return View(clubs);
        }


        //forgetpassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(LoginViewModel model)
        {
            var user = _db.Logins.FirstOrDefault(l => l.Email == model.Username);
            if (user == null)
            {
                ViewBag.Message = "Email not found.";
                return View();
            }

            var otp = new Random().Next(100000, 999999).ToString();

            user.OTP = otp;
            user.OTPExpiry = DateTime.Now.AddMinutes(5);
            _db.SaveChanges();

            var emailService = new EmailService();
            await emailService.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP is: {otp}");

            return RedirectToAction("VerifyOTP", new { email = user.Email });
        }


        //otpverify
        [HttpGet]
        public ActionResult VerifyOTP(string email)
        {
            return View(new VerifyOTPViewModel { Email = email });
        }

       [HttpPost]
public ActionResult VerifyOTP(VerifyOTPViewModel model)
{
    var user = _db.Logins.FirstOrDefault(l => l.Email == model.Email);

    if (user == null || user.OTP != model.OTP || user.OTPExpiry < DateTime.Now)
    {
        ViewBag.Message = "Invalid or expired OTP.";
        return View(model);
    }

    // ✅ Check if the new password is same as old
    if (user.PasswordHash == model.NewPassword)
    {
        ViewBag.Message = "New password must be different from the old password.";
        return View(model);
    }

    // Update password
    user.PasswordHash = model.NewPassword;
    user.OTP = null;
    user.OTPExpiry = null;

    _db.SaveChanges();

    ViewBag.Message = "Password reset successful!";
    return RedirectToAction("Login");
}


        //chnagepassword
        [HttpGet]
        public ActionResult ChangePassword()
        {
         
            if (Session["User1Email"] == null)
            {
                TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                return RedirectToAction("Login", "Admin");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userEmail = Session["User1Email"]?.ToString();

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["ErrorMessage"] = "Your session has expired. Please login again.";
                return RedirectToAction("Login", "Admin");
            }

            var user = _db.Logins.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (user.PasswordHash != model.CurrentPassword)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            user.PasswordHash = model.NewPassword;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("Index", "Admin"); // Updated to point to dashboard
        }


        [HttpGet]
        public ActionResult ViewDetails(int clubId)
        {
            // Fetch the club
            var club = _db.CLUBS.FirstOrDefault(c => c.ClubID == clubId);
            if (club == null)
            {
                return HttpNotFound();
            }

            // Fetch all events under this club
            var events = _db.EVENTS
                .Where(e => e.ClubID == clubId)
                .Select(e => new EventDetailsViewsModel
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    EventDescription = e.EventDescription,
                    EventDate = (DateTime)e.EventCreatedDate,
                    
                    Venue = "ICFAI Foundation,Hydreabad"
                    
                })
                .ToList();

            // Prepare the view model
            var model = new ClubDetailsViewModel
            {
                ClubID = club.ClubID,
                ClubName = club.ClubName,
                Description = club.Description,
                Events = events
            };

            return View(model);
        }


    }
}