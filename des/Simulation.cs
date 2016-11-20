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
            //init persons
            foreach (Model.Person person in Group.Members)
            {
                person.ReposterProbability = person.Contacts.Count / Group.Members.Count;
                person.NoOfRepostsPerRecv = (person.Contacts.Count * person.Contacts.Count) / Group.Members.Count;
                //TODO - init hated and loved ones
            }
            //TODO - send first mail

            int eventId = 0;
            Random rnd = new Random();
            do
            {
                foreach (Model.Person person in Group.Members)
                {
                    while (person.NoOfReceivedMails > 0)
                    {
                        if (person.Senders.Count > 0)
                        {
                            if (person.Senders.Dequeue() == person.LovedOne)
                            {
                                //TODO - rand new loved one, old one becomes hated.
                            }
                        }
                        //start sending
                        if (person.ReposterProbability > rnd.NextDouble() && person.NoOfRepostsPerRecv > 0)
                        {
                            int noOfRepostsLeft = person.NoOfRepostsPerRecv;
                            //send to hated one
                            person.HatedOne.NoOfReceivedMails++;
                            person.HatedOne.Senders.Enqueue(person);
                            System.Console.WriteLine(eventId.ToString() + " " + person.Id.ToString() + " " + person.HatedOne.Id.ToString());
                            eventId++;
                            noOfRepostsLeft--;
                            //more reposts
                            while (noOfRepostsLeft > 0)
                            {
                                //this assumes mail can be reposted multiple times to one person - this not disallowed
                                int target = rnd.Next(person.Contacts.Count);
                                if (person.Contacts.ToArray()[target] != person.LovedOne)
                                {
                                    person.Contacts.ToArray()[target].NoOfReceivedMails++;
                                    person.Contacts.ToArray()[target].Senders.Enqueue(person);
                                    System.Console.WriteLine(eventId.ToString() + " " + person.Id.ToString() + " " + person.Contacts.ToArray()[target].Id.ToString());
                                    eventId++;
                                    noOfRepostsLeft--;
                                }
                            }
                        }
                        else
                        {
                            System.Console.WriteLine(eventId.ToString() + " " + person.Id.ToString() + " !!!ignored !!!");
                            eventId++;
                        }
                        person.NoOfReceivedMails--;
                    }
                }
                //throw new NotImplementedException();
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
