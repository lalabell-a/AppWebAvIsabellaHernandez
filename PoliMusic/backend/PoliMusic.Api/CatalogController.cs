using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PoliMusic.Application;
using PoliMusic.Infrastructure;
using System.Security.Claims;
namespace PoliMusic.Api;
[ApiController,Route("api")]
public class CatalogController(ICatalogService service,MusicDb db,IConfiguration config):ControllerBase{
 [HttpGet("catalog")]public async Task<object> Catalog(CancellationToken ct)=>await service.Load(ct);
 [HttpGet("media/song/{id:int}")]public async Task<IActionResult> Audio(int id){var song=await db.Songs.FindAsync(id);if(song==null)return NotFound();var root=Path.GetFullPath(config["MediaRoot"]??"../public/resources");var relative=song.Path.Replace("~/","").Replace('\\','/').ToLowerInvariant();var path=Path.GetFullPath(Path.Combine(root,relative));if(!path.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.Ordinal)||!System.IO.File.Exists(path))return NotFound(new{message="Este audio no ha sido cargado al servidor."});return PhysicalFile(path,"audio/mpeg",enableRangeProcessing:true);}
 // Playback receipts use server timestamps and a unique event ID. These operations
 // require migration 002. Client metadata alone never increments PLAYS.
 [Authorize,HttpPost("songs/{id:int}/start")]public async Task<IActionResult> Start(int id){if(!await db.Songs.AnyAsync(s=>s.Id==id))return NotFound();var eventId=Guid.NewGuid();var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO dbo.TBL_PLAY_EVENT(EVENT_ID,ID_USER,ID_SONG,STARTED_AT,COUNTED) VALUES ({eventId},{userId},{id},SYSUTCDATETIME(),0)");return Ok(new{eventId});}
 [Authorize,HttpPost("songs/{id:int}/plays")]public async Task<IActionResult> Played(int id,PlayInput input){var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);var changed=await db.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.TBL_PLAY_EVENT SET COUNTED=1 WHERE EVENT_ID={input.EventId} AND ID_USER={userId} AND ID_SONG={id} AND COUNTED=0 AND STARTED_AT<=DATEADD(second,-30,SYSUTCDATETIME()) AND STARTED_AT>DATEADD(hour,-2,SYSUTCDATETIME())");if(changed==1)await db.Songs.Where(s=>s.Id==id).ExecuteUpdateAsync(x=>x.SetProperty(s=>s.Plays,s=>(s.Plays??0)+1));await tx.CommitAsync();return Ok(new{counted=changed==1});}
}
