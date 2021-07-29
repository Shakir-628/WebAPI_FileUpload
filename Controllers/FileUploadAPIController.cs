using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebApiFileUpload.Models;

namespace WebApiFileUpload.Controllers
{
    [RoutePrefix("API/Demo")]
    public class FileUploadAPIController : ApiController
    {

        FileDetailsEntities db = new FileDetailsEntities();
        tblFileDetail obj = new tblFileDetail();

        [HttpPost]
        [Route("AddFileDetails")]
        public IHttpActionResult AddFile()
        {
            string result = "";
            try
            {  // Create database object           // // Create Table object
                string fileName = null;
                string imageName = null;
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files["FileUpload"];
                var postedImage = httpRequest.Files["ImageUpload"];
                obj.UserName = httpRequest.Form["UserName"];

                if (postedFile != null)
                {
                    fileName = new String(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    fileName = fileName + DateTime.Now.ToString("yyyy-MM-dd") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Files/" + fileName);
                    postedFile.SaveAs(filePath);
                }

                if (postedImage != null)
                {
                    imageName = new String(Path.GetFileNameWithoutExtension(postedImage.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.Now.ToString("yyyy-MM-dd") + Path.GetExtension(postedImage.FileName);
                    var imagePath = HttpContext.Current.Server.MapPath("~/Files/" + imageName);
                    postedImage.SaveAs(imagePath);
                }

                obj.DocFile = fileName;
                obj.Image = imageName;
                db.tblFileDetails.Add(obj);
                int savecount = db.SaveChanges();
                if (savecount > 0)
                {
                    result = "File uploaded sucessfully";
                }
                else
                {
                    result = "File uploaded faild";
                }
            }
            catch (Exception)
            {

            }
            return Ok(result);
        }

        [HttpGet]
        [Route("GetFileDetails")]
        public IHttpActionResult GetFile()
        {
            var url = HttpContext.Current.Request.Url;
            //var lstFile = (dynamic)null;
            IEnumerable<tblFileDetail> lstFile = new List<tblFileDetail>();
            try
            {
                //lstFile = db.tblFileDetails.Select(a => new tblFileDetail
                //{
                //    Id = a.Id,
                //    UserName = a.UserName,
                //    Image = url.Scheme + "://" + url.Host + url.Port + "/Files/" + a.Image,
                //    DocFile = a.DocFile,
                //}).ToList();
                lstFile = (from item in db.tblFileDetails
                                       select item).ToList();
            }

            catch (Exception ex)
            {

            }
            return Ok(lstFile);
        }

        [HttpGet]
        [Route("GetImage")]
        public HttpResponseMessage GetImage(string image)
        {
            try
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                string imagePath = HttpContext.Current.Server.MapPath("~/Files/") + image + ".png";
                //Check whether File exists.  
                if (!File.Exists(imagePath))
                {
                    //Throw 404 (Not Found) exception if File not found.
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ReasonPhrase = String.Format("Image Not Found : {0} .", image);
                    throw new HttpResponseException(response);
                }
                byte[] bytes = File.ReadAllBytes(imagePath);
                response.Content = new ByteArrayContent(bytes);
                //Set the Response Content Length.  
                response.Content.Headers.ContentLength = bytes.LongLength;
                //Set the Content Disposition Header Value and FileName.  
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = image + ".PNG";
                //Set the File Content Type.  
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(image + ".PNG"));

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("GetFile")]
        //download file api  
        public HttpResponseMessage GetFile(string docFile)
        {
            //Create HTTP Response.  
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            //Set the File Path.  
            string filePath = System.Web.HttpContext.Current.Server.MapPath("~/Files/") + docFile + ".docx";
            //Check whether File exists.  
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.  
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", docFile);
                throw new HttpResponseException(response);
            }
            //Read the File into a Byte Array.  
            byte[] bytes = File.ReadAllBytes(filePath);
            //Set the Response Content.  
            response.Content = new ByteArrayContent(bytes);
            //Set the Response Content Length.  
            response.Content.Headers.ContentLength = bytes.LongLength;
            //Set the Content Disposition Header Value and FileName.  
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = docFile + ".docx";
            //Set the File Content Type.  
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(docFile + ".docx"));
            return response;
        }

    }
}
