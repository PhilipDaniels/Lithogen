using System.Collections.Generic;
using System.ComponentModel;

namespace Lithogen.TestSite2.Models
{
    public class Planet
    {
        public string Name { get; set; }
        public int Order { get; set; }
    }

    [ImmutableObject(true)]
    public class SolarSystem
    {
        public ICollection<Planet> Planets { get; private set; }

        public SolarSystem()
        {
            Planets = new List<Planet>();
            Planets.Add(new Planet() { Name = "Mercury", Order = 1 });
            Planets.Add(new Planet() { Name = "Venus", Order = 2 });
            Planets.Add(new Planet() { Name = "Earth", Order = 3 });
            Planets.Add(new Planet() { Name = "Mars", Order = 4 });
            Planets.Add(new Planet() { Name = "Jupiter", Order = 5 });
            Planets.Add(new Planet() { Name = "Saturn", Order = 6 });
            Planets.Add(new Planet() { Name = "Uranus", Order = 7 });
            Planets.Add(new Planet() { Name = "Neptune", Order = 8 });
            Planets.Add(new Planet() { Name = "Pluto", Order = 9 });
        }
    }
}
