using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blacklist.Application.Interfacses.FileSystems
{
    public interface IFileSystemManagerService
    {
       // string SaveFile(string rawFile, string fileName);
      
            Task<string> SaveFilesWebAPIFolder(string rawFile, string fileName, string fileExtension);
        string LocalLocation { get; set; }    

        string GetVideoByConfigOnBase64(string filename);
        string GetFileByConfigOnBase64(string filename);
        string GetFileByUrl(string filename);
        string GetVideoByUrl(string filename);
    }
}
