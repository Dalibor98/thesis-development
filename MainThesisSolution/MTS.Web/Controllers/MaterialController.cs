using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Material;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MTS.Web.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materialService;
        private readonly ICourseService _courseService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MaterialController(IMaterialService materialService, ICourseService courseService, IWebHostEnvironment webHostEnvironment)
        {
            _materialService = materialService;
            _courseService = courseService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(string weekCode, string courseCode)
        {
            // Verify that the week belongs to a course owned by the professor
            var courseResponse = await _courseService.GetCourseByCodeAsync(courseCode);
            if (courseResponse == null || !courseResponse.IsSuccess)
            {
                TempData["error"] = "Course not found";
                return RedirectToAction("Index", "Course");
            }

            var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

            // Verify the current user is the professor for this course
            var userUniversityId = User.FindFirstValue("UniversityId");
            if (course.ProfessorUniversityId != userUniversityId)
            {
                TempData["error"] = "You are not authorized to add materials to this course";
                return RedirectToAction("Details", "Course", new { courseCode = courseCode });
            }

            var materialCreateDto = new MaterialCreateDto
            {
                WeekCode = weekCode,
                CourseCode = courseCode
            };

            return View(materialCreateDto);
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Create(MaterialCreateDto materialDto, IFormFile file)
        {
            // We'll handle FileUrl validation ourselves
            ModelState.Remove("FileUrl");

            // Handle different material types
            switch (materialDto.MaterialType)
            {
                case "Text":
                    // For text materials, FileUrl is not needed
                    materialDto.FileUrl = string.Empty;
                    break;

                case "ExternalSource":
                    // Get URL from form
                    materialDto.FileUrl = Request.Form["externalUrl"];
                    if (string.IsNullOrEmpty(materialDto.FileUrl))
                    {
                        ModelState.AddModelError("", "Please enter an external URL.");
                    }
                    break;

                case "PDF":
                case "Video":
                case "Presentation":
                    // Check if file was provided
                    if (file == null || file.Length == 0)
                    {
                        ModelState.AddModelError("", "Please select a file to upload.");
                        break;
                    }

                    // Process file upload
                    try
                    {
                        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "FileStorage",
                                                   materialDto.CourseCode, materialDto.WeekCode);
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        // Sanitize filename
                        var sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName)
                            .Replace(" ", "_")
                            .Replace("(", "")
                            .Replace(")", "")
                            .Replace(";", "")
                            .Replace(":", "");

                        var extension = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid()}_{sanitizedFileName}{extension}";
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        materialDto.FileUrl = $"/FileStorage/{materialDto.CourseCode}/{materialDto.WeekCode}/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error uploading file: {ex.Message}");
                    }
                    break;
            }

            if (ModelState.IsValid)
            {
                var response = await _materialService.CreateMaterialAsync(materialDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Material created successfully";
                    return RedirectToAction("Details", "Course", new { courseCode = materialDto.CourseCode });
                }
                else
                {
                    // Clean up file if API call failed
                    if (!string.IsNullOrEmpty(materialDto.FileUrl) &&
                        (materialDto.MaterialType == "PDF" || materialDto.MaterialType == "Video" || materialDto.MaterialType == "Presentation"))
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, materialDto.FileUrl.TrimStart('/'));
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    TempData["error"] = response?.Message ?? "An error occurred";
                }
            }

            return View(materialDto);
        }


        public async Task<IActionResult> View(string materialCode)
        {
            var response = await _materialService.GetMaterialByCodeAsync(materialCode);

            if (response != null && response.IsSuccess)
            {
                var material = JsonConvert.DeserializeObject<MaterialDto>(Convert.ToString(response.Result));
                return View(material);
            }

            TempData["error"] = response?.Message ?? "Material not found";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(string materialCode)
        {
            var response = await _materialService.GetMaterialByCodeAsync(materialCode);

            if (response != null && response.IsSuccess)
            {
                var material = JsonConvert.DeserializeObject<MaterialDto>(Convert.ToString(response.Result));

                // Get course info to verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(material.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to edit this material";
                        return RedirectToAction("Details", "Course", new { courseCode = material.CourseCode });
                    }

                    var materialEditDto = new MaterialUpdateDto
                    {
                        CourseCode = material.CourseCode,
                        WeekCode = material.WeekCode,
                        MaterialCode = material.MaterialCode,
                        Title = material.Title,
                        Description = material.Description,
                        MaterialType = material.MaterialType,
                        FileUrl = material.FileUrl
                    };

                    return View(materialEditDto);
                }
            }

            TempData["error"] = response?.Message ?? "Material not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(MaterialUpdateDto materialDto, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload if a file was selected
                if (file != null && file.Length > 0)
                {
                    // Only process file if material type requires it
                    if (materialDto.MaterialType == "PDF" || materialDto.MaterialType == "Video" ||
                        materialDto.MaterialType == "Presentation")
                    {
                        // Delete existing file if there is one
                        if (!string.IsNullOrEmpty(materialDto.FileUrl))
                        {
                            var existingFilePath = Path.Combine(_webHostEnvironment.WebRootPath,
                                                             materialDto.FileUrl.TrimStart('/'));
                            if (System.IO.File.Exists(existingFilePath))
                            {
                                System.IO.File.Delete(existingFilePath);
                            }
                        }

                        // Create directory if it doesn't exist
                        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "FileStorage",
                                                   materialDto.CourseCode, materialDto.WeekCode);
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        // Generate unique filename
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                        var filePath = Path.Combine(uploadDir, fileName);

                        // Save file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Save relative path to database
                        materialDto.FileUrl = $"/FileStorage/{materialDto.CourseCode}/{materialDto.WeekCode}/{fileName}";
                    }
                }
                else if (materialDto.MaterialType == "ExternalSource" && !string.IsNullOrEmpty(Request.Form["externalSourceUrl"]))
                {
                    // For external source, save the URL
                    materialDto.FileUrl = Request.Form["externalSourceUrl"];
                }
                else if (materialDto.MaterialType == "Text")
                {
                    // For text type, FileUrl isn't needed
                    materialDto.FileUrl = null;
                }

                var response = await _materialService.UpdateMaterialAsync(materialDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Material updated successfully";
                    return RedirectToAction("Details", "Course", new { courseCode = materialDto.CourseCode });
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }

            return View(materialDto);
        }

        [HttpGet]
        public async Task<IActionResult> Download(string materialCode)
        {
            var response = await _materialService.GetMaterialByCodeAsync(materialCode);

            if (response != null && response.IsSuccess)
            {
                var material = JsonConvert.DeserializeObject<MaterialDto>(Convert.ToString(response.Result));

                if (string.IsNullOrEmpty(material.FileUrl))
                {
                    TempData["error"] = "No file available for download";
                    return RedirectToAction("View", new { materialCode });
                }

                // Get the physical file path
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, material.FileUrl.TrimStart('/'));

                if (!System.IO.File.Exists(filePath))
                {
                    TempData["error"] = "File not found";
                    return RedirectToAction("View", new { materialCode });
                }

                // Get the content type
                var contentType = GetContentType(Path.GetExtension(filePath));

                // Get the original filename
                var fileName = Path.GetFileName(filePath);

                // Use FileResult for reliable file downloads
                return PhysicalFile(filePath, contentType, fileName);
            }

            TempData["error"] = response?.Message ?? "Material not found";
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Delete(string materialCode)
        {
            var response = await _materialService.GetMaterialByCodeAsync(materialCode);

            if (response != null && response.IsSuccess)
            {
                var material = JsonConvert.DeserializeObject<MaterialDto>(Convert.ToString(response.Result));

                // Verify ownership
                var courseResponse = await _courseService.GetCourseByCodeAsync(material.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));

                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to delete this material";
                        return RedirectToAction("Details", "Course", new { courseCode = material.CourseCode });
                    }

                    return View(material);
                }
            }

            TempData["error"] = response?.Message ?? "Material not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> DeleteConfirmed(string materialCode)
        {
            var materialResponse = await _materialService.GetMaterialByCodeAsync(materialCode);
            if (materialResponse != null && materialResponse.IsSuccess)
            {
                var material = JsonConvert.DeserializeObject<MaterialDto>(Convert.ToString(materialResponse.Result));

                // Delete the file if it exists
                if (!string.IsNullOrEmpty(material.FileUrl) &&
                    (material.MaterialType == "PDF" || material.MaterialType == "Video" || material.MaterialType == "Presentation"))
                {
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, material.FileUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                var response = await _materialService.DeleteMaterialAsync(materialCode);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Material deleted successfully";
                    return RedirectToAction("Details", "Course", new { courseCode = material.CourseCode });
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

                return RedirectToAction("Details", "Course", new { courseCode = material.CourseCode });
            }

            TempData["error"] = "Error retrieving material details";
            return RedirectToAction("Index", "Home");
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".mp4":
                    return "video/mp4";
                default:
                    return "application/octet-stream";
            }
        }
        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters from filename
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            // Replace with underscores
            string sanitizedFileName = Regex.Replace(fileName, invalidRegStr, "_");

            // Also replace spaces and other potentially problematic characters
            sanitizedFileName = Regex.Replace(sanitizedFileName, @"[\s;,\(\)]", "_");

            // Limit length to avoid path length issues
            if (sanitizedFileName.Length > 50)
            {
                var extension = Path.GetExtension(sanitizedFileName);
                sanitizedFileName = sanitizedFileName.Substring(0, 46) + extension;
            }

            return sanitizedFileName;
        }
    }
}
