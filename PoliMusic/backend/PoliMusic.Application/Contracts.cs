using System.ComponentModel.DataAnnotations;
namespace PoliMusic.Application;
public record AuthInput([Required,StringLength(50)]string Username,[Required,StringLength(128,MinimumLength=1)]string Password,[EmailAddress,StringLength(50)]string? Email);
public record ProfileDto(int Id,string Username,string Email,string? Birthday,string? Photo,int Role);
public record PlaylistInput([Required,StringLength(50,MinimumLength=1)]string Title,[Range(0,1)]int Type,[StringLength(255)]string? Photo);
public record ProfileInput([Required,StringLength(50)]string Username,[Required,EmailAddress,StringLength(50)]string Email,DateTime? Birthday,[StringLength(255)]string? Photo);
public record PlayInput(Guid EventId);
public interface ICatalogService{Task<object> Load(CancellationToken ct);}
