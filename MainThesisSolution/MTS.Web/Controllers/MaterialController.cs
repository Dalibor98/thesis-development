using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTS.Web.Models.Curriculum.Course;
using MTS.Web.Models.Curriculum.Material;
using MTS.Web.Service.IService;
using MTS.Web.Utility;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MTS.Web.Controllers
{
    public class MaterialController : Controller
    {
        private readonly IMaterialService _materialService;
        private readonly ICourseService _courseService;

        public MaterialController(IMaterialService materialService, ICourseService courseService)
        {
            _materialService = materialService;
            _courseService = courseService;
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
        public async Task<IActionResult> Create(MaterialCreateDto materialDto)
        {
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
                    TempData["error"] = response?.Message;
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
                        MaterialType = material.MaterialType
                    };

                    return View(materialEditDto);
                }
            }

            TempData["error"] = response?.Message ?? "Material not found";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize(Roles = SD.RoleLeader)]
        public async Task<IActionResult> Edit(MaterialCreateDto materialDto)
        {
            if (ModelState.IsValid)
            {
                // Verify ownership (similar to above)
                var courseResponse = await _courseService.GetCourseByCodeAsync(materialDto.CourseCode);
                if (courseResponse != null && courseResponse.IsSuccess)
                {
                    var course = JsonConvert.DeserializeObject<CourseDto>(Convert.ToString(courseResponse.Result));
                    
                    // Verify the current user is the professor for this course
                    var userUniversityId = User.FindFirstValue("UniversityId");
                    if (course.ProfessorUniversityId != userUniversityId)
                    {
                        TempData["error"] = "You are not authorized to edit this material";
                        return RedirectToAction("Details", "Course", new { courseCode = materialDto.CourseCode });
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
            }

            return View(materialDto);
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
    }
}