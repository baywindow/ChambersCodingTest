using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentManagementAPI.Data;
using DocumentManagementAPI.Models;
using DocumentManagementAPI.Helpers;
using System.Net.Http;
using DocumentManagementAPI.ViewModels;
using System.Web;

namespace DocumentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentManagementAPIContext _context;

        public DocumentsController(DocumentManagementAPIContext context)
        {
            _context = context;
        }

        // GET: api/Documents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewDocument>>> GetDocument()
        {
			// sortField and sortOrder may or may not be in the request, if they are then we want to use their values and update the SortField table
			// so we'll keep using them
			string sortField = "";
			string sortOrder = "";
			var query = HttpUtility.ParseQueryString(ControllerContext.HttpContext.Request.QueryString.ToString());
			if (query != null && query.Count > 0)
			{
				sortField = query.Get("SortField"); // should be the name of the Document field
				sortOrder = query.Get("SortOrder"); // should be "ASC" or "DESC"
			}
			if (string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
			{
				// use the values in the DB
			}
			else
			{
				// update the values in the DB
			}

			List<ViewDocument> output = new List<ViewDocument>();
			foreach(var doc in await _context.Document.ToListAsync())
			{
				output.Add(new ViewDocument { Id = doc.Id, DocumentName = doc.DocumentName, Location = doc.Location, Size = doc.Size });
			}

			// right, I added a new table called SortField, this is where it would be used to sort the output. Which won't be easy as they're strings
			// but I'm sure there'll be a way of doing it using dynamic linq or something!
			
			return output;
        }

        // GET: api/Documents/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> GetDocument(int id)
        {
            var document = await _context.Document.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            return document;
        }

        // PUT: api/Documents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocument(int id, Document document)
        {
            if (id != document.Id)
            {
                return BadRequest();
            }

            _context.Entry(document).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Documents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Document>> PostDocument(Document document)
        {
			if (!Validations.IsPDF(document.DocumentName, document.Data))
			{
				return BadRequest("The supplied file is not a valid PDF file");
			}

			if (!Validations.IsTheRightSize(document.Size))
			{
				return BadRequest("The supplied file is too big");
			}

			// I would personally check for duplicate file names here and if they exist either 1. reject the post or 2. do something like file
			// explorer and change the name to "<name> - Copy(#)". Not going to do though
			_context.Document.Add(document);

            await _context.SaveChangesAsync();

			return CreatedAtAction("PostDocument", new { id = document.Id }, document);
        }

		// DELETE: api/Documents/5
		[HttpDelete("{id}")]
        public async Task<ActionResult<Document>> DeleteDocument(int id)
        {
            var document = await _context.Document.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            _context.Document.Remove(document);
            await _context.SaveChangesAsync();

            return document;
        }

        private bool DocumentExists(int id)
        {
            return _context.Document.Any(e => e.Id == id);
        }
    }
}
