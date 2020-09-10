using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services.Services.PuzzlePaintService
{
    public interface IPuzzlePaintService
    {
        Stream PuzzleNet(int lotId);
    }
}
