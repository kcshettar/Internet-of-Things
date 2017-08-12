
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using CNTK;
using IOT_Project;

namespace FileUploadServer 
{
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public string UploadFile()
        {

            if (HttpContext.Current.Request.Files.AllKeys.Any())
            {
               
                var httpPostedFile = HttpContext.Current.Request.Files["UploadedImage"];
                Console.WriteLine(httpPostedFile);
                
                if (httpPostedFile != null)
                {
                    
                    var fileSavePath = Path.Combine("C:\\Users\\Rohan\\Desktop\\IOT\\", "test4.png");

                    httpPostedFile.SaveAs(fileSavePath);
                    
                    List<ValPerc> output = Program.EvaluationSingleImage(DeviceDescriptor.UseDefaultDevice(), "C:\\Users\\Rohan\\Desktop\\IOT\\test4.png");

                    Console.WriteLine(output);

                    ValPerc actualVal = null;
                    double maxperc = 0;
                    foreach (var item in output)
                    {
                        if (Convert.ToDouble(item.percent) > maxperc)
                        {
                            maxperc = (Convert.ToDouble(item.percent));
                            actualVal = item;
                        }
                    }

                    return actualVal.val;
                }
            }

            return "done";
        }
    }
}