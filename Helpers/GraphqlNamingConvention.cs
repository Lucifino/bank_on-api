using System;
using System.Reflection;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using bank_on_api.Models.Entities;
using Humanizer;
using bank_on_api.GraphQL.Queries;
using bank_on_api.GraphQL.Mutations;

namespace bank_on_api.Helpers
{
    public class GraphQLNamingConvention : DefaultNamingConventions
    {

        public override NameString GetTypeName(Type type)
        {
            return base.GetTypeName(type);
        }

        public override NameString GetMemberName(MemberInfo member, MemberKind kind)
        {
            if (member.ReflectedType != null &&
                (member.ReflectedType == typeof(Query) || member.ReflectedType == typeof(Mutation)))
                return base.GetMemberName(member, kind).ToString().Pascalize();
            return base.GetMemberName(member, kind).ToString().Pascalize();
        }
    }
}