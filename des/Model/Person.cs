using System.Collections.Generic;

namespace des.Model
{
    public class Person
    {
        public int Id { get; private set; }
        public IList<Person> Contacts { get; private set; }
        public Person LovedOne { get; private set; }
        public Person HatedOne { get; private set; }

        private Person()
        {
            Contacts = new List<Person>();
        }

        public Person(int id)
            : this()
        {
            Id = id;
        }

        public bool AddContact(Person person)
        {
            if (person.Id == Id) return false;

            Contacts.Add(person);
            return true;
        }
    }
}
