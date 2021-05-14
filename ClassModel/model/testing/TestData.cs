using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClassModel.model.testing
{
    public class TestData
    {
        [Key]
        public int id { get; set; }//rigt
        public string name { get; set; }
    }
}
