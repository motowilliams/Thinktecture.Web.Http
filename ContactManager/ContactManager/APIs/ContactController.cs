using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ContactManager.Models;

namespace ContactManager.APIs
{
    public class ContactController : ApiController
    {
        private readonly IContactRepository repository;

        public ContactController(IContactRepository repository)
        {
            this.repository = repository;            
        }

        public HttpResponseMessage<Contact> Get(int id)
        {
            var contact = repository.Get(id);

            if (contact == null)
            {
                var response = new HttpResponseMessage();
                response.StatusCode = HttpStatusCode.NotFound;
                response.Content = new StringContent("Contact not found");
                
                throw new HttpResponseException(response);
            }

            var contactResponse = new HttpResponseMessage<Contact>(contact);

            contactResponse.Content.Headers.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(300));
            
            return contactResponse;
        }
        
        public Contact Put(int id, Contact contact)
        {
            repository.Get(id);
            repository.Update(contact);
            
            return contact;
        }
        
        public Contact Delete(int id)
        {
            var deleted = repository.Get(id);
            repository.Delete(id);
            
            return deleted;
        }
    }
}
