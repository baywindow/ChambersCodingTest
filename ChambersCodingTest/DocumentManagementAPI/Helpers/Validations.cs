using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentManagementAPI.Settings;
using Microsoft.Extensions.Configuration;

namespace DocumentManagementAPI.Helpers
{
	public static class Validations
	{
		public static bool IsPDF(string filename, byte[] filedata)
		{
			// let's assume if it is a PDF then they've actually kept the extension as it should be otherwise it's bunk
			if (!Path.GetExtension(filename).Equals(".PDF", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}

			// the file extension might have been renamed to pdf so also check if it really is a PDF so the first four bytes must be "%PDF"
			// otherwise someone is tring to pull a fast one!
			if (filedata[0] != 0x25 ||
				filedata[1] != 0x50 ||
				filedata[2] != 0x44 ||
				filedata[3] != 0x46)
			{
				return false;
			}

			return true;
		}

		public static bool IsTheRightSize(long size)
		{
			return (size < ClientSettings.Instance.MaxPdfFileSize ? true : false);
		}
	}
}
