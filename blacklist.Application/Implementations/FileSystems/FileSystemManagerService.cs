 
namespace blacklist.Application.Implementations.FileSystems
{
    public class FileSystemManagerService: IFileSystemManagerService
    {
    
        private readonly IHostEnvironment _hostEnvironment;
        private readonly SystemSettings _systemSettings;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
       
        public FileSystemManagerService( IHostEnvironment hostEnvironment, IOptions<SystemSettings> options, ILoggerFactory logger, IConfiguration config)
        {

           

            _hostEnvironment = hostEnvironment;
            _systemSettings = options.Value;
            _logger = logger.CreateLogger<FileSystemManagerService>();
            _config = config;
           
        }
        public string LocalLocation { get; set; }
        private byte[] GetFile(string url, bool isUrl)
        {
            Stream stream = null;
            byte[] buf;

            try
            {
                WebProxy myProxy = new WebProxy();
                if (!string.IsNullOrWhiteSpace(url))
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                    #region Added newly 30-10-2021

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, cert, chain, sslPolicyErrors) => true;
                    req.Credentials = CredentialCache.DefaultCredentials;
                    req.Method = "GET";
                    req.ContentType = "application/x-www-form-urlencoded";

                    #endregion

                    HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                    stream = response.GetResponseStream();

                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int len = (int)(response.ContentLength);
                        buf = br.ReadBytes(len);
                        br.Close();
                    }

                    stream.Close();
                    response.Close();

                    return buf;
                }

            }
            catch (Exception exp)
            {
                //  errorOcurred = true;

                buf = null;
            }

            return null;

        }
        public String ConvertImageURLToBase64(String url)
        {

            StringBuilder _sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(url))
            {
                Byte[] _byte = this.GetFile(url, true);

                if (_byte != null) { _sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length)); }
                //_sb.Append(Convert.ToBase64String(_byte, 0, _byte.Length));
            }


            return _sb.ToString();
        }
        public string GetImage(string photoUrl)
        {
            _logger.LogInformation("Inside the GetImage  method of the DocumetReviewService");
            _logger.LogInformation($"Document URL={photoUrl} passed to the GetImage method of the DocumetReviewService");

            var finaleImagePath = string.Empty;
            var base64Image = string.Empty;
            string returnedBaseUrl = string.Empty;

            if (!string.IsNullOrWhiteSpace(photoUrl))
            {
                var photoLocationSplit = photoUrl.Split('\\');


                var photoImage = photoLocationSplit[photoLocationSplit.Length - 1];
                string getphoneNumber = photoImage.Split('_')[0];
                string imageWithPath = $"{returnedBaseUrl}{getphoneNumber}/{photoImage}";// Path.Combine(returnedBaseUrl, photoImage);
                                                                                         //string imageWithPath = $"{returnedBaseUrl}221771000000.jpg";

                finaleImagePath = imageWithPath.Replace('\\', '/');
                base64Image = ConvertImageURLToBase64(finaleImagePath);
                if (string.IsNullOrWhiteSpace(base64Image))
                {
                    _logger.LogInformation("ConvertImageURLToBase64  returned an empty base64string");
                }
                else
                {
                    _logger.LogInformation("ConvertImageURLToBase64  returned  value");
                }
            }


            return base64Image;

        }

        public async Task<string> SaveFilesWebAPIFolder(string rawFile, string fileName, string fileExtension)
        {



            // var filePath = HttpContext.Current.Server.MapPath("~/FileUploads/");
            // var filePath = "http://localhost/apitest/FileUploads/Ojukwu.jpg";
            //  var filePath = _hostEnvironment.ContentRootPath + "\\FileUploads\\";
            var filePath = _systemSettings.FileLocalPath;
            if (fileExtension?.ToLower().Contains("mp4") == true || fileExtension?.ToLower().Contains("wav") == true || fileExtension?.ToLower().Contains("mp3") == true)
            {
                filePath = _systemSettings.VideoLocalPath;
            }
            var filePathUlr = _systemSettings.ReturnFileUrl;

            if (string.IsNullOrEmpty(rawFile))
            {
                return string.Empty;
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            //else
            //{
            //    FileAttributes(ref fileName, fileType, ref fileExtension);
            //}


            var filePaths = $"{filePath}{fileName}.{fileExtension}";
            if (File.Exists($"{filePaths}"))
            {
                File.Delete($"{filePaths}");
            }

            var file = $"{filePath}{fileName}.{fileExtension}";

            await File.WriteAllBytesAsync(file, Convert.FromBase64String(rawFile));

            LocalLocation = file;
            filePathUlr = Path.Combine(filePathUlr, $"{fileName}.{fileExtension}");
            return filePathUlr;




        }


        public string GetFileByConfigOnBase64(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {

                var returnedBaseUrl = _config.GetValue<string>("SystemSettings:CISWebFileURL");
                if (returnedBaseUrl != null)
                {
                    var fileWithPath = $"{returnedBaseUrl}{filename}";
                    string base64 = ConvertImageURLToBase64(fileWithPath);
                    return base64;


                }
            }
            return string.Empty;
        }
        public string GetFileByUrl(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {

                var returnedBaseUrl = _config.GetValue<string>("SystemSettings:CISWebFileURL");
                if (returnedBaseUrl != null)
                {
                    var fileWithPath = $"{returnedBaseUrl}{filename}";

                    return fileWithPath;


                }
            }
            return string.Empty;
        }


        public string GetVideoByConfigOnBase64(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {

                var returnedBaseUrl = _config.GetValue<string>("SystemSettings:CISWebVideoURL");
                if (returnedBaseUrl != null)
                {
                    var fileWithPath = $"{returnedBaseUrl}{filename}";
                    string base64 = ConvertImageURLToBase64(fileWithPath);
                    return base64;


                }
            }
            return string.Empty;
        }

        public string GetVideoByUrl(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {

                var returnedBaseUrl = _config.GetValue<string>("SystemSettings:CISWebVideoURL");
                if (returnedBaseUrl != null)
                {
                    var fileWithPath = $"{returnedBaseUrl}{filename}";

                    return fileWithPath;


                }
            }
            return string.Empty;
        }


    }
}
