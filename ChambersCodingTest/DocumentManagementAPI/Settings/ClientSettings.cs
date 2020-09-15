using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementAPI.Settings
{
	// yay - I had to get at least one design pattern in here. Today we see the Singleton
	public class ClientSettings
	{
		private static ClientSettings _instance = null;
		private static readonly object lockObj = new object();

		private ClientSettings()
		{
		}

		public static ClientSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (lockObj)
					{
						if (_instance == null)
						{
							_instance = new ClientSettings();
						}
					}
				}
				return _instance;
			}
		}

		public long MaxPdfFileSize { get; set; }
	}
}
