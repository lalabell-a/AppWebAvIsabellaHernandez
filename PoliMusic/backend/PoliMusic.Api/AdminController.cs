using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using PoliMusic.Domain;
using PoliMusic.Infrastructure;
namespace PoliMusic.Api;
public record AdminInput(int? Id,[StringLength(100)]string? Name,[StringLength(100)]string? Lastname,[StringLength(50)]string? Title,[StringLength(255)]string? Path,[StringLength(255)]string? Image,int? Year,int? SongId,int? ArtistId,int? AlbumId,[Range(0,1)]int? Role);
[Authorize(Roles="Admin"),ApiController,Route("api/admin")]
public class AdminController(MusicDb db):ControllerBase{
 [HttpGet("stats")] public async Task<object> Stats()=>new{songs=await db.Songs.CountAsync(),artists=await db.Artists.CountAsync(),albums=await db.Albums.CountAsync(),users=await db.Users.CountAsync(),plays=await db.Songs.SumAsync(x=>x.Plays??0)};
 [HttpGet("{kind}")]public async Task<IActionResult> List(string kind)=>kind switch{
 "songs"=>Ok(await db.Songs.Select(s=>new{s.Id,s.Name,s.Path,s.Plays}).ToListAsync()),
 "artists"=>Ok(await db.Artists.ToListAsync()),"albums"=>Ok(await db.Albums.ToListAsync()),
 "users"=>Ok(await db.Users.Select(u=>new{u.Id,u.Username,u.Email,u.Role}).ToListAsync()),
 "tracklists"=>Ok(await db.Tracklists.ToListAsync()),"players"=>Ok(await db.Players.ToListAsync()),_=>NotFound()};
 [HttpPost("{kind}")]public async Task<IActionResult> Save(string kind,AdminInput input){
 switch(kind){
 case "songs":{if(string.IsNullOrWhiteSpace(input.Name)||input.Name.Length>50||string.IsNullOrWhiteSpace(input.Path)||!input.Path.StartsWith("~/songFiles/")||input.Path.Contains(".."))return BadRequest(new{message="Nombre (máximo 50 caracteres) y ruta ~/songFiles/ requeridos."});var s=input.Id.HasValue?await db.Songs.FindAsync(input.Id):new Song();if(s==null)return NotFound();s.Name=input.Name.Trim();s.Path=input.Path;if(!input.Id.HasValue)db.Songs.Add(s);break;}
 case "artists":{if(string.IsNullOrWhiteSpace(input.Name))return BadRequest();var a=input.Id.HasValue?await db.Artists.FindAsync(input.Id):new Artist();if(a==null)return NotFound();a.Name=input.Name.Trim();a.Lastname=input.Lastname;a.Image=input.Image;if(!input.Id.HasValue)db.Artists.Add(a);break;}
 case "albums":{if(string.IsNullOrWhiteSpace(input.Title)||input.Year is <1000 or >9999)return BadRequest();var a=input.Id.HasValue?await db.Albums.FindAsync(input.Id):new Album();if(a==null)return NotFound();a.Title=input.Title.Trim();a.Year=input.Year;a.Cover=input.Image;if(!input.Id.HasValue)db.Albums.Add(a);break;}
 case "users":{if(!input.Id.HasValue||!input.Role.HasValue)return BadRequest();var u=await db.Users.FindAsync(input.Id);if(u==null)return NotFound();u.Role=input.Role.Value;break;}
 case "tracklists":{if(!await db.Songs.AnyAsync(s=>s.Id==input.SongId)||!await db.Albums.AnyAsync(a=>a.Id==input.AlbumId))return BadRequest();if(!await db.Tracklists.AnyAsync(t=>t.AlbumId==input.AlbumId&&t.SongId==input.SongId))db.Tracklists.Add(new Tracklist{SongId=input.SongId!.Value,AlbumId=input.AlbumId!.Value});break;}
 case "players":{if(!await db.Songs.AnyAsync(s=>s.Id==input.SongId)||!await db.Artists.AnyAsync(a=>a.Id==input.ArtistId))return BadRequest();if(!await db.Players.AnyAsync(p=>p.ArtistId==input.ArtistId&&p.SongId==input.SongId))db.Players.Add(new Player{SongId=input.SongId!.Value,ArtistId=input.ArtistId!.Value});break;}
 default:return NotFound();}
 await db.SaveChangesAsync();return Ok(new{saved=true});}
 [HttpDelete("{kind}/{id:int}")]public async Task<IActionResult> Delete(string kind,int id,[FromQuery]int? songId){
 switch(kind){case "songs":await db.Songs.Where(s=>s.Id==id).ExecuteDeleteAsync();break;case "artists":await db.Artists.Where(a=>a.Id==id).ExecuteDeleteAsync();break;case "albums":await db.Albums.Where(a=>a.Id==id).ExecuteDeleteAsync();break;case "tracklists":await db.Tracklists.Where(t=>t.AlbumId==id&&t.SongId==songId).ExecuteDeleteAsync();break;case "players":await db.Players.Where(p=>p.ArtistId==id&&p.SongId==songId).ExecuteDeleteAsync();break;default:return BadRequest(new{message="Elimina primero las relaciones. Los usuarios no se eliminan desde este panel."});}return NoContent();}
}
