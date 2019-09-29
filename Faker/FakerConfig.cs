using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Faker
{
    public class FakerConfig
    {
        private Dictionary<MemberInfo, IGenerator> Generators;

        internal IGenerator GetGeneratorByName(string parameterName)
        {
            foreach (var member in Generators.Keys)
            {
                if (member.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    Generators.TryGetValue(member, out var generator);
                    return generator;
                }
            }

            return null;
        }

        internal IGenerator GetGeneratorByMemberInfo(MemberInfo memberInfo)
        {
            Generators.TryGetValue(memberInfo, out var generator);
            return generator;
        }

        public FakerConfig()
        {
            Generators = new Dictionary<MemberInfo, IGenerator>();
        }

        public void Add<TClass, TMember, TGenerator>(Expression<Func<TClass, TMember>> expressionTree)
        where TGenerator: IGenerator
        {
            var generator = (IGenerator)Activator.CreateInstance<TGenerator>();

            var body = (MemberExpression)expressionTree.Body;
            var memberInfo = body.Member;
            Generators.Add(memberInfo, generator);
        }
    }
}