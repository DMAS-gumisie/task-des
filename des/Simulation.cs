using des.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace des
{
    public class Simulation
    {
        private readonly Group Group = new Group();

        public void AddPerson(int personId, List<int> contactsIds)
        {
            var person = GetOrCreatePerson(personId);
            Group.AddMember(person);
            foreach (var contactId in contactsIds)
            {
                var contact = GetOrCreatePerson(contactId);
                person.AddContact(contact);
                Group.AddMember(contact);
            }
        }

        public void RunScenario(int firstSenderId, int firstReceiverId)
        {
            do
            {
                throw new NotImplementedException();
            } while (true);
        }

        private Person GetOrCreatePerson(int id)
        {
            var person = Group.Members.FirstOrDefault(p => p.Id == id);
            if (person == null)
            {
                person = new Person(id);
            }

            return person;
        }
    }
}
