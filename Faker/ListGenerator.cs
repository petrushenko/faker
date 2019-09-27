using System;
using System.Collections.Generic;

namespace Faker
{
    public class ListGenerator<T> : IGenericGenerator
    {
        public object Generate()
        {
            var list = new List<T>();
            var random = new Random();
            var size = random.Next(1, 30);
            var faker = new Faker();
            for (var i = 0; i < size; i++)
            {
                var item = (T)faker.Create<T>();
                list.Add(item);
            }

            return list;
        }

        public Type GetGenerationType()
        {
            return typeof(List<T>);
        }
    }
}