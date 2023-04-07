using System.Collections.Generic;

namespace Ieroglyphs
{
    public class Ieroglyph
    {
        public string meaning { get; set; } = "UNDEFINED";
        public List<string> spelling { get; set; } = new List<string>();
    }
}