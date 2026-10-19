using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PoliMusic.Domain;
[Table("TBL_SONG")] public class Song {
[Key] [Column("ID_SONG")] public int Id {get;set;}
[Column("SONG_NAME")] public string Name {get;set;} = "";
[Column("SONG_PATH")] public string Path {get;set;} = "";
[Column("PLAYS")] public int? Plays {get;set;}
}
[Table("TBL_USER")] public class User {
[Key] [Column("ID_USER")] public int Id {get;set;}
[Column("USERNAME")] public string Username {get;set;} = "";
[Column("PASSWORD")] public string Password {get;set;} = "";
[Column("EMAIL")] public string Email {get;set;} = "";
[Column("BIRTHDAY")] public DateTime? Birthday {get;set;}
[Column("USER_PHOTO")] public string? Photo {get;set;}
[Column("USER_TYPE")] public int Role {get;set;}
}
[Table("TBL_ALBUM")] public class Album {
[Key] [Column("ID_ALBUM")] public int Id {get;set;}
[Column("TITLE_ALBUM")] public string Title {get;set;} = "";
[Column("RELEASE_YEAR")] public int? Year {get;set;}
[Column("ALBUM_IMAGE_PATH")] public string? Cover {get;set;}
}
[Table("TBL_ARTIST")] public class Artist {
[Key] [Column("ID_ARTIST")] public int Id {get;set;}
[Column("ARTIST_NAME")] public string Name {get;set;} = "";
[Column("ARTIST_LASTNAME")] public string? Lastname {get;set;}
[Column("ARTIST_IMAGE")] public string? Image {get;set;}
}
[Table("TBL_PLAYLIST")] public class Playlist {
[Key] [Column("ID_PLAYLIST")] public int Id {get;set;}
[Column("ID_USER")] public int UserId {get;set;}
[Column("TITLE")] public string Title {get;set;} = "";
[Column("CREATION_DATE")] public DateTime Created {get;set;}
[Column("TYPE")] public int Type {get;set;}
[Column("PHOTO")] public string? Photo {get;set;}
}
[Table("TBL_TRACKLIST")] public class Tracklist {
[Column("ID_ALBUM")] public int AlbumId {get;set;}
[Column("ID_SONG")] public int SongId {get;set;}
}
[Table("TBL_PLAYER")] public class Player {
[Column("ID_SONG")] public int SongId {get;set;}
[Column("ID_ARTIST")] public int ArtistId {get;set;}
}
[Table("TBL_PLAYLIST_SONG")] public class PlaylistSong {
[Column("ID_PLAYLIST")] public int PlaylistId {get;set;}
[Column("ID_SONG")] public int SongId {get;set;}
}
[Table("TBL_CATALOG")] public class Catalog {
[Key] [Column("ID_CATALOG")] public int Id {get;set;}
[Column("COD_CATALOG_PARENT")] public string? Parent {get;set;}
[Column("COD_CATALOG")] public string? Code {get;set;}
[Column("VALUE")] public string Value {get;set;} = "";
}
