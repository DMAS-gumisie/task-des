using des.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace des
{
    public class Simulation
    {
        private readonly Random rnd = new Random(DateTime.UtcNow.Millisecond);
        private readonly Group Group = new Group();
        private int eventId = 0;

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
            // init persons
            InitPersons();

            // send first mail based on the scenario
            var firstSender = GetPerson(firstSenderId);
            var firstReceiver = GetPerson(firstReceiverId);
            firstReceiver.NoOfReceivedMails = 1;
            firstReceiver.Senders.Enqueue(firstSender);

            // "main" loop
            do
            {
                foreach (var person in Group.Members)
                {
                    // sleep, improves behavior of random no generator
                    System.Threading.Thread.Sleep(rnd.Next(50));

                    // handle received emails
                    while (person.NoOfReceivedMails > 0)
                    {
                        // update hated and loved people if needed
                        if (person.Senders.Any() && person.Senders.Dequeue() == person.LovedOne) // FIXME: shouldn't work like that
                        {
                            person.HatedOne = person.LovedOne;
                            var lovedCandidates = person.Contacts.Except(person.HatedOne);
                            person.LovedOne = lovedCandidates.Random(rnd);
                        }

                        // send to other people
                        double tmpRnd = rnd.NextDouble();
                        if (person.ReposterProbability > tmpRnd && person.NoOfRepostsPerRecv > 0)
                        {
                            int noOfRepostsLeft = person.NoOfRepostsPerRecv;

                            // always send to hated one
                            Send(person, person.HatedOne);
                            noOfRepostsLeft--;

                            // more reposts
                            var notLovedContacts = person.Contacts.Except(person.LovedOne).ToList();
                            while (noOfRepostsLeft > 0 && notLovedContacts.Any())
                            {
                                var target = notLovedContacts.Random(rnd);
                                Send(person, target);
                                noOfRepostsLeft--;
                                notLovedContacts.Remove(target);
                            }
                        }
                        else
                        {
                            Console.WriteLine("{0} {1} {2}", eventId, person.Id, "!!! ignored !!!");
                            eventId++;
                        }

                        person.NoOfReceivedMails--;
                    }
                }
            } while (true);
        }

        private void InitPersons()
        {
            foreach (var person in Group.Members)
            {
                person.ReposterProbability = (double)person.Contacts.Count / Group.Members.Count;
                person.NoOfRepostsPerRecv = (person.Contacts.Count * person.Contacts.Count) / Group.Members.Count;

                // init loved and hated one
                person.LovedOne = person.Contacts.Random(rnd);
                var hatedCandidates = person.Contacts.Except(person.LovedOne);
                person.HatedOne = hatedCandidates.Random(rnd);
            }
        }

        private void Send(Person from, Person to)
        {
            to.NoOfReceivedMails++;
            to.Senders.Enqueue(from);
            Console.WriteLine("{0} {1} {2}", eventId, from.Id, to.Id);
            eventId++;
        }

        private Person GetPerson(int id)
        {
            return Group.Members.FirstOrDefault(p => p.Id == id);
        }

        private Person GetOrCreatePerson(int id)
        {
            var person = GetPerson(id);
            if (person == null)
            {
                person = new Person(id);
            }

            return person;
        }
    }
}
