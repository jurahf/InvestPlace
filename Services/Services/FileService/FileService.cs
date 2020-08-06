using Services.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Services.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment environment;

        public FileService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }



        public async Task<string> UploadFileAndGetLinkAsync(
            UploadFileType uploadType,
            string extension,
            Stream stream,
            ExtendedUserDto uploadByUser)
        {
            string rootPath = environment.WebRootPath;
            string subFoldler = GetUploadSubfolder(uploadType, uploadByUser);
            string fileName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
            // Path.Combine почему-то не сработал, может из-за веба (проверить на хостинге)
            string path = rootPath.Trim(Path.DirectorySeparatorChar) + $"{Path.DirectorySeparatorChar}" 
                + subFoldler.Trim(Path.DirectorySeparatorChar) + $"{Path.DirectorySeparatorChar}" 
                + fileName.Trim(Path.DirectorySeparatorChar);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return Path.Combine(subFoldler, fileName)
                .Replace(Path.DirectorySeparatorChar, '/'); // для веба
        }


        private string GetUploadSubfolder(UploadFileType uploadType, ExtendedUserDto byUser)
        {
            string result = $"{Path.DirectorySeparatorChar}";

            switch (uploadType)
            {
                case UploadFileType.LotImage:
                    result += $"images{Path.DirectorySeparatorChar}Lots{Path.DirectorySeparatorChar}UserUploads{Path.DirectorySeparatorChar}";
                    break;
                default:
                    break;
            }

            if (byUser != null && byUser.Id > 0)
                result += $"user{byUser.Id}";

            return result;
        }

    }
}
