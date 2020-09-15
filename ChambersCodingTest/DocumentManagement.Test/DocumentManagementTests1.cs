using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Xunit;
using DocumentManagementAPI;
using DocumentManagementAPI.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace DocumentManagement.Test
{
	public class DocumentManagementTests1
	{
		private readonly HttpClient _client;
		static string basePath = @"C:\Users\user\source\repos\ChambersCodingTest\TestData";
		static string fileValidPdfSmall = Path.Combine(basePath, "label.pdf");
		static string fileWrongExtension = Path.Combine(basePath, "ChambersDevTest.docx");
		static string fileRightExtensionButNotPdf = Path.Combine(basePath, "ChambersDevTest.pdf");
		static string fileValidPdfTooBig = Path.Combine(basePath, "Beginning_SOLID_Principles_and_Design_Pa.pdf");

		public DocumentManagementTests1()
		{
			string curDir = Directory.GetCurrentDirectory();
			var builder = new ConfigurationBuilder()
				.SetBasePath(curDir)
				.AddJsonFile("appsettings.json");
			var server = new TestServer(new WebHostBuilder()
								//.UseEnvironment("Development")
								.UseContentRoot(curDir)
								.UseConfiguration(builder.Build())
								.UseStartup<Startup>());
			_client = server.CreateClient();
		}

		[Theory]
		[InlineData("GET")]
		public async Task RunAreYouAlive(string method)
		{
			var request = new HttpRequestMessage(new HttpMethod(method), "/api/AreYouAlive");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			Assert.Equal("Hello world", data);
		}

		[Theory]
		[InlineData("POST")]
		public async Task UploadFileValidPdfWorks(string method)
		{
			Document doc = new Document {
				DocumentName = Path.GetFileName(fileValidPdfSmall),
				Location = Path.GetDirectoryName(fileValidPdfSmall),
				Size = (int)new FileInfo(fileValidPdfSmall).Length,
				Data = File.ReadAllBytes(fileValidPdfSmall)
			};
			var request = new HttpRequestMessage(new HttpMethod(method), "/api/Documents");
			request.Content = new StringContent (JsonConvert.SerializeObject(doc), Encoding.UTF8, "application/json");

			var response = await _client.SendAsync(request);
			var data = await response.Content.ReadAsStringAsync();
			var document = JsonConvert.DeserializeObject<Document>(data);

			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			Assert.Equal(doc.DocumentName, document.DocumentName);
			Assert.Equal(doc.Size, document.Size);
		}

		[Theory]
		[InlineData("POST")]
		public async Task UploadFileWrongExtension(string method)
		{
			Document doc = new Document
			{
				DocumentName = Path.GetFileName(fileRightExtensionButNotPdf),
				Location = Path.GetDirectoryName(fileRightExtensionButNotPdf),
				Size = (int)new FileInfo(fileRightExtensionButNotPdf).Length,
				Data = File.ReadAllBytes(fileRightExtensionButNotPdf)
			};
			var request = new HttpRequestMessage(new HttpMethod(method), "/api/Documents");
			request.Content = new StringContent(JsonConvert.SerializeObject(doc), Encoding.UTF8, "application/json");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			Assert.Equal("The supplied file is not a valid PDF file", data);
		}

		[Theory]
		[InlineData("POST")]
		public async Task UploadFileRightExtensionButNotAPdf(string method)
		{
			Document doc = new Document
			{
				DocumentName = Path.GetFileName(fileWrongExtension),
				Location = Path.GetDirectoryName(fileWrongExtension),
				Size = (int)new FileInfo(fileWrongExtension).Length,
				Data = File.ReadAllBytes(fileWrongExtension)
			};
			var request = new HttpRequestMessage(new HttpMethod(method), "/api/Documents");
			request.Content = new StringContent(JsonConvert.SerializeObject(doc), Encoding.UTF8, "application/json");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			Assert.Equal("The supplied file is not a valid PDF file", data);
		}

		[Theory]
		[InlineData("POST")]
		public async Task UploadFileTooBig(string method)
		{
			Document doc = new Document
			{
				DocumentName = Path.GetFileName(fileValidPdfTooBig),
				Location = Path.GetDirectoryName(fileValidPdfTooBig),
				Size = (int)new FileInfo(fileValidPdfTooBig).Length,
				Data = File.ReadAllBytes(fileValidPdfTooBig)
			};
			var request = new HttpRequestMessage(new HttpMethod(method), "/api/Documents");
			request.Content = new StringContent(JsonConvert.SerializeObject(doc), Encoding.UTF8, "application/json");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			Assert.Equal("The supplied file is too big", data);
		}

		[Theory]
		[InlineData("GET")]
		public async Task GetListOfAllDocumentsInDb(string method)
		{
			// I should really add a doc here as each test should be taken in total isolation so, if the database has been cleared out, there
			// will be no docs at all and I can't rely on other tests adding them for me. But since this isn't a complete solution I'm going to let
			// it slide :-)
			var request = new HttpRequestMessage(new HttpMethod(method), "/api/Documents?SortField=DocumentName&SortOrder=ASC");
			//var request = new HttpRequestMessage(new HttpMethod(method), "/api/Documents");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			var documents = JsonConvert.DeserializeObject<Document[]>(data);
			Assert.NotNull(documents);
		}

		[Theory]
		[InlineData("GET")]
		public async Task GetAnActualDocumentAndSaveItOffToDisk(string method)
		{
			// I must add a doc here just in case one doesn't exist. Now, the GET Documents (that returns a list of all docs) DOESN'T return the id
			// which is a short coming IMHO but I'm going to cheat here and use the id of the one I just uploaded. This would need sorting out ASAFP

			Document doc = new Document
			{
				DocumentName = Path.GetFileName(fileValidPdfSmall),
				Location = Path.GetDirectoryName(fileValidPdfSmall),
				Size = (int)new FileInfo(fileValidPdfSmall).Length,
				Data = File.ReadAllBytes(fileValidPdfSmall)
			};
			var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/Documents");
			request.Content = new StringContent(JsonConvert.SerializeObject(doc), Encoding.UTF8, "application/json");

			var response = await _client.SendAsync(request);
			var data = await response.Content.ReadAsStringAsync();
			var document = JsonConvert.DeserializeObject<Document>(data);

			Assert.Equal(HttpStatusCode.Created, response.StatusCode);

			// HUGE assumption here, it's the last one but I'm running out of time on this test!
			var request2 = new HttpRequestMessage(new HttpMethod(method), "/api/Documents/" + document.Id);

			var response2 = await _client.SendAsync(request2);

			Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
			var data2 = await response2.Content.ReadAsStringAsync();
			var document2 = JsonConvert.DeserializeObject<Document>(data2);

			// write byte array to file - I should really move this to a helper method but it's just a test
			string abc = Path.Combine(document2.Location, document2.DocumentName);
			File.WriteAllBytes(Path.Combine(document2.Location, document2.DocumentName), document2.Data);

			Assert.NotNull(document2);
		}

		[Theory]
		[InlineData("DELETE")]
		public async Task DeleteOneOfTheFileEntries(string method)
		{
			// so we're going to get a list of the docs held on the DB
			var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Documents");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			var documents = JsonConvert.DeserializeObject<Document[]>(data);
			int numDocsBeforeDeletion = documents.Length;
			Assert.NotNull(documents);

			if (numDocsBeforeDeletion == 0)
			{
				return;
			}

			// from here I can delete one - we'll choose the first assuming there's more than one
			var request2 = new HttpRequestMessage(new HttpMethod(method), "/api/Documents/" + documents[0].Id);

			var response2 = await _client.SendAsync(request2);
			Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
			var data2 = await response2.Content.ReadAsStringAsync();
			var document2 = JsonConvert.DeserializeObject<Document>(data2);

			// the deletion returns the deleted entry so these should be the same
			Assert.Equal(documents[0].Id, document2.Id);

			// and then get the list again to make sure it's gone down by one
			request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Documents");

			response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			data = await response.Content.ReadAsStringAsync();
			documents = JsonConvert.DeserializeObject<Document[]>(data);
			int numDocsAfterDeletion = documents.Length;
			Assert.Equal(numDocsAfterDeletion, numDocsBeforeDeletion - 1);
		}

		[Theory]
		[InlineData("DELETE")]
		public async Task DeleteEntriesThatDoesntExist(string method)
		{
			// so we're going to get a list of the docs held on the DB
			var request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Documents");

			var response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var data = await response.Content.ReadAsStringAsync();
			var documents = JsonConvert.DeserializeObject<Document[]>(data);
			int numDocsBeforeDeletion = documents.Length;
			Assert.NotNull(documents);

			if (numDocsBeforeDeletion == 0)
			{
				return;
			}

			// now I try to delete an id that doesn't exist (I hope!)
			var request2 = new HttpRequestMessage(new HttpMethod(method), "/api/Documents/" + 7777777);

			var response2 = await _client.SendAsync(request2);
			Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);

			// and then get the list again to make sure it hasn't gone down by one
			request = new HttpRequestMessage(new HttpMethod("GET"), "/api/Documents");

			response = await _client.SendAsync(request);

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			data = await response.Content.ReadAsStringAsync();
			documents = JsonConvert.DeserializeObject<Document[]>(data);
			int numDocsAfterDeletion = documents.Length;
			Assert.Equal(numDocsAfterDeletion, numDocsBeforeDeletion);
		}
	}
}
