using System.Collections.Generic;
using System.Linq;

namespace ContactManager.Models
{
    public class ContactRepository : IContactRepository
    {
        private int nextContactId;

        private readonly IList<Contact> contacts;

        public ContactRepository()
        {
            contacts = new List<Contact>();

            contacts.Add(new Contact { ContactId = 1, Name = "Ingo Rammer", Address = "Rammer Way", City = "Landshut", State = "N/A", Zip = "12345", Email = "ingo.rammer@thinktecture.com" });
            contacts.Add(new Contact { ContactId = 2, Name = "Christian Weyer", Address = "Wire Way", City = "Neustadt", State = "N/A", Zip = "23456", Email = "christian.weyer@thinktecture.com" });
            contacts.Add(new Contact { ContactId = 3, Name = "Dominick Baier", Address = "Bavarian Way", City = "Heidelberg", State = "N/A", Zip = "34567", Email = "dominick.baier@thinktecture.com" });
            contacts.Add(new Contact { ContactId = 4, Name = "Christian Nagel", Address = "Nail Way", City = "Wien", State = "N/A", Zip = "45678", Email = "christian.nagel@thinktecture.com" });
            contacts.Add(new Contact { ContactId = 5, Name = "Jörg Neumann", Address = "Newman Way", City = "Hamburg", State = "N/A", Zip = "56789", Email = "joerg.neumann@thinktecture.com" });
            contacts.Add(new Contact { ContactId = 6, Name = "Oliver Sturm", Address = "Storm Way", City = "Somewhere in Scotland", State = "N/A", Zip = "67890", Email = "oliver.sturm@thinktecture.com" });
            contacts.Add(new Contact { ContactId = 7, Name = "Richard Blewett", Address = "Blewett Way", City = "Somwhere in England", State = "N/A", Zip = "78901", Email = "richard.blewett@thinktecture.com" });
            
            nextContactId = contacts.Count + 1;
        }

        public void Update(Contact updatedContact)
        {
            var contact = Get(updatedContact.ContactId);
            contact.Name = updatedContact.Name;
            contact.Address = updatedContact.Address;
            contact.City = updatedContact.City;
            contact.State = updatedContact.State;
            contact.Zip = updatedContact.Zip;
            contact.Email = updatedContact.Email;
            contact.Twitter = updatedContact.Twitter;
        }

        public Contact Get(int id)
        {
            return contacts.SingleOrDefault(c => c.ContactId == id);
        }

        public List<Contact> GetAll()
        {
            return contacts.ToList();
        }

        public void Post(Contact contact)
        {
            contact.ContactId = nextContactId++;
            contacts.Add(contact);
        }

        public void Delete(int id)
        {
            var contact = Get(id);
            contacts.Remove(contact);
        }
    }
}