using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TetrisAPI.Data;
using TetrisAPI.Dtos;
using TetrisAPI.Models;
using AutoMapper;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using TetrisAPI;
using Microsoft.EntityFrameworkCore;

namespace TetrisAPI.libs{
    public class TetrisLibs
    {
        private readonly TetrisAPIDbContext _context;

        public TetrisLibs(TetrisAPIDbContext context)
        {
            _context = context;
        }
        public async void connectHttp(string nickname, int score){
            HttpClient client = new HttpClient();
            string url = "https://localhost:7009/index.html";

            var payload = "{\"Nickname\": \""+nickname+"\",\"Score\": "+score+"}";
            Console.WriteLine(payload);
            Console.WriteLine(url);
            try
            {
                HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponse = client.PostAsync(url, c).GetAwaiter().GetResult();
                httpResponse.EnsureSuccessStatusCode(); // throws if not 200-299
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }    
}