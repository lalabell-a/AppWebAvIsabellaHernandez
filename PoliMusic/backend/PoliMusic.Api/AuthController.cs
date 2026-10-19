using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PoliMusic.Domain;
using PoliMusic.Application;
using PoliMusic.Infrastructure;
namespace PoliMusic.Api;
[ApiController,Route("api/auth")]
public class AuthController(MusicDb db,IPasswordHasher<User> hasher):ControllerBase{
 public static ProfileDto Profile(User u)=>new(u.Id,u.Username,u.Email,u.Birthday?.ToString("yyyy-MM-dd"),u.Photo,u.Role);
 async Task SignIn(User u){var claims=new[]{new Claim(ClaimTypes.NameIdentifier,u.Id.ToString()),new Claim(ClaimTypes.Name,u.Username),new Claim(ClaimTypes.Role,u.Role==1?"Admin":"User")};await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme)));}
 [HttpPost("register")]public async Task<IActionResult> Register(AuthInput input){if(input.Password.Length<12||string.IsNullOrWhiteSpace(input.Username)||string.IsNullOrWhiteSpace(input.Email))return BadRequest(new{message="Se requieren usuario, correo y una contraseña de al menos 12 caracteres."});var name=input.Username.Trim();var email=input.Email.Trim();if(await db.Users.AnyAsync(u=>u.Username==name||u.Email==email))return Conflict(new{message="El nombre de usuario o correo ya está registrado."});var user=new User{Username=name,Email=email,Role=0};user.Password=hasher.HashPassword(user,input.Password);db.Users.Add(user);await db.SaveChangesAsync();await SignIn(user);return Ok(Profile(user));}
 [HttpPost("login")]public async Task<IActionResult> Login(AuthInput input){var user=await db.Users.SingleOrDefaultAsync(u=>u.Username==input.Username.Trim());if(user==null||!user.Password.StartsWith("AQAAAA"))return Unauthorized(new{message="Credenciales no válidas o cuenta heredada que requiere restablecimiento de contraseña."});PasswordVerificationResult result;try{result=hasher.VerifyHashedPassword(user,user.Password,input.Password);}catch(FormatException){return Unauthorized(new{message="Es necesario restablecer esta contraseña."});}if(result==PasswordVerificationResult.Failed)return Unauthorized(new{message="Credenciales no válidas."});if(result==PasswordVerificationResult.SuccessRehashNeeded){user.Password=hasher.HashPassword(user,input.Password);await db.SaveChangesAsync();}await SignIn(user);return Ok(Profile(user));}
 [Authorize,HttpGet("me")]public async Task<IActionResult> Me(){var id=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);var user=await db.Users.FindAsync(id);return user==null?Unauthorized():Ok(Profile(user));}
 [Authorize,HttpPost("logout")]public async Task<IActionResult> Logout(){await HttpContext.SignOutAsync();return NoContent();}
}
[Authorize,ApiController,Route("api/profile")]
public class ProfileController(MusicDb db):ControllerBase{
 [HttpPut]public async Task<IActionResult> Update(ProfileInput input){var u=await db.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!));if(u==null)return Unauthorized();if(string.IsNullOrWhiteSpace(input.Username)||input.Birthday>DateTime.UtcNow)return BadRequest(new{message="Revisa el nombre y la fecha de nacimiento."});if(input.Photo is {Length:>0}&&(!Uri.TryCreate(input.Photo,UriKind.Absolute,out var uri)||uri.Scheme!="https"))return BadRequest(new{message="La imagen debe utilizar una URL HTTPS."});if(await db.Users.AnyAsync(x=>x.Id!=u.Id&&(x.Username==input.Username.Trim()||x.Email==input.Email.Trim())))return Conflict(new{message="Nombre o correo ya utilizados."});u.Username=input.Username.Trim();u.Email=input.Email.Trim();u.Birthday=input.Birthday;u.Photo=input.Photo;await db.SaveChangesAsync();return Ok(AuthController.Profile(u));}
}
