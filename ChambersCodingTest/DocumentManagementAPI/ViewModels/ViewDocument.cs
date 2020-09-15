using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementAPI.ViewModels
{
	public class ViewDocument
	{
		public int Id { get; set; }
		public string DocumentName { get; set; }
		public string Location { get; set; }
		public int Size { get; set; }
	}
}
