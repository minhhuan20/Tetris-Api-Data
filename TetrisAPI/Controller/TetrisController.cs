using System;
using System.Linq;
using TetrisAPI.Data;
using TetrisAPI.Dtos;
using TetrisAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetrisAPI.libs;
using System.Net.Http.Headers;
using System.Text;
namespace TetrisAPI.Controllers
{           
    [Route("api/[controller]")]
    [ApiController]
    public class TetrisController : ControllerBase
    {
        private readonly TetrisAPIDbContext _context;   
        private readonly IMapper _mapper;

        static readonly HttpClient client = new HttpClient();

        public TetrisController(TetrisAPIDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        [HttpPost]
        public ActionResult<TetrisReadDtos> CreateScore(TetrisDtos tetrisDtos)
        {

                var subscription = _context.tetris.FirstOrDefault(s=>s.NickName == tetrisDtos.NickName && s.Score == tetrisDtos.Score);
                if(subscription == null){
                    subscription =_mapper.Map<TetrisModel>(tetrisDtos);
                try
                {
                    _context.tetris.Add(subscription);
                    _context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                }
            return Ok();       
        }
        [HttpGet]
        public ActionResult GetHighScore(){
            var highscore = _context.tetris.OrderByDescending(s=>s.Score).Take(5);
            return Ok(highscore);
        }
    }
}