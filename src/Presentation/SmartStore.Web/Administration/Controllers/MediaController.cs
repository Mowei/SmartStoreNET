﻿using System;
using System.IO;
using System.Web.Mvc;
using System.Web.SessionState;
using SmartStore.Core;
using SmartStore.Services.Media;
using SmartStore.Services.Security;
using SmartStore.Web.Framework.Controllers;

namespace SmartStore.Admin.Controllers
{
	[SessionState(SessionStateBehavior.Disabled)]
	public partial class MediaController : AdminControllerBase
    {
		private readonly IPermissionService _permissionService;
		private readonly IMediaFileSystem _fileSystem;
		private readonly IWebHelper _webHelper;

		public MediaController(IPermissionService permissionService, IMediaFileSystem fileSystem, IWebHelper webHelper)
        {
			_permissionService = permissionService;
			_fileSystem = fileSystem;
            _webHelper = webHelper;
        }

		[HttpPost]
		public ActionResult UploadImageAjax()
		{
			var result = this.UploadImageInternal();
			return Json(result);
		}

		private UploadFileResult UploadImageInternal()
		{
			var postedFile = Request.ToPostedFileResult();
			if (postedFile == null)
			{
				return new UploadFileResult { Message = T("Common.NoFileUploaded") };
			}

			if (postedFile.FileName.IsEmpty())
			{
				return new UploadFileResult { Message = "No file name provided" };
			}

			if (!postedFile.IsImage)
			{
				return new UploadFileResult { Message = "Files with extension '{0}' cannot be uploaded".FormatInvariant(postedFile.FileExtension) };
			}

			var path = _fileSystem.Combine("Uploaded", postedFile.FileName);
			_fileSystem.SaveStream(path, postedFile.Stream);
			
			return new UploadFileResult
			{
				Success = true,
				Url = _fileSystem.GetPublicUrl(path)
			};
		}

		public class UploadFileResult
		{
			public bool Success { get; set; }
			public string Url { get; set; }
			public string Message { get; set; }
		}

    }
}
