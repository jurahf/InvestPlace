using InvestPlaceDB;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.FileService
{
    public interface IFileService
    {
        Task<string> UploadFileAndGetLinkAsync(UploadFileType uploadType, string extension, Stream stream, ExtendedUserDto uploadByUser);


    }


    public enum UploadFileType
    {
        LotImage,
        Avatar
    }


}
