using System;
using System.Collections.Generic;
using System.Text;

namespace MicrosoftAgentFramework.VeilleTechno.Deconstruction_Object
{
    public  static class Deconst_Object
    {
        public static async Task RunSample()
        {
             var newCustomer = new Customer("John", "Doe", 30);
            var (firstName, lastName, age) = newCustomer;
        }

      
    }

    public class Customer
    {
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; }

        public Customer(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }

        public void Deconstruct(out string firstName, out string lastName, out int age)
        {
            firstName = FirstName;
            lastName = LastName;
            age = Age;
        }
    }
}
