using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CacheAside
{
    class UserInterface
    {
        //static ICacheManager<object> cache;
        DataSource dataSource;

        public UserInterface()
        {
            dataSource = new DataSource();

            //cache = CacheFactory.Build("getStartedCache", settings =>
            //{
            //    settings.WithSystemRuntimeCacheHandle("handleName");
            //});
        }

        public void Start()
        {
            while (true)
            {
                Console.WriteLine("");
                Console.WriteLine("Select an option (type 1, 2, 3, or 4 and hit enter): ");
                Console.WriteLine("  1. Query value");
                Console.WriteLine("  2. Update value");
                Console.WriteLine("  3. List values");
                Console.WriteLine("  4. Clear console");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        QueryValue();
                        break;
                    case "2":
                        UpdateValue();
                        break;
                    case "3":
                        ListAllValues();
                        break;
                    case "4":
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Input not recognized. Hit enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }


        private void QueryValue()
        {
            string key;

            while (true)
            {
                Console.WriteLine("Enter a key to query: ");
                key = Console.ReadLine();

                if (ValidateInput(key))
                    break;
            }

            var value = dataSource.Get(key);

            if (value != null)
                Console.WriteLine(value.ToString());
            else
                Console.WriteLine("Query did not find a cache or datastore value.");
        }

        private void UpdateValue()
        {
            string key;
            string value;

            while (true)
            {
                Console.WriteLine("Enter a key to update: ");
                key = Console.ReadLine();

                if (ValidateInput(key))
                    break;

                if (dataSource.ValueExists(key))
                    break;
                else
                {
                    Console.WriteLine("Key does not exist. Press enter to continue.");
                    Console.ReadLine();
                }
                    
            }

            while (true)
            {
                Console.WriteLine("Enter a new value: ");
                value = Console.ReadLine();

                if (ValidateInput(value))
                    break;
            }

            if (dataSource.Update(key, value))
                Console.WriteLine(value.ToString());
            else
                Console.WriteLine("Update not successful.");
        }

        private void ListAllValues()
        {
            Dictionary<string, object> values = dataSource.GetDataValues();

            foreach (KeyValuePair<string, object> item in values)
            {
                Console.WriteLine(item.Key + " - " + item.Value.ToString());
            }
        }

        private bool ValidateInput(string keyOrValue)
        {
            if (Regex.IsMatch(keyOrValue, @"^[a-zA-Z0-9_]+$"))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Only letters, numbers, and underscores allowed.");
                return false;
            }
        }
    }
}
