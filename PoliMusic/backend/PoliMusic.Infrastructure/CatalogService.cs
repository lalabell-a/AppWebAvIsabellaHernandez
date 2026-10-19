using Microsoft.EntityFrameworkCore;
using PoliMusic.Application;
namespace PoliMusic.Infrastructure;
public class CatalogService(MusicDb db):ICatalogService{
 public async Task<object> Load(CancellationToken ct){
 var songs=await db.Songs.AsNoTracking().ToListAsync(ct);var albums=await db.Albums.AsNoTracking().ToListAsync(ct);var artists=await db.Artists.AsNoTracking().ToListAsync(ct);var tracks=await db.Tracklists.AsNoTracking().ToListAsync(ct);var players=await db.Players.AsNoTracking().ToListAsync(ct);
 var art=new[]{"rock","relax","retro","classic","country"};
 return new{songs=songs.Select(s=>new{id=s.Id,name=s.Name,path=s.Path,plays=s.Plays??0,artistIds=players.Where(p=>p.SongId==s.Id).Select(p=>p.ArtistId)}),albums=albums.Select((a,i)=>new{id=a.Id,title=a.Title,year=a.Year,songIds=tracks.Where(t=>t.AlbumId==a.Id).Select(t=>t.SongId),cover=art[i%5]}),artists=artists.Select(a=>new{id=a.Id,name=(a.Name+" "+a.Lastname).Trim(),image=""})};
 }
}
