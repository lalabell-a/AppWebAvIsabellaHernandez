using Microsoft.EntityFrameworkCore;
using PoliMusic.Domain;
namespace PoliMusic.Infrastructure;
public class MusicDb(DbContextOptions<MusicDb> options):DbContext(options){
 public DbSet<Song> Songs=>Set<Song>(); public DbSet<User> Users=>Set<User>(); public DbSet<Album> Albums=>Set<Album>(); public DbSet<Artist> Artists=>Set<Artist>(); public DbSet<Playlist> Playlists=>Set<Playlist>(); public DbSet<Tracklist> Tracklists=>Set<Tracklist>(); public DbSet<Player> Players=>Set<Player>(); public DbSet<PlaylistSong> PlaylistSongs=>Set<PlaylistSong>(); public DbSet<Catalog> Catalogs=>Set<Catalog>();
 protected override void OnModelCreating(ModelBuilder b){
 b.Entity<Tracklist>().HasKey(x=>new{x.AlbumId,x.SongId});b.Entity<Player>().HasKey(x=>new{x.SongId,x.ArtistId});b.Entity<PlaylistSong>().HasKey(x=>new{x.PlaylistId,x.SongId});
 b.Entity<Tracklist>().HasOne<Album>().WithMany().HasForeignKey(x=>x.AlbumId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<Tracklist>().HasOne<Song>().WithMany().HasForeignKey(x=>x.SongId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<Player>().HasOne<Song>().WithMany().HasForeignKey(x=>x.SongId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<Player>().HasOne<Artist>().WithMany().HasForeignKey(x=>x.ArtistId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<Playlist>().HasOne<User>().WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<PlaylistSong>().HasOne<Playlist>().WithMany().HasForeignKey(x=>x.PlaylistId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<PlaylistSong>().HasOne<Song>().WithMany().HasForeignKey(x=>x.SongId).OnDelete(DeleteBehavior.Restrict);
 b.Entity<User>().Property(x=>x.Password).HasMaxLength(255);b.Entity<User>().HasIndex(x=>x.Username).IsUnique();b.Entity<User>().HasIndex(x=>x.Email).IsUnique();
 }
}
