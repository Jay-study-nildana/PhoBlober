using AzureBlogWebAppDemo.DTO;
using AzureBlogWebAppDemo.Models;
using AzureBlogWebAppDemo.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AzureBlogWebAppDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhotoService _photoService;
        private readonly IBlobStorageStuff _blobStorageStuff;

        //public HomeController(ILogger<HomeController> logger)
        public HomeController(
            IPhotoService photoService
            ,IBlobStorageStuff blobStorageStuff
            )
        {
            //_logger = logger;
            _photoService = photoService;
            _blobStorageStuff = blobStorageStuff;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> PhotoUpload()
        {
            return View();
        }

        public async Task<IActionResult> PhotoUploadSuccessfull()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PhotoUpload(PhotoUploadDTO model)
        {
            if (model.containerName == null)
                model.containerName = "";

                //the backend already has an option to add a default container
                //but you can use this too if you want, but I don't recommend this.

                //if(model?.containerName.Length==0)
                //{
                //    string containerName = "containersept172024onef24502ee-d160-435d-ae07-19f9c8a5dc96"; //as of now, hardcoding it. we will need to get his elsewhere, from DB, later.
                //    model.containerName = containerName;
                //}
                    
                ResponseDto? response = await _photoService.UploadPhotoAsync(model);


                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Photo Uploaded successfully";
                    return RedirectToAction(nameof(PhotoUploadSuccessfull));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            return View(model);
        }

        public async Task<IActionResult> PhotoDisplay()
        {
            GetAllBlobsDTO getAllBlobsDTO = new GetAllBlobsDTO();
            var defaultContainer = _blobStorageStuff.GiveMeDefaultContainerName();
            string containerName = defaultContainer;
            ResponseDto? response = await _photoService.GetPhotosAsync(containerName);


            if (response != null && response.IsSuccess)
            {
                //if there are no images to show, just go to home page.
                if(response.Result == null)
                {
                    TempData["error"] = "no images in the container";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    getAllBlobsDTO = JsonConvert.DeserializeObject<GetAllBlobsDTO>(Convert.ToString(response.Result));
                }
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(getAllBlobsDTO);
        }
    }
}
