using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DocumentManagementAPI.Models;

namespace DocumentManagementAPI.Data
{
    public class DocumentManagementAPIContext : DbContext
    {
        public DocumentManagementAPIContext (DbContextOptions<DocumentManagementAPIContext> options)
            : base(options)
        {
        }

        public DbSet<DocumentManagementAPI.Models.Document> Document { get; set; }
    }
}
