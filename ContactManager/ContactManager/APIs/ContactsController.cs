using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ContactManager.Models;

namespace ContactManager.APIs
{
    public class ContactsController : ApiController
    {
        private readonly IContactRepository repository;

        public ContactsController(IContactRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<Contact> Get()
        {
            return repository.GetAll().AsQueryable();
        }

        public HttpResponseMessage<Contact> Post(Contact contact)
        {
            var rd = Request.GetRouteData();

            string uri = Url.Route(routeName: "Default", routeValues: new { controller = "Contacts", id = contact.ContactId });

            repository.Post(contact);
            var response = new HttpResponseMessage<Contact>(contact)
                            {
                                StatusCode = HttpStatusCode.Created
                            };

            return response;
        }
    }
}