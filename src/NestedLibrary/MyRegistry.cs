﻿using System;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace NestedLibrary
{
    public class MyRegistry : Registry
    {
        public MyRegistry()
        {
            Scan(
                x =>
                {
                    x.TheCallingAssembly();
                    x.Convention<MyConvention>();
                });
        }
    }

    public class MyConvention : IRegistrationConvention
    {
        public void ScanTypes(TypeSet types, Registry registry)
        {
            var matches = types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)
                .Where(type => type.CanBeCastTo<ITeam>());

            foreach (var type in matches)
            {
                registry.For(typeof (ITeam)).Add(type);
            }
        }
    }

    public interface ITeam{}

    public class Chiefs : ITeam{}
    public class Chargers : ITeam{}
    public class Broncos : ITeam{}
    public class Raiders : ITeam{}
}