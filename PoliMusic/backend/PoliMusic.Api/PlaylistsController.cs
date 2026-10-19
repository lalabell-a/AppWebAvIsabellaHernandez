using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PoliMusic.Application;
using PoliMusic.Infrastructure;
using PoliMusic.Domain;
namespace PoliMusic.Api;
[Authorize,ApiController,Route("api/playlists")]
public class PlaylistsController(MusicDb db):ControllerBase{
 int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 bool CanEdit(Playlist p)=>p.UserId==UserId||User.IsInRole("Admin");
 async Task<object> Dto(Playlist p)=>new{id=p.Id,userId=p.UserId,title=p.Title,type=p.Type,photo=p.Photo??"",created=p.Created,songIds=await db.PlaylistSongs.Where(x=>x.PlaylistId==p.Id).Select(x=>x.SongId).ToListAsync()};
 bool Valid(PlaylistInput p)=>!string.IsNullOrWhiteSpace(p.Title)&&(string.IsNullOrEmpty(p.Photo)||Uri.TryCreate(p.Photo,UriKind.Absolute,out var u)&&u.Scheme=="https");
 [HttpGet]public async Task<IActionResult> List(){var values=await db.Playlists.Where(p=>p.UserId==UserId).ToListAsync();var result=new List<object>();foreach(var p in values)result.Add(await Dto(p));return Ok(result);}
 [HttpGet("{id:int}")]public async Task<IActionResult> Get(int id){var p=await db.Playlists.FindAsync(id);if(p==null)return NotFound();if(p.Type==1&&!CanEdit(p))return Forbid();return Ok(await Dto(p));}
 [HttpPost]public async Task<IActionResult> Create(PlaylistInput input){if(!Valid(input))return BadRequest(new{message="Revisa el título y la URL HTTPS de la imagen."});var p=new Playlist{Title=input.Title.Trim(),Type=input.Type,Photo=input.Photo,UserId=UserId,Created=DateTime.UtcNow};db.Playlists.Add(p);await db.SaveChangesAsync();return Ok(await Dto(p));}
 [HttpPut("{id:int}")]public async Task<IActionResult> Update(int id,PlaylistInput input){if(!Valid(input))return BadRequest();var p=await db.Playlists.FindAsync(id);if(p==null)return NotFound();if(!CanEdit(p))return Forbid();p.Title=input.Title.Trim();p.Type=input.Type;p.Photo=input.Photo;await db.SaveChangesAsync();return Ok(await Dto(p));}
 [HttpDelete("{id:int}")]public async Task<IActionResult> Delete(int id){var p=await db.Playlists.FindAsync(id);if(p==null)return NotFound();if(!CanEdit(p))return Forbid();await using var tx=await db.Database.BeginTransactionAsync();await db.PlaylistSongs.Where(x=>x.PlaylistId==id).ExecuteDeleteAsync();db.Playlists.Remove(p);await db.SaveChangesAsync();await tx.CommitAsync();return NoContent();}
 [HttpPost("{id:int}/songs/{songId:int}")]public async Task<IActionResult> AddSong(int id,int songId){var p=await db.Playlists.FindAsync(id);if(p==null)return NotFound();if(!CanEdit(p))return Forbid();if(!await db.Songs.AnyAsync(s=>s.Id==songId))return NotFound();if(!await db.PlaylistSongs.AnyAsync(x=>x.PlaylistId==id&&x.SongId==songId)){db.PlaylistSongs.Add(new PlaylistSong{PlaylistId=id,SongId=songId});await db.SaveChangesAsync();}return NoContent();}
 [HttpDelete("{id:int}/songs/{songId:int}")]public async Task<IActionResult> RemoveSong(int id,int songId){var p=await db.Playlists.FindAsync(id);if(p==null)return NotFound();if(!CanEdit(p))return Forbid();await db.PlaylistSongs.Where(x=>x.PlaylistId==id&&x.SongId==songId).ExecuteDeleteAsync();return NoContent();}
}
