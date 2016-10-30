using System.Collections.Generic;
using System.Linq;

namespace des.Model
{
    public class Group
    {
        public ISet<Person> Members { get; private set; }

        public Group()
        {
            Members = new HashSet<Person>();
        }

        public void AddMember(Person person)
        {
            Members.Add(person); // check for duplicates is not needed
        }
    }
}
