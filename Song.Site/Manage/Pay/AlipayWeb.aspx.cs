using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using WeiSha.Common;

using Song.ServiceInterfaces;
using Song.Entities;
using System.Text.RegularExpressions;



namespace Song.Site.Manage.Pay
{
    public partial class AlipayWeb : Extend.CustomPage
    {
        private int id = WeiSha.Common.Request.QueryString["id"].Decrypt().Int32 ?? 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                fill();
                Pai_Pattern.DdlInterFace.Enabled = false;
            } 
        }
        private void fill()
        {
            Song.Entities.PayInterface pi = id <= 0 ? null : Business.Do<IPayInterface>().PaySingle(id);
            if (pi == null) return;
            this.EntityBind(pi);
            //�Զ���������
            WeiSha.Common.CustomConfig config = CustomConfig.Load(pi.Pai_Config);
            tbPrivatekey.Text = config["Privatekey"].Value.String;           
        }
        /// <summary>
        /// �޸�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnter_Click(object sender, EventArgs e)
        {
            Song.Entities.PayInterface pi = id <= 0 ? new Song.Entities.PayInterface() : Business.Do<IPayInterface>().PaySingle(id);
            pi = this.EntityFill(pi) as Song.Entities.PayInterface;
            //֧����ʽ
            pi.Pai_Pattern = Pai_Pattern.DdlInterFace.SelectedItem.Text;
            //�Զ���������
            WeiSha.Common.CustomConfig config = CustomConfig.Load(pi.Pai_Config);
            string privateKey = Regex.Replace(tbPrivatekey.Text.Trim(), @"\r|\n|\s", "", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            config["Privatekey"].Text = privateKey;
            pi.Pai_Config = config.XmlString;
            //���õ�ƽ̨
            pi.Pai_Platform = "web";
            try
            {
                if (id <= 0)
                {
                    Business.Do<IPayInterface>().PayAdd(pi);
                }
                else
                {
                    Business.Do<IPayInterface>().PaySave(pi);
                }

                Master.AlertCloseAndRefresh("�����ɹ���");
            }
            catch (Exception ex)
            {
                Master.Alert(ex.Message);
            }
        }
    }
}