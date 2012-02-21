﻿using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ContactManager.Models;
using Thinktecture.Web.Http.Filters;

namespace ContactManager.APIs
{
    public class ContactsController : ApiController
    {
        private readonly IContactRepository repository;

        public ContactsController(IContactRepository repository)
        {
            this.repository = repository;
        }

        [EnableCors]
        public IQueryable<Contact> Get()
        {
            return repository.GetAll().AsQueryable();
        }

        public HttpResponseMessage<Contact> Post(Contact contact)
        {            
            repository.Post(contact);
            
            var response = new HttpResponseMessage<Contact>(contact)
                            {
                                StatusCode = HttpStatusCode.Created
                            };
            response.Headers.Location = new Uri(
                ControllerContext.Request.RequestUri.LocalPath + "/" + contact.Id.ToString(CultureInfo.InvariantCulture), UriKind.Relative);
            
            return response;
        }
    }
}