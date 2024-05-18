using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using authentication.Data;
using authentication.Dtos;
using authentication.Helpers;
using authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Renci.SshNet.Messages;

namespace authentication.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly JwtService _jwtService;
        public AuthController(IUserRepository repository, JwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        // [HttpGet]
        // public IActionResult Hello()
        // {
        //     return Ok("success");
        // }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            //install bcrypt

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                // Password = dto.Password
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            // _repository.Create(user);
            // return Ok("success");

            //agar mempersingakat dan lebih efesien
            return Created("success", _repository.Create(user));

            //sekarang mari tes di postman
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _repository.GetByEmail(dto.Email);

            if (user == null) return BadRequest(new { message = "Invalid Credentials" });

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid Credentials" });
            }

            //tes terlebih dahulu di posman apakah sudah berjalan dengan semestinya
            //http://localhost:8000/api/login
            //tes dengan menyalahkan inputan email ataupun password nya,
            //agar kondisi diatas terpenuhi

            //step berikutnya mari membuat token jwt(hak akses bagi pengguna)
            //install terlebih dahulu System.IdentityModel.Tokens.Jwt di browser
            //next buat folder Helpers yg didalamnya ada class JwtService
            //dan panggil class tersebut

            var jwt = _jwtService.Generate(user.Id);
            //tambahkan layanan jwt(class jwtService di Startup.cs)

            //step berikutnya
            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return Ok(new
            {
                message = "success",
            });

            //tes kembali di postman dengan email dan password yg benar
        }

        [HttpGet("user")]
        public IActionResult User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = _repository.GetById(userId);

                return Ok(user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return Ok(new
            {
                message = "success",
            });
        }
    }
}