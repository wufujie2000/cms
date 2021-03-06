﻿using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationUploadVideo : BasePageCms
    {
		public TextBox TbVideoUploadDirectoryName;
		public DropDownList DdlVideoUploadDateFormatString;
		public DropDownList DdlIsVideoUploadChangeFileName;
        public TextBox TbVideoUploadTypeCollection;
        public DropDownList DdlVideoUploadTypeUnit;
        public TextBox TbVideoUploadTypeMaxSize;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            TbVideoUploadDirectoryName.Text = SiteInfo.Extend.VideoUploadDirectoryName;

            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年存入不同目录(不推荐)", EDateFormatTypeUtils.GetValue(EDateFormatType.Year)));
            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年/月存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)));
            DdlVideoUploadDateFormatString.Items.Add(new ListItem("按年/月/日存入不同目录", EDateFormatTypeUtils.GetValue(EDateFormatType.Day)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlVideoUploadDateFormatString, SiteInfo.Extend.VideoUploadDateFormatString);

            EBooleanUtils.AddListItems(DdlIsVideoUploadChangeFileName, "自动修改文件名", "保持文件名不变");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsVideoUploadChangeFileName, SiteInfo.Extend.IsVideoUploadChangeFileName.ToString());

            TbVideoUploadTypeCollection.Text = SiteInfo.Extend.VideoUploadTypeCollection.Replace("|", ",");
            var mbSize = GetMbSize(SiteInfo.Extend.VideoUploadTypeMaxSize);
            if (mbSize == 0)
            {
                DdlVideoUploadTypeUnit.SelectedIndex = 0;
                TbVideoUploadTypeMaxSize.Text = SiteInfo.Extend.VideoUploadTypeMaxSize.ToString();
            }
            else
            {
                DdlVideoUploadTypeUnit.SelectedIndex = 1;
                TbVideoUploadTypeMaxSize.Text = mbSize.ToString();
            }
        }

		private static int GetMbSize(int kbSize)
		{
			var retval = 0;
			if (kbSize >= 1024 && ((kbSize % 1024) == 0))
			{
				retval = kbSize / 1024;
			}
			return retval;
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                SiteInfo.Extend.VideoUploadDirectoryName = TbVideoUploadDirectoryName.Text;

                SiteInfo.Extend.VideoUploadDateFormatString = EDateFormatTypeUtils.GetValue(EDateFormatTypeUtils.GetEnumType(DdlVideoUploadDateFormatString.SelectedValue));
                SiteInfo.Extend.IsVideoUploadChangeFileName = TranslateUtils.ToBool(DdlIsVideoUploadChangeFileName.SelectedValue);

                SiteInfo.Extend.VideoUploadTypeCollection = TbVideoUploadTypeCollection.Text.Replace(",", "|");
                var kbSize = int.Parse(TbVideoUploadTypeMaxSize.Text);
                SiteInfo.Extend.VideoUploadTypeMaxSize = (DdlVideoUploadTypeUnit.SelectedIndex == 0) ? kbSize : 1024 * kbSize;
				
				try
				{
                    DataProvider.Site.Update(SiteInfo);

                    AuthRequest.AddSiteLog(SiteId, "修改视频上传设置");

                    SuccessMessage("上传视频设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "上传视频设置修改失败！");
				}
			}
		}

	}
}
