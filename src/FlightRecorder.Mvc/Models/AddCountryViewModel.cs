﻿using FlightRecorder.Mvc.Entities;

namespace FlightRecorder.Mvc.Models
{
    public class AddCountryViewModel : Country
    {
        public string Message { get; set; }

        public void Clear()
        {
            Id = 0;
            Name = "";
            Message = "";
        }
    }
}

