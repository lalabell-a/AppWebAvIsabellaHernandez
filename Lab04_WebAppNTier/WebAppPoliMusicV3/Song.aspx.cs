using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Web;
using System.Web.UI.WebControls;
namespace WebAppPoliMusicV3
{
    public partial class Song : Util.AuthPage
    {
        protected void Page_Load(object sender,EventArgs e)
        {
            
            if(!IsPostBack) BindSongs();
        }
        private void BindSongs()
        {
            try
            {
                DataTable table=new BLL.PoliMusicV3.Song(ConfigurationManager.ConnectionStrings["BDD_PoliMusicConnectionString"].ConnectionString).Read(txtSearch.Text);
                gvSongs.DataSource=table; gvSongs.DataBind(); litCount.Text=table.Rows.Count.ToString(); lblMessage.Text=string.Empty;
                if(gvSongs.HeaderRow!=null) gvSongs.HeaderRow.TableSection=TableRowSection.TableHeader;
            }
            catch(DbException) { gvSongs.DataSource=null; gvSongs.DataBind(); litCount.Text="0"; lblMessage.Text="No se pudo consultar la biblioteca. Revisa la conexión a SQL Server e intenta de nuevo."; }
        }
        protected void btnSearch_Click(object sender,EventArgs e) { gvSongs.PageIndex=0; BindSongs(); }
        protected void btnClear_Click(object sender,EventArgs e) { txtSearch.Text=string.Empty; gvSongs.PageIndex=0; BindSongs(); }
        protected void gvSongs_PageIndexChanging(object sender,GridViewPageEventArgs e) { gvSongs.PageIndex=e.NewPageIndex; BindSongs(); }
        protected string AudioUrl(object id) { return ResolveUrl("~/Media.ashx?id="+Convert.ToInt32(id).ToString()); }
        protected string AudioLabel(object name) { return HttpUtility.HtmlAttributeEncode("Reproducir "+Convert.ToString(name)); }
    }
}
