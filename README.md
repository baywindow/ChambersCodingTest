Comments begin with ">>>>>"

The test
 
A new requirement has come in for a small document management solution.
 
As a publisher
I would like to upload, manually re-order, download and delete pdf's
So, I can place a list of documents on my client apps and website for users to download
And in an arbitrary order of my choosing

>>>>> I didn't really understand the ordering requirement, the files are the files but how you view them I would normally leave up to the UI.
>>>>> I had to delete loads of files to get it into Git but a rebuild sorts it out. You can recreate the DB publishing off the DocumentManagementDB but I'm sure you'll have one fit for purpose. IT's .net core and SQL BTW
 
Acceptance criteria
 
1. Given I have a PDF to upload
When I send the PDF to the API
Then it is uploaded successfully
>>>>> done
 
2. Given I have a non-pdf to upload
When I send the non-pdf to the API
Then the API does not accept the file and returns the appropriate messaging and status
>>>>> done
 
3. Given I have a max pdf size of 5MB
When I send the pdf to the API
Then the API does not accept the file and returns the appropriate messaging and status
>>>>> done
 
4. Given I call the new document service API
When I call the API to get a list of documents
Then a list of PDFs’ is returned with the following properties: name, location, file-size
>>>>> Done except I also had to return the Id for use elsewhere. 
 
5. Given I have a list of PDFs’
When I choose to re-order the list of PDFs’
Then the list of PDFs’ is returned in the new order for subsequent calls to the API
>>>>> Not done, I started doing this and you'll see a table to persist the values and comments in the code of where it would probably be applied .... but really? You expect this to be done in an hour????
 
6. Given I have chosen a PDF from the list API
When I request the location for one of the PDF's
The PDF is downloaded
>>>>> Sort of done, I'd missed the location bit so the file is retrieved using it's Id
 
7. Given I have selected a PDF from the list API that I no longer require
When I request to delete the PDF
Then the PDF is deleted and will no longer return from the list API and can no longer be downloaded from its location directly
>>>>> done
 
8. Given I attempt to delete a file that does not exist
When I request to delete the non-existing pdf
Then the API returns an appropriate response
 >>>>> done - although it just comes up with a Not Found and not a more appropriate message

How we verify your submission
 
Please provide a GIT repository where we can clone your solution.
 
It is important the API is a working solution that we can run from our local development machines, we will try out your solution with real files.
 
Ideally build this in .net core as this represents most of our new work.
 
Ideally use any of the following data-stores:
Cosmos (emulator)
Azure Storage (blobs/tables/etc.) (emulator)
SQL
If you choose a different store, please provide instructions on installation (it must be a safe installation of a well-known technology and with a free to use licence for this purpose).
 
We will be checking for code quality so code this as you would for a production system.
 
Prioritise your hour to ensure you show us the features/code you really want us to see.
