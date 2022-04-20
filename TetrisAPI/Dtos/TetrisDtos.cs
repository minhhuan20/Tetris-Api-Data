using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TetrisAPI.Dtos
{
    public class TetrisDtos
    {
        [Required]
        public int Id{get;set;}
        [Required]
        public string NickName { get; set; }
        [Required]
        public int Score { get; set; }
        [Required]
        public DateTime GameTime{ get; set; }

    }
}