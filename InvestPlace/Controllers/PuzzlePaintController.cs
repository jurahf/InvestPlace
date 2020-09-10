using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Services.Services.PuzzlePaintService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestPlace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuzzlePaintController : ControllerBase
    {
        private readonly IPuzzlePaintService service;

        public PuzzlePaintController(IPuzzlePaintService service)
        {
            this.service = service;
        }


        // https://localhost:44344/api/PuzzlePaint/PuzzleNet/1
        [HttpGet("PuzzleNet/{lotId}")]
        public IActionResult PuzzleNet(int lotId)
        {
            return File(service.PuzzleNet(lotId), "image/png");
        }


    }
}
