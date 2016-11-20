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
            int eventId = 0;
            Random rnd = new Random(DateTime.UtcNow.Millisecond);
            //init persons
            foreach (Model.Person person in Group.Members)
            {
                person.ReposterProbability = (double)person.Contacts.Count / (double)Group.Members.Count;
                person.NoOfRepostsPerRecv = (person.Contacts.Count * person.Contacts.Count) / Group.Members.Count;
                int target = rnd.Next(1, person.Contacts.Count + 1);
                //init loved and hated one
                person.LovedOne = Group.Members.First(per => per.Id == target);
                //we don't believe in love-hate relationships
                while (true)
                {
                    target = rnd.Next(1, person.Contacts.Count + 1);
                    if (person.LovedOne != Group.Members.First(per => per.Id == target))
                    {
                        person.HatedOne = Group.Members.First(per => per.Id == target);
                        break;
                    }
                }
            }
            //first mail
            Group.Members.First(per => per.Id == firstReceiverId).NoOfReceivedMails = 1;
            Person firstSender = Group.Members.First(per => per.Id == firstSenderId);
            Group.Members.First(per => per.Id == firstReceiverId).Senders.Enqueue(firstSender);
            //"main" loop
            do
            {
                foreach (Model.Person person in Group.Members)
                {
                    //sleep, improves behavior of random no generator.
                    System.Threading.Thread.Sleep(rnd.Next(50));
                    //handle rejection
                    while (person.NoOfReceivedMails > 0)
                    {
                        if (person.Senders.Count > 0)
                        {
                            if (person.Senders.Dequeue() == person.LovedOne)
                            {
                                person.HatedOne = person.LovedOne;
                                while (true)
                                {
                                    int target = rnd.Next(1, person.Contacts.Count + 1);
                                    if (Group.Members.First(per => per.Id == target) != person.HatedOne)
                                    {
                                        person.LovedOne = person.Contacts.ToArray()[target];
                                        break;
                                    }
                                }
                            }
                        }
                        //start sending
                        double tmpRnd = rnd.NextDouble();
                        if (person.ReposterProbability > tmpRnd && person.NoOfRepostsPerRecv > 0)
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
                                int target = rnd.Next(1, person.Contacts.Count + 1);
                                if (Group.Members.First(per => per.Id == target) != person.LovedOne)
                                {
                                    Group.Members.First(per => per.Id == target).NoOfReceivedMails++;
                                    Group.Members.First(per => per.Id == target).Senders.Enqueue(person);
                                    System.Console.WriteLine(eventId.ToString() + " " + person.Id.ToString() + " " + Group.Members.First(per => per.Id == target).Id.ToString());
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
