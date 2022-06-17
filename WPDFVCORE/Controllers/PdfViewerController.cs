using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Syncfusion.EJ2.PdfViewer;
using System;
using System.Collections.Generic;
using System.IO;

namespace PdfViewerService2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PdfViewerController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        public PdfViewerController(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            //Console.WriteLine("PdfViewerController initialized");
        }

        [HttpPost("Load")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        //[Microsoft.AspNetCore.Cors.DisableCors]
        [Route("[controller]/Load")]
        //Post action for Loading the PDF documents   
        public IActionResult Load(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Load called");
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            //PdfRenderer.ReferencePath =  @"Z:\_SVN\_publish\fileservice\wwwroot\Files\";
            //PdfRenderer.ReferencePath = @"X:\wwwroot\Files\";
            //PdfRenderer.ReferencePath = @"C:\_SERVICES_\wwwroot-edelne\Files\";
            PdfRenderer.ReferencePath = @"C:\_SERVICES_\wwwroot-elgocas\Files\";

            MemoryStream stream = new MemoryStream();
            object jsonResult = new object();
            if (jsonObject != null && jsonObject.ContainsKey("document") /*&& jsonObject.ContainsKey("docpath")*/)
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {
                    string documentPath = GetDocumentPath(jsonObject["document"]/*, jsonObject["docpath"]*/);
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(documentPath);
                        stream = new MemoryStream(bytes);
                        Console.WriteLine(documentPath + "  found");
                    }
                    else
                    {
                        Console.WriteLine(documentPath +  " Doc not found");
                        return this.Content(jsonObject["document"] + " is not found");
                    }
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(jsonObject["document"]);
                    stream = new MemoryStream(bytes);
                }
            }
            jsonResult = pdfviewer.Load(stream, jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Bookmarks")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Bookmarks")]
        //Post action for processing the bookmarks from the PDF documents
        public IActionResult Bookmarks(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Bookmarks called");
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            var jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));

        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderPdfPages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderPdfPages")]
        //Post action for processing the PDF documents  
        public IActionResult RenderPdfPages(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            Console.WriteLine("RenderPdfPages called");
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetPage(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("RenderThumbnailImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderThumbnailImages")]
        //Post action for rendering the ThumbnailImages
        public IActionResult RenderThumbnailImages(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            Console.WriteLine("RenderThumbNailImages called ");
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }
        [AcceptVerbs("Post")]
        [HttpPost("RenderAnnotationComments")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/RenderAnnotationComments")]
        //Post action for rendering the annotations
        public IActionResult RenderAnnotationComments(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Renderannotarionscomments called");
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        [AcceptVerbs("Post")]
        [HttpPost("ExportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportAnnotations")]
        //Post action to export annotations
        public IActionResult ExportAnnotations(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Export Annotations called");
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.GetAnnotations(jsonObject);
            return Content(jsonResult);
        }
        [AcceptVerbs("Post")]
        [HttpPost("ImportAnnotations")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ImportAnnotations")]
        //Post action to import annotations
        public IActionResult ImportAnnotations(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Import Annotations called");
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = string.Empty;
            if (jsonObject != null && jsonObject.ContainsKey("fileName"))
            {
                string documentPath = GetDocumentPath(jsonObject["fileName"]/*, jsonObject["docpath"]*/);
                if (!string.IsNullOrEmpty(documentPath))
                {
                    jsonResult = System.IO.File.ReadAllText(documentPath);
                }
                else
                {
                    return this.Content(jsonObject["document"] + " is not found (import anotations)");
                }
            }
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload3")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ExportFormFields")]
        public IActionResult ExportFormFields(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("ExportFormFields called");
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            string jsonResult = pdfviewer.ExportFormFields(jsonObject);
            return Content(jsonResult);
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload2")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/ImportFormFields")]
        public IActionResult ImportFormFields(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("ImportFormFields called");
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object jsonResult = pdfviewer.ImportFormFields(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }

        [AcceptVerbs("Post")]
        [HttpPost("Unload")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Unload")]
        //Post action for unloading and disposing the PDF document resources  
        public IActionResult Unload(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Unload called");
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            pdfviewer.ClearCache(jsonObject);
            return this.Content("Document cache is cleared");
        }


        [HttpPost("Download")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/Download")]
        //Post action for downloading the PDF documents
        public IActionResult Download(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            Console.WriteLine("Download called");
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            //PdfRenderer.ReferencePath = @"Z:\_SVN\_publish\fileservice\wwwroot\Files\";
            PdfRenderer.ReferencePath = @"C:\_SERVICES_\wwwroot-edelne\Files\";
            //PdfRenderer.ReferencePath = @"X:\wwwroot\Files\";
            string documentBase =  pdfviewer.GetDocumentAsBase64(jsonObject);
            //Console.WriteLine("DocumentBase" + documentBase);

            string documentPath = "";
            string base64String = "";
            if (jsonObject != null && jsonObject.ContainsKey("documentId"))
            {

                documentPath = FormatPathDownload(jsonObject["documentId"]);

                if(documentPath != "")
                {
                    Console.WriteLine("DocumentPath:" + documentPath);
                    string tmpdirectoryName = "";
                    tmpdirectoryName = Path.GetDirectoryName(documentPath);
                    if (!Directory.Exists(tmpdirectoryName))
                    {
                        Directory.CreateDirectory(tmpdirectoryName);
                    }


                    //Guarda fichero en ruta especificada. 
                    base64String = documentBase.Split(new string[] { "data:application/pdf;base64," }, StringSplitOptions.None)[1];
                    if (base64String != null || base64String != string.Empty)
                    {
                        byte[] byteArray = Convert.FromBase64String(base64String);
                        System.IO.File.WriteAllBytes(documentPath, byteArray);
                        Console.Write("Saved on " + documentPath);
                    }


                }

            }
            //return Content(String.Empty);
            //devuelve fichero descarga. 
            return Content(String.Empty);
            //return Content(documentPath);

        }

        [HttpPost("PrintImages")]
        [Microsoft.AspNetCore.Cors.EnableCors("MyPolicy")]
        [Route("[controller]/PrintImages")]
        //Post action for printing the PDF documents
        public IActionResult PrintImages(/*string id,*/ [FromBody] Dictionary<string, string> jsonObject)
        {
            //Initialize the PDF Viewer object with memory cache object
            PdfRenderer pdfviewer = new PdfRenderer(_cache);
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return Content(JsonConvert.SerializeObject(pageImage));
        }

        //Gets the path of the PDF document
        private string GetDocumentPath(string document/*, string pathtodoc*/)
        {
            string documentPath = string.Empty;
            string Refpath = PdfRenderer.ReferencePath;


            if(document.Contains(":"))
            {

                string[] separatedFile = document.Split(':');


                document = separatedFile[0];
                Console.WriteLine("Document Contains : "+ document );
            }

            string path = Path.GetFullPath(Refpath + /*pathtodoc +*/ document);

            if (System.IO.File.Exists(path))
            {
                documentPath = path;

                /*
                var path = _hostingEnvironment.ContentRootPath;
                if (System.IO.File.Exists(path + "\\Data\\" + document))
                    documentPath = path + "\\Data\\" + document;*/
            }
            else
            {
                documentPath = "";
            }
            Console.WriteLine("GetDocumentPath: " + documentPath);
            return documentPath;
        }

        private string FormatPathDownload(string document)
        {
            string path = string.Empty;

            string documentPath = string.Empty;
            string documentName = string.Empty;
            string RefPath = PdfRenderer.ReferencePath;

            string tmpDate = "";
            tmpDate = "cp";//DateTime.Now.ToString("ddMMyyhhmmss");


            if (document.Contains(":"))
            {
                string[] separatedFile = document.Split(':');
                documentPath = separatedFile[1].Replace("?","/");

                string tmpDocName = "";
                tmpDocName = separatedFile[0];
                documentName = Path.GetFileNameWithoutExtension(tmpDocName) + "_" + tmpDate + Path.GetExtension(tmpDocName);

                string FolderName = new DirectoryInfo(System.IO.Path.GetDirectoryName(documentPath)).Name;
                string DocFolderName = Path.GetDirectoryName(documentPath);

                path = Path.GetFullPath(RefPath + DocFolderName + "\\" +  documentName);
                //path = Path.GetFullPath(RefPath + FolderName + "\\" + documentName);
                Console.WriteLine("Download " + path);
            }
            else
            {
                path = "";
                Console.WriteLine("Download " + path);

            }

            return path;

        }


        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        /*
        // POST api/values
        [HttpPost]
        public void Post()
        {
        }

        //   public void Index()

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/



    }
}