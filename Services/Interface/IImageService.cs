using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tentamen_Server_Side_Programming.Services.Interface
{
    public interface IImageService
    {
        Task<byte[]> GetImageAsync();
    }
}
