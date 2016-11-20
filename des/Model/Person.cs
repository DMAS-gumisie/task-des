using System.Collections.Generic;

namespace des.Model
{
    public class Person
    {
        public int Id { get; private set; }
        public IList<Person> Contacts { get; private set; }
        public Person LovedOne { get; set; }
        public Person HatedOne { get; set; }
        public int NoOfReceivedMails { get; set; }
        public Queue<Person> Senders { get; private set; }
        public double ReposterProbability { get; set; }
        public int NoOfRepostsPerRecv { get; set; }

        private Person()
        {
            Contacts = new List<Person>();
            Senders = new Queue<Person>();
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
