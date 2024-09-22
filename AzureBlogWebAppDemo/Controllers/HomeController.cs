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

        public IActionResult PhotoUpload()
        {
            return View();
        }

		public IActionResult PhotoUploadForAnalysis()
		{
			return View();
		}

        public IActionResult PhotoUploadForOCR()
        {
            return View();
        }

        public IActionResult PhotoUploadMultiple()
        {
            return View();
        }

        public IActionResult PhotoUploadSuccessfull()
        {
            return View();
        }

        public IActionResult PhotoAnalyseSuccessfull()
        {
            PhotoAnalysedDTO photoAnalysedDTO = new PhotoAnalysedDTO();
            if (TempData["newuser"] is string s)
            {
                var response = JsonConvert.DeserializeObject<ResponseDto>(s);
                // use newUser object now as needed
                photoAnalysedDTO = JsonConvert.DeserializeObject<PhotoAnalysedDTO>(Convert.ToString(response.Result));

            }
            return View(photoAnalysedDTO);
        }

        
        public IActionResult PhotoOCRSuccessfull()
        {
            PhotoOCRedDTO photoAnalysedDTO = new PhotoOCRedDTO();
            if (TempData["newuser"] is string s)
            {
                var response = JsonConvert.DeserializeObject<ResponseDto>(s);
                // use newUser object now as needed
                photoAnalysedDTO = JsonConvert.DeserializeObject<PhotoOCRedDTO>(Convert.ToString(response.Result));

            }
            return View(photoAnalysedDTO);
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

        [HttpPost]
        public async Task<IActionResult> PhotoUploadMultiple(PhotoUploadMultipleDTO model)
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

            ResponseDto? response = new ResponseDto();

            foreach (var file in model.Image)
            {
                var tempPhotoUploadDTO = new PhotoUploadDTO();
                tempPhotoUploadDTO.Image = file;
                response = await _photoService.UploadPhotoAsync(tempPhotoUploadDTO);
            }

            //ResponseDto? response = new ResponseDto();
            //response.IsSuccess = true;


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

        [HttpPost]
        public async Task<IActionResult> PhotoUploadForAnalysis(PhotoUploadDTO model)
        {
            if (model.containerName == null)
                model.containerName = "";

            ResponseDto? response = await _photoService.UploadForPhotoAnalysis(model);


            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Photo Analysed successfully";

                var s = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                TempData["newuser"] = s;
                //return RedirectToAction("Index", "Users");
                return RedirectToAction(nameof(PhotoAnalyseSuccessfull));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PhotoUploadForOCR(PhotoUploadDTO model)
        {
            if (model.containerName == null)
                model.containerName = "";

            ResponseDto? response = await _photoService.UploadForOCRAnalysis(model);


            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Photo OCR successfully";

                var s = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                TempData["newuser"] = s;
                //return RedirectToAction("Index", "Users");
                return RedirectToAction(nameof(PhotoOCRSuccessfull));
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

                if(getAllBlobsDTO.BlobFullURL.Count < 6)
                {
					TempData["error"] = "not enough images to show. Upload "+ (6-getAllBlobsDTO.BlobFullURL.Count) + " more images please";
					return RedirectToAction(nameof(Index));
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
