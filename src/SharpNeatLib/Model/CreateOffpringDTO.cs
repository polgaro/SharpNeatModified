using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNeatLib.Model
{
    public class CreateOffpringDTO<TGenome>
    {
        public List<TGenome> OffspringList { get; set; }
        public bool EmptySpeciesFlag { get; set; }
    }
}
